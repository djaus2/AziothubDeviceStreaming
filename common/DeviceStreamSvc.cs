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

        public string msgOut { get; set; }
        public string msgIn { get; set; }

        private String _deviceId;
        //private string _connectionString;

        public DeviceStream_Svc(ServiceClient deviceClient, String deviceId, string _msgOut, ActionReceivedText _OnRecvdText)
        {
            _serviceClient = deviceClient;
            _deviceId = deviceId;
            msgOut = _msgOut;
            OnRecvdText = _OnRecvdText;
        }

        public static async Task RunSvc(string s_connectionString, String deviceId, string msgOut, ActionReceivedText _OnRecvdText)
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
                        var sample = new DeviceStream_Svc(serviceClient, deviceId, msgOut, _OnRecvdText);
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
        
        public async Task RunSvcAsync()
        {
            try
            {
                int xx = 0;
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
                                xx++;
                                    byte[] sendBuffer = Encoding.UTF8.GetBytes(msgOut);
                                    byte[] receiveBuffer = new byte[1024];
                                xx++;
                                    await stream.SendAsync(sendBuffer, WebSocketMessageType.Binary, true, cancellationTokenSource.Token).ConfigureAwait(false);
                                xx++;
                                    System.Diagnostics.Debug.WriteLine(string.Format("Svc Sent stream data: {0}", Encoding.UTF8.GetString(sendBuffer, 0, sendBuffer.Length)));

                                    var receiveResult = await stream.ReceiveAsync(receiveBuffer, cancellationTokenSource.Token).ConfigureAwait(false);
                                xx++;
                                    msgIn = Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);

                                if (OnRecvdText != null)
                                     OnRecvdText(msgIn);

                                System.Diagnostics.Debug.WriteLine(string.Format("Svc Received stream data: {0}", msgIn));
                                }
                            }
                            catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                            {
                            System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Hub connection failure " + xx.ToString());
                            }
                            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                            {
                                System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Device not found " + xx.ToString());
                            }
                            catch (TaskCanceledException)
                            {
                                System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Task canceled " + xx.ToString());
                            }
                            catch (OperationCanceledException)
                            {
                            System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Operation canceled " + xx.ToString()); ;
                            }
                            catch (Exception ex)
                            {
                                if (!ex.Message.Contains("Timeout"))
                                    System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): " + ex.Message);
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Timeout " + xx.ToString());
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
    }
}
