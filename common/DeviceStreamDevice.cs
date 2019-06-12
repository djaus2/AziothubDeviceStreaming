using Microsoft.Azure.Devices.Client;
using System;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace AzIoTHubDeviceStreams
{
    
    public class DeviceStream_Device
    {
        // From SDK in Microsoft.Azure.Devices.Client.DeviceClient
        private DeviceClient deviceClient;

        // Callback for received message
        private ActionReceivedTextIO OnRecvdTextIO = null;

        // Can trigger cancellation of DS task due to timeout or manual
        private CancellationTokenSource cancellationTokenSource = null;

        // Callback to handle received message, post stripping of flag
        public DeviceAndSvcCurrentSettings DeviceCurrentSettings {  get;  set; }

        // An instance of this class
        // Nb: Referred to for Cancel and for subsequent messages in KeepAlive mode
        public static DeviceStream_Device deviceStream_Device = null;

        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="deviceClient">Instance of class for Device Steaming from the SDK</param>
        /// <param name="_OnRecvText">Callback to handle received message, post stripping of flags</param>
        /// <param name="deviceCurrentSettings">Optional custom class to handle processing of flags</param>
        public DeviceStream_Device(DeviceClient deviceClient, ActionReceivedTextIO _OnRecvdText, DeviceAndSvcCurrentSettings deviceCurrentSettings = null)
        {
            this.deviceClient = deviceClient;
            OnRecvdTextIO = _OnRecvdText;
            if (deviceCurrentSettings != null)
                DeviceCurrentSettings = deviceCurrentSettings;
            else
                DeviceCurrentSettings = new DeviceAndSvcCurrentSettings();
        }

        /// <summary>
        /// Method called from app to instantiate this class and start Device Streaming on Device.
        /// </summary>
        /// <param name="device_cs">Device Id eg "MyDevice"</param>
        /// <param name="_OnRecvText">Callback to handle received message, post stripping of flags</param>
        /// <param name="deviceCurrentSettings">Optional custom class to handle processing of flags</param>
        /// <returns>The running task</returns>
        public static async Task RunDevice(string device_cs, ActionReceivedTextIO _OnRecvText, DeviceAndSvcCurrentSettings deviceCurrentSettings = null)
        {
            TransportType device_hubTransportTryp = DeviceStreamingCommon.device_transportType;
            try
            {
                using (DeviceClient deviceClient = DeviceClient.CreateFromConnectionString(device_cs, device_hubTransportTryp))
                {
                    if (deviceClient == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to create DeviceClient!");
                        //return null;
                    }

                    deviceStream_Device = new DeviceStream_Device(deviceClient, _OnRecvText, deviceCurrentSettings);
                    if (deviceStream_Device == null)
                    {
                        System.Diagnostics.Debug.WriteLine("Failed to create DeviceStreamClient!");
                        //return null;
                    }

                    try
                    {
                        await deviceStream_Device.RunDeviceAsync();//.GetAwaiter().GetResult();
                    }
                    catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                    {
                        System.Diagnostics.Debug.WriteLine("3 Error RunDevice(): Hub connection failure");
                    }
                    catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                    {
                        System.Diagnostics.Debug.WriteLine("3 Error RunDevice(): Device not found");
                    }
                    catch (TaskCanceledException)
                    {
                        System.Diagnostics.Debug.WriteLine("3 Error RunDevice(): Task canceled");
                    }
                    catch (OperationCanceledException)
                    {
                        System.Diagnostics.Debug.WriteLine("3 Error RunDevice(): Operation canceled");
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("Timeout"))
                            System.Diagnostics.Debug.WriteLine("3 Error RunDevice(): " + ex.Message);
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("3 Error RunDevice(): Timeout");
                        }
                    }

                }
                //return null;
            }
            catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
            {
                System.Diagnostics.Debug.WriteLine("4 Error RunDevice(): Hub connection failure");
            }
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("4 Error RunDevice(): Device not found");
            }
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("4 Error RunDevice(): Task canceled");
            }
            catch (OperationCanceledException eex)
            {
                System.Diagnostics.Debug.WriteLine("4 Error RunDevice(): Operation canceled \r\n" + eex.Message);
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Timeout"))
                    System.Diagnostics.Debug.WriteLine("4 Error RunDevice(): " + ex.Message);
                else
                {
                    System.Diagnostics.Debug.WriteLine("4 Error RunDevice(): Timeout");
                }
            }
        }

        /// <summary>
        /// Issues cancellation
        /// </summary>
        public void Cancel()
        {
            cancellationTokenSource?.Cancel();
        }

        /// <summary>
        /// Launches the task to run the Device's Device Streaming
        /// </summary>
        /// <returns></returns>
        private async Task RunDeviceAsync()
        {
            await RunDeviceAsync(true).ConfigureAwait(false);
        }


        /// <summary>
        /// The actual Device Streaming method at Device end of the pipe
        /// </summary>
        /// <param name="acceptDeviceStreamingRequest"></param>
        /// <returns></returns>
        private async Task RunDeviceAsync(bool acceptDeviceStreamingRequest)
        {
            byte[] buffer = new byte[1024];

            try
            {
                using ( cancellationTokenSource = new CancellationTokenSource(DeviceStreamingCommon._Timeout))
                {
                    try
                    {
                        System.Diagnostics.Debug.WriteLine("Starting Device Stream Request");
                        Microsoft.Azure.Devices.Client.DeviceStreamRequest streamRequest = await deviceClient.WaitForDeviceStreamRequestAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                        if (streamRequest != null)
                        {
                            if (acceptDeviceStreamingRequest)
                            {
                                await deviceClient.AcceptDeviceStreamRequestAsync(streamRequest, cancellationTokenSource.Token).ConfigureAwait(false);
                                System.Diagnostics.Debug.WriteLine("Device got a connection.");
                                using (ClientWebSocket webSocket = await DeviceStreamingCommon.GetStreamingDeviceAsync(streamRequest.Url, streamRequest.AuthorizationToken, cancellationTokenSource.Token).ConfigureAwait(false))
                                {
                                    System.Diagnostics.Debug.WriteLine(string.Format("Device got stream: Name={0}", streamRequest.Name));
                                    bool keepAlive = false;
                                    do
                                    {
                                        WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), cancellationTokenSource.Token).ConfigureAwait(false);
                                        string msgIn = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);
                                        System.Diagnostics.Debug.WriteLine(string.Format("Device Received stream data: {0}", msgIn));

                                        //Get keepAlive and respond flags and strip from msg in
                                        
                                        msgIn = DeviceCurrentSettings.ProcessMsgIn(msgIn);
                                        bool respond = DeviceCurrentSettings.ResponseExpected;
                                        keepAlive = DeviceCurrentSettings.KeepAlive;

                                        string msgOut = msgIn;
                                        if (OnRecvdTextIO != null)
                                            msgOut = OnRecvdTextIO(msgIn);

                                        if (respond)
                                        {
                                            byte[] sendBuffer = Encoding.UTF8.GetBytes(msgOut);

                                            await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer, 0, sendBuffer.Length), WebSocketMessageType.Binary, true, cancellationTokenSource.Token).ConfigureAwait(false);
                                            System.Diagnostics.Debug.WriteLine(string.Format("Device Sent stream data: {0}", Encoding.UTF8.GetString(sendBuffer, 0, sendBuffer.Length)));
                                        }
                                    //By default do not loop    
                                    } while (keepAlive);

                                    System.Diagnostics.Debug.WriteLine("Closing Device Socket");
                                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, cancellationTokenSource.Token).ConfigureAwait(false);
                                }
                            }
                            else
                            {
                                await deviceClient.RejectDeviceStreamRequestAsync(streamRequest, cancellationTokenSource.Token).ConfigureAwait(false);
                            }
                        }

                        await deviceClient.CloseAsync().ConfigureAwait(false);
                    }
                    catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                    {
                        System.Diagnostics.Debug.WriteLine("1 Error RunDeviceAsync(): Hub connection failure");
                    }
                    catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                    {
                        System.Diagnostics.Debug.WriteLine("1 Error RunDeviceAsync(): Device not found");
                    }
                    catch (TaskCanceledException)
                    {
                        System.Diagnostics.Debug.WriteLine("1 Error RunDeviceAsync(): Task canceled");
                    }
                    catch (OperationCanceledException eex)
                    {
                        System.Diagnostics.Debug.WriteLine("1 Error RunDeviceAsync(): Operation canceled \r\n" + eex.Message);
                    }
                    catch (Exception ex)
                    {
                        if (!ex.Message.Contains("Timeout"))
                            System.Diagnostics.Debug.WriteLine("1 Error RunDeviceAsync(): " + ex.Message);
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("1 Error RunDeviceAsync(): Timeout");
                        }
                    }
                }
            }
            catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
            {
                System.Diagnostics.Debug.WriteLine("2 Error RunDeviceAsync(): Hub connection failure");
            }
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("2 Error RunDeviceAsync(): Device not found");
            }
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("2 Error RunDeviceAsync(): Task canceled");
            }
            catch (OperationCanceledException eex)
            {
                System.Diagnostics.Debug.WriteLine("2 Error RunDeviceAsync(): Operation canceled \r\n" + eex.Message);
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Timeout"))
                    System.Diagnostics.Debug.WriteLine("2 Error RunDeviceAsync(): " + ex.Message);
                else
                {
                    System.Diagnostics.Debug.WriteLine("2 Error RunDeviceAsync(): Timeout");
                }
            }
            cancellationTokenSource = null;
            deviceStream_Device = null;
        }     
    }
}
