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
        private bool KeepDeviceListening = false;

        // Callback for received message
        private ActionReceivedTextIO OnRecvdTextIO = null;
        private ActionCommandD ActionCmdD = null;
        private ActionReceivedText OnDeviceStatusUpdateD = null;

        // Can trigger cancellation of DS task due to timeout or manual
        private CancellationTokenSource cancellationTokenSourceTimeout = null;
        private CancellationTokenSource cancellationTokenSourceManual = null;

        // Callback to handle received message, post stripping of flag
        public DeviceAndSvcCurrentSettings DeviceCurrentSettings { get; set; }

        // An instance of this class
        // Nb: Referred to for Cancel and for subsequent messages in KeepAlive mode
        public static DeviceStream_Device deviceStream_Device = null;

        /// <summary>
        /// The class constructor
        /// </summary>
        /// <param name="deviceClient">Instance of class for Device Steaming from the SDK</param>
        /// <param name="_OnRecvText">Callback to handle received message, post stripping of flags</param>
        /// <param name="deviceCurrentSettings">Optional custom class to handle processing of flags</param>
        public DeviceStream_Device(DeviceClient deviceClient, ActionReceivedTextIO _OnRecvdText, ActionCommandD _ActionCommand = null, ActionReceivedText _OnDeviceStatusUpdateD = null, bool _keepDeviceListening = false, DeviceAndSvcCurrentSettings deviceCurrentSettings = null)
        {
            this.deviceClient = deviceClient;
            OnRecvdTextIO = _OnRecvdText;
            OnDeviceStatusUpdateD = _OnDeviceStatusUpdateD;
            KeepDeviceListening = _keepDeviceListening;
            ActionCmdD = _ActionCommand;
            if (deviceCurrentSettings != null)
                DeviceCurrentSettings = deviceCurrentSettings;
            else
                DeviceCurrentSettings = new DeviceAndSvcCurrentSettings();
            DeviceCurrentSettings.KeepDeviceListening = KeepDeviceListening;
        }


        /// <summary>
        /// Method called from app to instantiate this class and start Device Streaming on Device.
        /// </summary>
        /// <param name="device_cs">Device Id eg "MyDevice"</param>
        /// <param name="_OnRecvText">Callback to handle received message, post stripping of flags</param>
        /// <param name="deviceCurrentSettings">Optional custom class to handle processing of flags</param>
        /// <returns>The running task</returns>
        public static async Task RunDevice(string device_cs, ActionReceivedTextIO _OnRecvText, ActionReceivedText _OnDeviceStatusUpdateD = null, ActionCommandD _ActionCommand =null, bool _keepDeviceListening = false, DeviceAndSvcCurrentSettings deviceCurrentSettings = null)
        {
            bool __keepDeviceListening = false;
            do
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

                        deviceStream_Device = new DeviceStream_Device(deviceClient, _OnRecvText, _ActionCommand, _OnDeviceStatusUpdateD,  _keepDeviceListening, deviceCurrentSettings);
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
                            if (!ex.Message.Contains("Timed out"))
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
                __keepDeviceListening = deviceStream_Device.DeviceCurrentSettings.KeepDeviceListening;
                if (__keepDeviceListening)
                    deviceStream_Device.UpdateStatus("Continuing to listen");
                else
                    deviceStream_Device.UpdateStatus("Not listening");
                //Nb: deviceStream_Device is disposed here.
            } while (__keepDeviceListening);
        }

        /// <summary>
        /// Issues cancellation
        /// </summary>
        public void Cancel()
        {
            isManualCancel = true;
            cancellationTokenSourceTimeout?.Cancel();
            KeepDeviceListening = false;
        }

        /// <summary>
        /// Launches the task to run the Device's Device Streaming
        /// </summary>
        /// <returns></returns>
        private async Task RunDeviceAsync()
        {
            await RunDeviceAsync(true).ConfigureAwait(false);
        }

        bool isManualCancel = false;
        /// <summary>
        /// The actual Device Streaming method at Device end of the pipe
        /// </summary>
        /// <param name="acceptDeviceStreamingRequest"></param>
        /// <returns></returns>
        private async Task RunDeviceAsync(bool acceptDeviceStreamingRequest)
        {
            isManualCancel = false;
            string errorMsg = "";
            string updateMsg = "";
            byte[] buffer = new byte[1024];
            ClientWebSocket webSocket = null;
            try
            {
                using (cancellationTokenSourceTimeout = new CancellationTokenSource(DeviceStreamingCommon.DeviceTimeout))
                {
                    try
                    {
                        cancellationTokenSourceTimeout.Token.Register(() =>
                        {
                            webSocket?.Abort();
                            webSocket?.Dispose();
                        });
                        updateMsg = "Starting Device Stream Request.";
                        System.Diagnostics.Debug.WriteLine(updateMsg);
                        UpdateStatus(updateMsg);

                        Microsoft.Azure.Devices.Client.DeviceStreamRequest streamRequest = await deviceClient.WaitForDeviceStreamRequestAsync(cancellationTokenSourceTimeout.Token).ConfigureAwait(false);
                        if (streamRequest != null)
                        {
                            if (acceptDeviceStreamingRequest)
                            {
                                await deviceClient.AcceptDeviceStreamRequestAsync(streamRequest, cancellationTokenSourceTimeout.Token).ConfigureAwait(false);
                                updateMsg = "Device got a connection.";
                                UpdateStatus(updateMsg);

                                using (webSocket = await DeviceStreamingCommon.GetStreamingDeviceAsync(streamRequest.Url, streamRequest.AuthorizationToken, cancellationTokenSourceTimeout.Token).ConfigureAwait(false))
                                {
                                    updateMsg = string.Format("Device got stream: Name={0}. Socket open.", streamRequest.Name);
                                    UpdateStatus(updateMsg);

                                    bool keepAlive = false;
                                    do
                                    { 
                                        
                                        updateMsg = string.Format("Device is connected and listening");
                                        UpdateStatus(updateMsg);
                                        WebSocketReceiveResult receiveResult = await webSocket.ReceiveAsync(new ArraySegment<byte>(buffer, 0, buffer.Length), cancellationTokenSourceTimeout.Token).ConfigureAwait(false);
                                        if (receiveResult.MessageType == WebSocketMessageType.Close)
                                        {
                                            UpdateStatus("Received Close msg");
                                            keepAlive = false;
                                        }
                                        else
                                        {
                                            string msgIn = Encoding.UTF8.GetString(buffer, 0, receiveResult.Count);

                                            updateMsg = string.Format("Device Received stream data: {0}.", msgIn);
                                            UpdateStatus(updateMsg);

                                            //Get keepAlive and respond flags and strip from msg in
                                            bool respond = true;
                                            try
                                            {
                                                msgIn = DeviceCurrentSettings.ProcessMsgIn(msgIn);
                                                respond = DeviceCurrentSettings.ResponseExpected;
                                                keepAlive = DeviceCurrentSettings.KeepAlive;
                                                if (DeviceCurrentSettings.AutoStartDeviceChanged)
                                                    ActionCmdD?.Invoke(DeviceCurrentSettings.AutoStartDevice, "", 0, 0);
                                                if (DeviceCurrentSettings.KeepDeviceListeningChanged)
                                                    ActionCmdD?.Invoke(DeviceCurrentSettings.KeepDeviceListening, "", 0, 1);
                                            }
                                            catch (System.NotImplementedException niex)
                                            {
                                                errorMsg += "DeviceCurrentSettings not properly Implemented";
                                                keepAlive = false;
                                                respond = false;
                                            }

                                            string msgOut = msgIn;
                                            try
                                            {
                                                if (OnRecvdTextIO != null)
                                                    msgOut = OnRecvdTextIO(msgIn);
                                            }
                                            catch (Exception exx)
                                            {
                                                errorMsg += "OnRecvdTextIO not properly nmplemented: " + exx.Message;
                                                keepAlive = false;
                                                respond = false;
                                            }

                                            if (respond)
                                            {
                                                byte[] sendBuffer = Encoding.UTF8.GetBytes(msgOut);

                                                await webSocket.SendAsync(new ArraySegment<byte>(sendBuffer, 0, sendBuffer.Length), WebSocketMessageType.Binary, true, cancellationTokenSourceTimeout.Token).ConfigureAwait(false);
                                                updateMsg = string.Format("Device Sent stream data: {0}", Encoding.UTF8.GetString(sendBuffer, 0, sendBuffer.Length));
                                                UpdateStatus(updateMsg);
                                            }
                                            //By default do not loop   
                                        }
                                    } while (keepAlive);

                                    updateMsg = "Closing Device Socket Normally.";
                                    UpdateStatus(updateMsg);
                                    await webSocket.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, cancellationTokenSourceTimeout.Token).ConfigureAwait(false);
                                    webSocket = null;
                                }
                            }
                            else
                            {
                                await deviceClient.RejectDeviceStreamRequestAsync(streamRequest, cancellationTokenSourceTimeout.Token).ConfigureAwait(false);
                            }
                        }

                        await deviceClient.CloseAsync().ConfigureAwait(false);
                    }
                    catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                    {
                        if ((bool)cancellationTokenSourceTimeout?.IsCancellationRequested)
                        {
                            System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Timed Out.");
                            errorMsg += " Timed Out";
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("1 Error RunDeviceAsync(): Hub connection failure");
                            errorMsg = "Hub connection failure";
                        }
                    }
                    catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                    {
                        System.Diagnostics.Debug.WriteLine("1 Error RunDeviceAsync(): Device not found");
                        errorMsg = "Device not found";
                    }
                    catch (TaskCanceledException)
                    {
                        System.Diagnostics.Debug.WriteLine("1 Error RunDeviceAsync(): Task cancelled");
                        errorMsg = "Task cancelled";
                    }
                    catch (OperationCanceledException eex)
                    {
                        System.Diagnostics.Debug.WriteLine("1 Error RunDeviceAsync(): Operation cancelled \r\n" + eex.Message);
                        errorMsg = "Operation cancelled";
                    }
                    catch (Exception ex)
                    {
                        if ((bool)(cancellationTokenSourceManual?.IsCancellationRequested))
                        {
                            System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Cancelled.");
                            errorMsg += " Cancelled";
                        }
                        else if ((bool)(cancellationTokenSourceTimeout?.IsCancellationRequested))
                        {
                            System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Timed Out.");
                            errorMsg += " Timed Out";
                        }
                        else if (!ex.Message.Contains("Timed out"))
                        {
                            System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): " + ex.Message);
                            errorMsg += " " + ex.Message;
                        }
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Timed out");
                            errorMsg += " Timed Out";
                        }
                    }
                }
            }
            catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
            {
                System.Diagnostics.Debug.WriteLine("2 Error RunDeviceAsync(): Hub connection failure");
                errorMsg = "Hub connection failure";
            }
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("2 Error RunDeviceAsync(): Device not found");
                errorMsg = "Device not found";
            }
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("2 Error RunDeviceAsync(): Task cancelled");
                errorMsg = "Task cancelled";
            }
            catch (OperationCanceledException eex)
            {
                System.Diagnostics.Debug.WriteLine("2 Error RunDeviceAsync(): Operation cancelled \r\n" + eex.Message);
                errorMsg = "Operation cancelled";
            }
            catch (Exception ex)
            {
                if ((bool)(cancellationTokenSourceManual?.IsCancellationRequested))
                {
                    System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Cancelled.");
                    errorMsg += " Cancelled";
                }
                else if ((bool)(cancellationTokenSourceTimeout?.IsCancellationRequested))
                {
                    System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Timed Out.");
                    errorMsg += " Timed Out";
                }
                else if (!ex.Message.Contains("Timed out"))
                {
                    System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): " + ex.Message);
                    errorMsg += " " + ex.Message;
                }
                else
                {
                    System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Timed out");
                    errorMsg += " Timed Out";
                }
            };
            if (webSocket != null)
            {
                if (webSocket.CloseStatus != WebSocketCloseStatus.NormalClosure)
                {
                    updateMsg = "Aborting Device Socket as is errant or cancelled: " + errorMsg;
                    UpdateStatus(updateMsg);
                    webSocket.Abort();
                    updateMsg = "Aborted Device Socket as was errant or cancelled: " + errorMsg;
                    UpdateStatus(updateMsg);
                }
                else
                {
                    updateMsg = "Socket closed normally: " + errorMsg;
                    UpdateStatus(updateMsg);
                }
            }
            else
            {
                if (isManualCancel)
                    updateMsg = "Socket closed Normally: Manually ";
                else
                    updateMsg = "Socket closed Normally: " + errorMsg;
                UpdateStatus(updateMsg);
            }
            webSocket = null;
            cancellationTokenSourceTimeout = null;            
        }


        private void UpdateStatus(string msg)
        {
            System.Diagnostics.Debug.WriteLine("Device: " + msg);
            if (OnDeviceStatusUpdateD != null)
                    OnDeviceStatusUpdateD(msg);
        }
    }
}
