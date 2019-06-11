// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

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
        private DeviceClient _deviceClient;
        private ActionReceivedTextIO OnRecvdTextIO = null;


        public static DeviceStream_Device deviceStream_Device = null;

        public DeviceStream_Device(DeviceClient deviceClient, ActionReceivedTextIO _OnRecvdText)
        {
            _deviceClient = deviceClient;
            OnRecvdTextIO = _OnRecvdText;
        }

        public static async Task RunDevice(string device_cs, ActionReceivedTextIO _OnRecvText)
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

                    deviceStream_Device = new DeviceStream_Device(deviceClient, _OnRecvText);
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

        public void Cancel()
        {
            cancellationTokenSource?.Cancel();
        }

        private async Task RunDeviceAsync()
        {
            await RunDeviceAsync(true).ConfigureAwait(false);
        }

        private CancellationTokenSource cancellationTokenSource = null;

        private async Task RunDeviceAsync(bool acceptDeviceStreamingRequest)
        {
            byte[] buffer = new byte[1024];

            try
            {
                //System.Diagnostics.Debug.WriteLine("Device-1");
                using ( cancellationTokenSource = new CancellationTokenSource(DeviceStreamingCommon._Timeout))
                {
                    try
                    {
                        //System.Diagnostics.Debug.WriteLine("Device-2");
                        System.Diagnostics.Debug.WriteLine("Starting Device Stream Request");
                        Microsoft.Azure.Devices.Client.DeviceStreamRequest streamRequest = await _deviceClient.WaitForDeviceStreamRequestAsync(cancellationTokenSource.Token).ConfigureAwait(false);
                        //System.Diagnostics.Debug.WriteLine("Device-3");
                        if (streamRequest != null)
                        {
                            if (acceptDeviceStreamingRequest)
                            {
                                await _deviceClient.AcceptDeviceStreamRequestAsync(streamRequest, cancellationTokenSource.Token).ConfigureAwait(false);
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
                                        DeviceCurrentSettings deviceCurrentSettings = new DeviceCurrentSettings();
                                        msgIn = deviceCurrentSettings.ProcessMsgIn(msgIn);
                                        bool respond = deviceCurrentSettings.Respond;
                                        keepAlive = deviceCurrentSettings.KeepAlive;

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
                                await _deviceClient.RejectDeviceStreamRequestAsync(streamRequest, cancellationTokenSource.Token).ConfigureAwait(false);
                            }
                        }

                        await _deviceClient.CloseAsync().ConfigureAwait(false);
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
