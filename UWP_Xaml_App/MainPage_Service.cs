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

        private void OnDeviceSvcUpdate(string msgIn)
        {
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tbSvcStatus.Text = msgIn;
                });
            });
        }
        private void ButtonCanceLSvc_Click(object sender, RoutedEventArgs e)
        {
            DeviceStream_Svc.deviceStream_Svc?.Cancel();
        }

        private async void Button_Click_Svc(object sender, RoutedEventArgs e)
        {
            string msgOut = tbSvcMsgOut.Text;

            switch (DevKeepListening)
            {
                case 0:
                    msgOut = DeviceAndSvcCurrentSettings.Info.KeepDeviceListeningChar + msgOut;
                        break;
                case 1:
                    msgOut = DeviceAndSvcCurrentSettings.Info.UnKeepDeviceListeningChar + msgOut;
                    break;
                case 2:
                    break;
            }
            switch (DevAutoStart)
            {
                case 0:
                    msgOut = DeviceAndSvcCurrentSettings.Info.AutoStartDevice + msgOut;
                    break;
                case 1:
                    msgOut = DeviceAndSvcCurrentSettings.Info.UnAutoStartDevice+ msgOut;
                    break;
                case 2:
                    break;
            }
            bool keepAlive = (chkKeepAlive.IsChecked == true);
            bool responseExpected = (chkExpectResponse.IsChecked == true);
            bool useCustomClass = (ChkUseCustomClassSvc.IsChecked == true);



            if (!DeviceStream_Svc.SignalSendMsgOut(msgOut, keepAlive,responseExpected))
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (!useCustomClass)
                            DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText, OnDeviceSvcUpdate, keepAlive, responseExpected).GetAwaiter().GetResult();
                        else
                            DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText, OnDeviceSvcUpdate, keepAlive, responseExpected, new DeviceSvcCurrentSettings_Example()).GetAwaiter().GetResult();
                    }
                    catch (TaskCanceledException)
                    {
                        System.Diagnostics.Debug.WriteLine("Error App.RunSvc(): Task cancelled");
                    }
                    catch (OperationCanceledException)
                    {
                        System.Diagnostics.Debug.WriteLine("Error App.RunSvc(): Operation cancelled");
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine("Error App.RunSvc(): " + ex.Message);
                    }
                });

            }
        }
    }
}
