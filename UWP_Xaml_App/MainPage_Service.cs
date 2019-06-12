using AzIoTHubDeviceStreams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPXamlApp
{


    sealed partial class MainPage : Page
    {
        private void OnSvcRecvText(string recvdMsg)
        {
            //Action here the returned msg:


            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tbSvcMsgIn.Text = recvdMsg;
                });
            });
        }
        private void ButtonCanceLSvc_Click(object sender, RoutedEventArgs e)
        {
            DeviceStream_Svc.deviceStream_Svc?.Cancel();
        }

        private async void Button_Click_Svc(object sender, RoutedEventArgs e)
        {
            double to;
            if (double.TryParse(tbSvcTimeout.Text, out to))
                DeviceStreamingCommon._Timeout = TimeSpan.FromMilliseconds(to);
            string msgOut = tbSvcMsgOut.Text;
            bool keepAlive = (chkKeepAlive.IsChecked == true);
            bool responseExpected = (chkExpectResponse.IsChecked == true);
            bool useCustomClass = (ChkUseCustomClassSvc.IsChecked == true);

            if (!DeviceStream_Svc.SignalSendMsgOut(msgOut, keepAlive,responseExpected))
            {
                try
                {
                    await Task.Run(() =>
                    {
                        try
                        {
                            if(!useCustomClass)
                                DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText, keepAlive, responseExpected).GetAwaiter().GetResult();
                            else
                                DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText, keepAlive, responseExpected, new DeviceSvcCurrentSettings_Example()).GetAwaiter().GetResult();
                        }
                        catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                        {
                            System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Hub connection failure");
                        }
                        catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                        {
                            System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Device not found");
                        }
                        catch (TaskCanceledException)
                        {
                            System.Diagnostics.Debug.WriteLine("0Error App.RunSvc(): Task canceled");
                        }
                        catch (OperationCanceledException)
                        {
                            System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Operation canceled");
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("Timeout"))
                                System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): " + ex.Message);
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Timeout");
                            }
                        }
                    });

                }
                catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                {
                    System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Hub connection failure");
                }
                catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                {
                    System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Device not found");
                }
                catch (TaskCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("00 Error RunSvc(): Task canceled");
                }
                catch (OperationCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("00 Error RunSvc(): Operation canceled");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Timeout"))
                        System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): " + ex.Message);
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Timeout");
                    }
                }
            }

        }
    }
}
