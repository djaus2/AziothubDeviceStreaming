// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Net.Sockets;
using System.Net.WebSockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;

namespace AzIoTHubDeviceStreams
{
    
    public class DeviceStream_Svc
    {

        //Microsoft.Azure.Devices.ServiceClient ;
        private ServiceClient _serviceClient;
        public ActionReceivedText OnRecvdTextD = null;

        public string MsgOut { get; set; }
        public static string MsgIn { get; set; }

        private  AutoResetEvent MsgOutWaitHandle = null;
        public static DeviceStream_Svc deviceStream_Svc = null;

        DeviceAndSvcCurrentSettings SvcCurrentSettings = null;

        private String _deviceId;

        public DeviceStream_Svc(ServiceClient deviceClient, String deviceId, string _msgOut, ActionReceivedText _OnRecvdTextD, bool keepAlive = false, bool responseExpected = true, DeviceAndSvcCurrentSettings svcCurrentSettings = null)
        {
            _serviceClient = deviceClient;
            _deviceId = deviceId;
            OnRecvdTextD = _OnRecvdTextD;
            if (svcCurrentSettings != null)
                SvcCurrentSettings = svcCurrentSettings;
            else
                SvcCurrentSettings = new DeviceAndSvcCurrentSettings();
            SvcCurrentSettings.KeepAlive = keepAlive;
            SvcCurrentSettings.ResponseExpected = responseExpected;
            MsgOut = SvcCurrentSettings.ProcessMsgOut(_msgOut, keepAlive, responseExpected);
        }

        public static bool SignalSendMsgOut(string msgOut, bool keepAlive, bool responseExpected)
        {
            if (deviceStream_Svc != null)
            {
                if (deviceStream_Svc.MsgOutWaitHandle != null)
                {
                    if (deviceStream_Svc.SvcCurrentSettings != null)
                    {
                        msgOut = deviceStream_Svc.SvcCurrentSettings.ProcessMsgOut(msgOut, keepAlive, responseExpected);
                        deviceStream_Svc.MsgOut = msgOut;;
                        deviceStream_Svc.MsgOutWaitHandle.Set();
                        return true;
                    }
                }
            }
            return false;
        }

