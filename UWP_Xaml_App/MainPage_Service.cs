﻿using AzIoTHubDeviceStreams;
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

            

            //Store these current values then reset. These vales are passed to the device and remain so until changed.
            //Whereas 
            int devAutoStart = DevAutoStart;
            int devKeepListening = DevKeepListening;
            rbNoChangeListening.IsChecked = true;
            rbNoChangeAutoStart.IsChecked = true;

            //These values are passed if true with each connection. If not passed then the device clears them.
            bool keepAlive = (chkKeepAlive.IsChecked == true);
            bool responseExpected = (chkExpectResponse.IsChecked == true);
            bool svcCustomClassMode = (rbSvcModeExp.IsChecked == true);
            bool svcBasicMode = (rbSvcModeBasic.IsChecked == true);


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

        private int DevKeepListening = 2;
        private void RbKeepListening_Checked(object sender, RoutedEventArgs e)
        {
            string dm = Convert.ToString(((RadioButton)sender)?.Tag);
            if (!string.IsNullOrEmpty(dm))
            {
                if (!int.TryParse(dm, out DevKeepListening))
                    DevKeepListening = 2;
                SvcCommands.IsOpen = false;
            }

        }

        private int DevAutoStart = 2;
        private void RbAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            string dm = Convert.ToString(((RadioButton)sender)?.Tag);
            if (!string.IsNullOrEmpty(dm))
            {
                if (!int.TryParse(dm, out DevAutoStart))
                    DevAutoStart = 2;
                SvcCommands.IsOpen = false;
            }
        }



    }
}
