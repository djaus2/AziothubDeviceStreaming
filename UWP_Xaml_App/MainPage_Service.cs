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
                    tbSvcStat.Text = msgIn;
                });
            });
        }
        private void ButtonCanceLSvc_Click(object sender, RoutedEventArgs e)
        {
            DeviceStream_Svc.deviceStream_Svc?.Cancel();
        }

        public bool svcCustomClassMode { get; set; } = false;
        public bool svcBasicMode { get; set; } = false;

        public int DevAutoStart { get; set; } = 2;
        public int DevKeepListening { get; set; }  = 2;

        private async void Button_Click_Svc(object sender, RoutedEventArgs e)
        {
            string msgOut = tbSvcMsgOut.Text;

            

            //Store these current values then reset. These vales are passed to the device and remain so until changed.
            //Whereas 
            int devAutoStart = DevAutoStart;
            int devKeepListening = DevKeepListening;


            ClearAllToggles();

            //These values are passed if true with each connection. If not passed then the device clears them.
            bool keepAlive = (chkKeepAlive.IsChecked == true);
            bool responseExpected = (chkExpectResponse.IsChecked == true);
            


            if (!DeviceStream_Svc.SignalSendMsgOut(msgOut, keepAlive, responseExpected))
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if(svcBasicMode)
                            DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText).GetAwaiter().GetResult();
                        else if (!svcCustomClassMode)
                            DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText, devKeepListening, devAutoStart, OnDeviceSvcUpdate, keepAlive, responseExpected).GetAwaiter().GetResult();

                        else
                            DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText, devKeepListening, devAutoStart, OnDeviceSvcUpdate, keepAlive, responseExpected, new DeviceSvcCurrentSettings_Example()).GetAwaiter().GetResult();
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