        public static async Task RunSvc(string s_connectionString, String deviceId, string msgOut, ActionReceivedText _OnRecvdTextD, bool keepAlive = false, bool responseExpected = true, DeviceAndSvcCurrentSettings deviceAndSvcCurrentSettings = null )
        {
            if (deviceStream_Svc != null)
            {
                System.Diagnostics.Debug.WriteLine("Svc Socket is already open!");
                return;
            }
            else
            {
                
                try
                {
                    using (ServiceClient serviceClient = ServiceClient.CreateFromConnectionString(s_connectionString, DeviceStreamingCommon.s_transportType))
                    {
                        if (serviceClient == null)
                        {
                            System.Diagnostics.Debug.WriteLine("Failed to create SericeClient!");
                            //return null;
                        }

                        //Attach keepalive and respond info if set

                        deviceStream_Svc = new DeviceStream_Svc(serviceClient, deviceId, msgOut, _OnRecvdTextD, keepAlive,responseExpected, deviceAndSvcCurrentSettings);
                        if (deviceStream_Svc == null)
                        {
                            System.Diagnostics.Debug.WriteLine("Failed to create DeviceStreamSvc!");
                        }
                        else
                        {
                            try
                            {
                                await deviceStream_Svc.RunSvcAsync();//.GetAwaiter().GetResult();
                            }
                            catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                            {
                                System.Diagnostics.Debug.WriteLine("3 Error RunSvc(): Hub connection failure");
                            }
                            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                            {
                                System.Diagnostics.Debug.WriteLine("3 Error RunSvc(): Device not found");
                            }
                            catch (TaskCanceledException)
                            {
                                System.Diagnostics.Debug.WriteLine("3 Error RunSvc(): Task canceled");
                            }
                            catch (OperationCanceledException)
                            {
                                System.Diagnostics.Debug.WriteLine("3 Error RunSvc(): Operation canceled");
                            }
                            catch (Exception ex)
                            {
                                if (!ex.Message.Contains("Timeout"))
                                    System.Diagnostics.Debug.WriteLine("3 Error RunSvc(): " + ex.Message);
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("3 Error RunSvc(): Timeout");
                                }
                            }
                        }
                    }
                }
                catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                {
                    System.Diagnostics.Debug.WriteLine("4 Error RunSvc(): Hub connection failure");
                }
                catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                {
                    System.Diagnostics.Debug.WriteLine("4 Error RunSvc(): Device not found");
                }
                catch (TaskCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("4 Error RunSvc(): Task canceled");
                }
                catch (OperationCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("4 Error RunSvc(): Operation canceled");
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("Timeout"))
                        System.Diagnostics.Debug.WriteLine("4 Error RunSvc(): " + ex.Message);
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("4 Error RunSvc(): Timeout");
                    }
                }
            }
            
        }

        public void Cancel()
        {
            cancellationTokenSource2?.Cancel();
            cancellationTokenSource?.Cancel();
        }

        private CancellationTokenSource cancellationTokenSource2= null;
        private CancellationTokenSource cancellationTokenSource = null;

        private async Task RunSvcAsync()
        {
            using (cancellationTokenSource2 = new CancellationTokenSource())
            {
                ClientWebSocket stream = null;
                try
                {
                    DeviceStreamRequest deviceStreamRequest = new DeviceStreamRequest(
                        streamName: "TestStream"
                    );
                    System.Diagnostics.Debug.WriteLine("Starting Svc TestStream");
                    DeviceStreamResponse result = await _serviceClient.CreateStreamAsync(_deviceId, deviceStreamRequest).ConfigureAwait(false);

                    System.Diagnostics.Debug.WriteLine(string.Format("Svc Stream response received: Name={0} IsAccepted={1}", deviceStreamRequest.StreamName, result.IsAccepted));

                    if (result.IsAccepted)
                    {
                        using (cancellationTokenSource = new CancellationTokenSource(DeviceStreamingCommon._Timeout))
                        {
                            try
                            {
                                using (stream = await DeviceStreamingCommon.GetStreamingDeviceAsync(result.Url, result.AuthorizationToken, cancellationTokenSource.Token).ConfigureAwait(false))
                                {
                                    MsgOutWaitHandle = new AutoResetEvent(true);
                                    do
                                    {
                                        //Nb: Not waited on first entry as waiting for msgOut, which we already have.
                                        MsgOutWaitHandle.WaitOne();
                                        await SendMsg(stream, MsgOut, cancellationTokenSource);
                                        if (this.SvcCurrentSettings.ResponseExpected)
                                        {
                                            byte[] receiveBuffer = new byte[1024];
                                            System.ArraySegment<byte> ReceiveBuffer = new ArraySegment<byte>(receiveBuffer);

                                            var receiveResult = await stream.ReceiveAsync(ReceiveBuffer, cancellationTokenSource.Token).ConfigureAwait(false);

                                            MsgIn = Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);

                                            if (OnRecvdTextD != null)
                                                OnRecvdTextD(MsgIn);

                                            System.Diagnostics.Debug.WriteLine(string.Format("Svc Received stream data: {0}", MsgIn));
                                        }
                                        MsgOutWaitHandle.Reset();
                                    } while (this.SvcCurrentSettings.KeepAlive);
                                    MsgOutWaitHandle = null;
                                    System.Diagnostics.Debug.WriteLine("Closing Svc Socket");
                                    await stream.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, cancellationTokenSource.Token).ConfigureAwait(false);
                                    stream = null;
                                }
                            }
                            catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                            {
                                System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Hub connection failure");
                            }
                            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                            {
                                System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Device not found");
                            }
                            catch (TaskCanceledException)
                            {
                                System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Task cancelled");
                            }
                            catch (OperationCanceledException)
                            {
                                System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Operation canceleld"); ;
                            }
                            catch (Exception ex)
                            {
                                if (!ex.Message.Contains("Timeout"))
                                    System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): " + ex.Message);
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Timeout");
                                }
                            }
                        }
                    }
                }
                catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                {
                    System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Hub connection failure");
                }
                catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                {
                    System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Device not found");
                }
                catch (TaskCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Task cancelled");
                }
                catch (OperationCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Operation cancelled");
                }
                catch (Exception ex)
                {
                    if (!ex.Message.Contains("Timeout"))
                        System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): " + ex.Message);
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Timeout");
                    }
                }
           ;
                if (stream != null)
                {
                    if (stream.CloseStatus != WebSocketCloseStatus.NormalClosure)
                    {
                        System.Diagnostics.Debug.WriteLine("Aborting Svc Socket as is errant or cancelled");
                        stream.Abort();
                    }
                }
                deviceStream_Svc = null;
                MsgOutWaitHandle = null;
                cancellationTokenSource = null;
            }
        }

        private static async Task SendMsg(ClientWebSocket stream, string msgOut, CancellationTokenSource cancellationTokenSource)
        {
            byte[] sendBuffer = Encoding.UTF8.GetBytes(msgOut);
            System.ArraySegment<byte> SendBuffer = new ArraySegment<byte>(sendBuffer);

            await stream.SendAsync(SendBuffer, WebSocketMessageType.Binary, true, cancellationTokenSource.Token).ConfigureAwait(false);

            System.Diagnostics.Debug.WriteLine(string.Format("Svc Sent stream data: {0}", Encoding.UTF8.GetString(sendBuffer, 0, sendBuffer.Length)));

        }
    }
}
