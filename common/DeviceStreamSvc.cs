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
        public ActionReceivedText OnRecvdText = null;
        bool KeepAlive = false;
        bool ExpectResponse = true;

        public static string MsgOut { get; set; }
        public static string MsgIn { get; set; }

        private static AutoResetEvent MsgOutWaitHandle = null;

        private String _deviceId;
        //private string _connectionString;

        public DeviceStream_Svc(ServiceClient deviceClient, String deviceId, string _msgOut, ActionReceivedText _OnRecvdText, bool _KeepAlive=false, bool _ExpectResponse=true)
        {
            _serviceClient = deviceClient;
            _deviceId = deviceId;
            KeepAlive = _KeepAlive;
            ExpectResponse = _ExpectResponse;
            MsgOut = _msgOut;
            OnRecvdText = _OnRecvdText;
        }

        public static async Task RunSvc(string s_connectionString, String deviceId, string msgOut, ActionReceivedText _OnRecvdText, bool _KeepAlive = false, bool _Respond = true)
        {
            MsgOut = msgOut;
            if (MsgOutWaitHandle != null)
            {
                MsgOutWaitHandle.Set();
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
                        var sample = new DeviceStream_Svc(serviceClient, deviceId, msgOut, _OnRecvdText, _KeepAlive, _Respond);
                        if (sample == null)
                        {
                            System.Diagnostics.Debug.WriteLine("Failed to create DeviceStreamSvc!");
                            //return null;
                        }
                        System.Diagnostics.Debug.WriteLine("Starting Svc 1");

                        try
                        {
                            await sample.RunSvcAsync();//.GetAwaiter().GetResult();
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


                        //return null;
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

  

        public async Task RunSvcAsync()
        {
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
                    using (var cancellationTokenSource = new CancellationTokenSource(DeviceStreamingCommon._Timeout))
                        {
                        try
                        {
                            using (var stream = await DeviceStreamingCommon.GetStreamingClientAsync(result.Url, result.AuthorizationToken, cancellationTokenSource.Token).ConfigureAwait(false))
                            {
                                MsgOutWaitHandle = new AutoResetEvent(true);
                                do
                                {
                                    //Nb: Not waited on first entry as waiting for msgOut, which we already have.
                                    MsgOutWaitHandle.WaitOne();
                                    await SendMsg(stream, MsgOut, cancellationTokenSource);
                                    if (ExpectResponse)
                                    {
                                        byte[] receiveBuffer = new byte[1024];
                                        System.ArraySegment<byte> ReceiveBuffer = new ArraySegment<byte>(receiveBuffer);

                                        var receiveResult = await stream.ReceiveAsync(ReceiveBuffer, cancellationTokenSource.Token).ConfigureAwait(false);

                                        MsgIn = Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);

                                        if (OnRecvdText != null)
                                            OnRecvdText(MsgIn);

                                        System.Diagnostics.Debug.WriteLine(string.Format("Svc Received stream data: {0}", MsgIn));
                                    }
                                    MsgOutWaitHandle.Reset();
                                } while (KeepAlive);
                                MsgOutWaitHandle = null;
                                System.Diagnostics.Debug.WriteLine("Closing Svc Socket");
                                await stream.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, cancellationTokenSource.Token).ConfigureAwait(false);                            
                            }
                        }
                        catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                        {
                        System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Hub connection failure" );
                        }
                        catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                        {
                            System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Device not found" );
                        }
                        catch (TaskCanceledException)
                        {
                            System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Task canceled" );
                        }
                        catch (OperationCanceledException)
                        {
                        System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Operation canceled" ); ;
                        }
                        catch (Exception ex)
                        {
                            if (!ex.Message.Contains("Timeout"))
                                System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): " + ex.Message);
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Timeout" );
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
                System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Task canceled");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Operation canceled");
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
        }

        public static async Task SendMsg(ClientWebSocket stream, string msgOut, CancellationTokenSource cancellationTokenSource)
        {
            byte[] sendBuffer = Encoding.UTF8.GetBytes(msgOut);
            System.ArraySegment<byte> SendBuffer = new ArraySegment<byte>(sendBuffer);

            await stream.SendAsync(SendBuffer, WebSocketMessageType.Binary, true, cancellationTokenSource.Token).ConfigureAwait(false);

            System.Diagnostics.Debug.WriteLine(string.Format("Svc Sent stream data: {0}", Encoding.UTF8.GetString(sendBuffer, 0, sendBuffer.Length)));

        }
    }
}
