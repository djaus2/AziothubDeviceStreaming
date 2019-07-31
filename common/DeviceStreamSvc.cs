// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Linq;
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
        public ActionReceivedText OnStatusUpdateD = null;

        public string MsgOut { get; set; }
        public static string MsgIn { get; set; }

        private  AutoResetEvent MsgOutWaitHandle = null;
        public static DeviceStream_Svc deviceStream_Svc = null;

        DeviceAndSvcCurrentSettings SvcCurrentSettings = null;

        public int DevKeepListening { get; set; } = 2;
        public int DevAutoStart { get; set; } = 2;

        private String _deviceId;

        public DeviceStream_Svc(ServiceClient deviceClient, String deviceId, string msgOut, ActionReceivedText onRecvdTextD, int devKeepListening = 2, int devAutoStart = 2,ActionReceivedText onStatusUpdateD = null, bool keepAlive = false, bool responseExpected = true, DeviceAndSvcCurrentSettings svcCurrentSettings = null)
        {
            _serviceClient = deviceClient;
            _deviceId = deviceId;
            OnRecvdTextD = onRecvdTextD;
            OnStatusUpdateD = onStatusUpdateD;
            if (svcCurrentSettings != null)
                SvcCurrentSettings = svcCurrentSettings;
            else
                SvcCurrentSettings = new DeviceAndSvcCurrentSettings();
            SvcCurrentSettings.KeepAlive = keepAlive;
            SvcCurrentSettings.ResponseExpected = responseExpected;
            DevKeepListening = devKeepListening;
            DevAutoStart = devAutoStart;
            MsgOut = msgOut; 
        }

        public static bool SignalSendMsgOut(string msgOut, bool keepAlive, bool responseExpected)
        {
            if (deviceStream_Svc != null)
            {
                if (deviceStream_Svc.MsgOutWaitHandle != null)
                {
                    if (deviceStream_Svc.SvcCurrentSettings != null)
                    {
                        //msgOut = deviceStream_Svc.SvcCurrentSettings.ProcessMsgOut(msgOut, keepAlive, responseExpected);
                        deviceStream_Svc.MsgOut = msgOut;;
                        deviceStream_Svc.SvcCurrentSettings.KeepAlive = keepAlive; ;
                        deviceStream_Svc.SvcCurrentSettings.ResponseExpected = responseExpected; ;
                        deviceStream_Svc.MsgOutWaitHandle.Set();
                        return true;
                    }
                }
            }
            return false;
        }

        public static async Task RunSvc(string s_connectionString, String deviceId, string msgOut, ActionReceivedText onRecvdTextD, int devKeepListening=2, int devAutoStart=2, ActionReceivedText oOnStatusUpdate=null, bool keepAlive = false, bool responseExpected = true, DeviceAndSvcCurrentSettings deviceAndSvcCurrentSettings = null )
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

                        deviceStream_Svc = new DeviceStream_Svc(serviceClient, deviceId, msgOut, onRecvdTextD,  devKeepListening , devAutoStart , oOnStatusUpdate, keepAlive,responseExpected, deviceAndSvcCurrentSettings);
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
                                if (!ex.Message.Contains("Timed out"))
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
            if (cancellationTokenSourceTimeout==null)
                cancellationTokenSourceManual?.Cancel();
            else
                cancellationTokenSourceTimeout?.Cancel();
        }

        private CancellationTokenSource cancellationTokenSourceManual= null;
        private CancellationTokenSource cancellationTokenSourceTimeout = null;

        private async Task RunSvcAsync()
        {
            string errorMsg = "";
            string updateMsg = "";
            
            using (cancellationTokenSourceManual = new CancellationTokenSource())
            {
                ClientWebSocket stream = null;
                try
                {
                    DeviceStreamRequest deviceStreamRequest = new DeviceStreamRequest(
                        streamName: "TestStream"
                    );
                    updateMsg = "Starting Svc TestStream";
                    UpdateStatus(updateMsg);

                    cancellationTokenSourceManual.Token.Register(() =>
                    {
                        _serviceClient?.CloseAsync();
                        _serviceClient?.Dispose();
                    });

                    DeviceStreamResponse result = await _serviceClient.CreateStreamAsync(_deviceId, deviceStreamRequest).ConfigureAwait(false);

                    updateMsg = string.Format("Svc Stream response received: Name={0} IsAccepted={1}", deviceStreamRequest.StreamName, result.IsAccepted);
                    UpdateStatus(updateMsg);

                    if (result.IsAccepted)
                    {
                        using (cancellationTokenSourceTimeout = new CancellationTokenSource(DeviceStreamingCommon.DeviceTimeout))
                        {
                            try
                            {
                                using (stream = await DeviceStreamingCommon.GetStreamingDeviceAsync(result.Url, result.AuthorizationToken, cancellationTokenSourceTimeout.Token).ConfigureAwait(false))
                                {
                                    updateMsg = "Stream is open.";
                                    UpdateStatus(updateMsg);
                                    bool keepAlive = false;
                                    MsgOutWaitHandle = new AutoResetEvent(true);
                                    do
                                    {
                                        //Nb: Not waited on first entry as waiting for msgOut, which we already have.
                                        updateMsg = "Stream is open. Waiting for msg to send.";
                                        UpdateStatus(updateMsg);

                                        MsgOutWaitHandle.WaitOne();
                                        updateMsg = "Sending msg.";
                                        UpdateStatus(updateMsg);
                                        bool caught = false;
                                        try
                                        {
                                          MsgOut = SvcCurrentSettings.ProcessMsgOut(MsgOut, SvcCurrentSettings.KeepAlive, SvcCurrentSettings.ResponseExpected, DevKeepListening , DevAutoStart );
                                        } catch (NotImplementedException )
                                        {
                                            errorMsg += "DeviceCurrentSettings not properly implemented";
                                            keepAlive = false;
                                            caught = true;
                                        }
                                        if (!caught)
                                        {
                                            await SendMsg(stream, MsgOut, cancellationTokenSourceTimeout);
                                            updateMsg = "Sent msg.";
                                            UpdateStatus(updateMsg);

                                            if (this.SvcCurrentSettings.ResponseExpected)
                                            {
                                                byte[] receiveBuffer = new byte[1024];
                                                System.ArraySegment<byte> ReceiveBuffer = new ArraySegment<byte>(receiveBuffer);

                                                var receiveResult = await stream.ReceiveAsync(ReceiveBuffer, cancellationTokenSourceTimeout.Token).ConfigureAwait(false);

                                                MsgIn = Encoding.UTF8.GetString(receiveBuffer, 0, receiveResult.Count);
                                                string subStrn = AzIoTHubDeviceStreams.DeviceStreamingCommon.DeiceInSimuatedDeviceModeStrn;
                                                int subStrnLen = subStrn.Length;
                                                if (MsgIn.Length>= subStrnLen)
                                                    if (MsgIn.Substring(0,subStrnLen) == subStrn)
                                                {
                                                    MsgIn = MsgIn.Substring(subStrnLen);
                                                    AzIoTHubModules.IoTMessage iotHubMessage = AzIoTHubModules.IoTMessage.Deserialsie(MsgIn);
                                                    Microsoft.Azure.Devices.Client.Message message = iotHubMessage.ToMessage();
                                                    Microsoft.Azure.EventHubs.EventData eventData = AzIoTHubModules.IoTMessage.ToEventData(message);
                                                    MsgIn = AzIoTHubModules.IoTMessage.EventData_ToString(eventData);
                                                }
                                                keepAlive = false;
                                                if (SvcCurrentSettings != null)
                                                    keepAlive = this.SvcCurrentSettings.KeepAlive;
                                                try
                                                {
                                                    if (OnRecvdTextD != null)
                                                        OnRecvdTextD(MsgIn);
                                                }
                                                catch (Exception exx)
                                                {
                                                    errorMsg += "OnRecvdTextD not properly implemented: " + exx.Message;
                                                    keepAlive = false;
                                                }

                                                updateMsg = string.Format("Svc Received stream data: {0}", MsgIn);
                                                UpdateStatus(updateMsg);
                                            }
                                            MsgOutWaitHandle.Reset();
                                        }
                                    } while (keepAlive) ;
                                    MsgOutWaitHandle = null;
                                    updateMsg = "Closing Svc Socket";
                                    UpdateStatus(updateMsg);
                                    await stream.CloseAsync(WebSocketCloseStatus.NormalClosure, String.Empty, cancellationTokenSourceTimeout.Token).ConfigureAwait(false);
                                    stream = null;
                                }
                            }
                            catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                            {
                                System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Hub connection failure");
                                errorMsg = "Hub connection failure";
                            }
                            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                            {
                                System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Device not found");
                                errorMsg = "Device not found";
                            }
                            catch (TaskCanceledException)
                            {
                                System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Task cancelled");
                                errorMsg = "Task cancelled";
                            }
                            catch (OperationCanceledException)
                            {
                                System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Operation cancelled");
                                errorMsg = "Operation cancelled";
                            }
                            catch (Exception ex)
                            {
                                if ((bool)cancellationTokenSourceManual?.IsCancellationRequested)
                                {
                                    System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Cancelled.");
                                    errorMsg = "Cancelled";
                                }
                                else if ((bool)cancellationTokenSourceTimeout?.IsCancellationRequested)
                                {
                                    System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): Timed Out.");
                                    errorMsg = "Timed Out";
                                }
                                else if (!ex.Message.Contains("Timed out"))
                                {
                                    System.Diagnostics.Debug.WriteLine("1 Error RunSvcAsync(): " + ex.Message);
                                    errorMsg = ex.Message;
                                }
                                else
                                {
                                    System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Timed out");
                                    errorMsg = "Timed Out";
                                }
                            }
                        }
                    }
                }
                catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                {
                    System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Hub connection failure");
                    errorMsg += " Hub connection failure";
                }
                catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                {
                    System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Device not found");
                    errorMsg += " Device not found";
                }
                catch (TaskCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Task cancelled");
                    errorMsg += " Task cancelled";
                }
                catch (OperationCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Operation cancelled");
                    errorMsg += " Operation cancelled";
                }
                catch (Exception ex)
                { if ((bool)cancellationTokenSourceManual?.IsCancellationRequested)
                    {
                        System.Diagnostics.Debug.WriteLine("2 Error RunSvcAsync(): Cancelled.");
                        errorMsg += " Cancelled";
                    }
                    else if ((bool)cancellationTokenSourceTimeout?.IsCancellationRequested)
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

                if (stream != null)
                {
                    if (stream.CloseStatus != WebSocketCloseStatus.NormalClosure)
                    {
                        updateMsg= "Aborting Svc Socket as is errant or cancelled: " + errorMsg;
                        UpdateStatus(updateMsg);
                        stream.Abort();
                        updateMsg = "Aborted Svc Socket as was errant or cancelled:" + errorMsg;
                        UpdateStatus(updateMsg);
                    }
                    else
                    {
                        updateMsg = "Socket closed normally: " + errorMsg;
                        UpdateStatus(updateMsg);
                    }
                    stream = null;
                }
                else
                {
                    updateMsg = "Socket closed Normally: " + errorMsg; 
                    UpdateStatus(updateMsg);
                }

                deviceStream_Svc = null;
                MsgOutWaitHandle = null;
                cancellationTokenSourceTimeout = null;
            }
        }

        private static async Task SendMsg(ClientWebSocket stream, string msgOut, CancellationTokenSource cancellationTokenSource)
        {
            byte[] sendBuffer = Encoding.UTF8.GetBytes(msgOut);
            System.ArraySegment<byte> SendBuffer = new ArraySegment<byte>(sendBuffer);

            await stream.SendAsync(SendBuffer, WebSocketMessageType.Binary, true, cancellationTokenSource.Token).ConfigureAwait(false);

            System.Diagnostics.Debug.WriteLine(string.Format("Svc Sent stream data: {0}", Encoding.UTF8.GetString(sendBuffer, 0, sendBuffer.Length)));

        }

        private void UpdateStatus(string msg)
        {
            System.Diagnostics.Debug.WriteLine("Service: " + msg);
            if (OnStatusUpdateD != null)
                OnStatusUpdateD(msg);
        }
    }
}
