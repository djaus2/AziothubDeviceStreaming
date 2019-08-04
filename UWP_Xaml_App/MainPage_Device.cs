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

        private string OnDeviceRecvTextIO(string msgIn, out Microsoft.Azure.Devices.Client.Message message )
        {
            message = null;
            //Perform device side processing here. Eg read sensors.
            string msgOut = msgIn;
            switch (DeviceAction)
            {
                case 0:
                    msgOut = msgIn;
                    break;
                case 1:
                    msgOut = msgIn.ToUpper();
                    break;
                case 2:
                    switch (msgIn.Substring(0,3).ToLower())
                    {
                        case "tem":
                            msgOut = "45 C";
                            break;
                        case "pre":
                            msgOut = "1034.0 hPa";
                            break;
                        case "hum":
                            msgOut = "67%";
                            break;
                        default:
                            msgOut = "Invalid request";
                            break;
                    }
                    break;
                case 3:
                    msgOut  = AzIoTHubDeviceStreams.DeviceStreamingCommon.DeiceInSimuatedDeviceModeStrn + SimulatedDevice_ns.SimulatedDevice.Run().GetAwaiter().GetResult();
                    message = SimulatedDevice_ns.SimulatedDevice.Message;
                    break;
                case 4:
                    msgOut = "Coming. Not yet implemented. This is a pace holder for now.";
                    break;
            }
           

            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tbDeviceMsgIn.Text = msgIn;
                    tbDeviceMsgOut.Text = msgOut;
                });
            });
            return msgOut;
        }

        private void OnDeviceStatusUpdate(string msgIn)
        {
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tbDevMode.Text = ListEnum2[AzureConnections.MyConnections.DeviceAction];
                    tbDeviceStatus.Text = msgIn;
                });
            });
        }

        private void ActionCommand(bool isChecked,string val, int value,  int cmd )
        {
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    switch (cmd)
                    {
                        case 0:
                            if (chkAutoStart.IsChecked != isChecked)
                                chkAutoStart.IsChecked = isChecked;
                            break;
                        case 1:
                            if (chKeepDeviceListening.IsChecked != isChecked)
                                chKeepDeviceListening.IsChecked = isChecked;
                            break;
                    }
                    
                });
            });
        }


        private void ButtonCanceLDevice_Click(object sender, RoutedEventArgs e)
        {
            DeviceStream_Device.deviceStream_Device?.Cancel();
        }

        private async void Button_Click_Device(object sender, RoutedEventArgs e)
        {

            bool deviceBasicMode = (rbDeviceModeBasic.IsChecked == true);
            bool deviceUseCustomClass = (rbDeviceModeExp.IsChecked == true);           
            await Task.Run(() =>
            {
                try
                {
                    if (deviceBasicMode)
                        DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO).GetAwaiter().GetResult();
                    if (!deviceUseCustomClass)
                        DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO, OnDeviceStatusUpdate, ActionCommand, KeepDeviceListening ).GetAwaiter().GetResult();
                    else
                        DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO, OnDeviceStatusUpdate, ActionCommand, KeepDeviceListening , new DeviceSvcCurrentSettings_Example()).GetAwaiter().GetResult();
                }
                catch (TaskCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Task cancelled");
                }
                catch (OperationCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Operation cancelled");
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): " + ex.Message);
                }
            });
        }

        bool keepDeviceListening = false;
        public bool KeepDeviceListening
        {
            get
            {
                return keepDeviceListening;
            }
            set
            {
                keepDeviceListening = value;
                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values.Keys.Contains("KeepDeviceListening"))
                {
                    if (localSettings.Values["KeepDeviceListening"] is bool)
                        localSettings.Values["KeepDeviceListening"] = keepDeviceListening;
                    else
                        localSettings.Values.Remove("KeepDeviceListening");
                }
                if (!localSettings.Values.Keys.Contains("KeepDeviceListening"))
                    localSettings.Values.Add("KeepDeviceListening", keepDeviceListening);
            }
        }
        private void ChKeepDeviceListening_Checked(object sender, RoutedEventArgs e)
        {
            KeepDeviceListening = (bool)((CheckBox)sender)?.IsChecked;
        }


        bool autoStartDevice = false;
        bool AutoStartDevice
        {
            get { return autoStartDevice; }
            set {
                autoStartDevice = value;
                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values.Keys.Contains("AutoStartDevice"))
                {
                    if (localSettings.Values["AutoStartDevice"] is bool)
                        localSettings.Values["AutoStartDevice"] = value;
                    else
                        localSettings.Values.Remove("AutoStartDevice");
                }
                if (!localSettings.Values.Keys.Contains("AutoStartDevice"))
                    localSettings.Values.Add("AutoStartDevice",value);
            }
        }
        private void ChAutoStart_Checked(object sender, RoutedEventArgs e)
        {
            AutoStartDevice = (bool)((CheckBox)sender)?.IsChecked;
            if (AutoStartDevice)
                Task.Run(async () => {
                    await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                    {
                        chKeepDeviceListening.IsChecked = true;
                    });
                });           
        }

        
        private void ListviewTransports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListviewTransports2.SelectedIndex != -1)
            {
                AzIoTHubDeviceStreams.DeviceStreamingCommon.device_transportType = (Microsoft.Azure.Devices.Client.TransportType)ListviewTransports2.SelectedItem;
                System.Diagnostics.Debug.WriteLine(string.Format("Device Transport set to: {0}", AzIoTHubDeviceStreams.DeviceStreamingCommon.device_transportType));
                DeviceProcessingModeCommands.IsOpen = false;
                OnDeviceStatusUpdate(string.Format("Device Transport set to: {0}", AzIoTHubDeviceStreams.DeviceStreamingCommon.device_transportType));
            }
        }

        private int DeviceAction = 2;
        private void DeviceAction_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (LstDeviceAction.SelectedIndex != -1)
            {
                AzureConnections.MyConnections.DeviceAction = LstDeviceAction.SelectedIndex;
                DeviceAction = AzureConnections.MyConnections.DeviceAction;
                DeviceProcessingModeCommands.IsOpen = false;
                tbDevMode.Text = ListEnum2[AzureConnections.MyConnections.DeviceAction];
                OnDeviceStatusUpdate(string.Format("Device Processing set to: {0}", ListEnum2[DeviceAction]));
                if(ListEnum2[DeviceAction] == "Sim Telemetry")
                    SimulatedDevice_ns.SimulatedDevice.Configure(AzureConnections.MyConnections.DeviceConnectionString, true, AzIoTHubDeviceStreams.DeviceStreamingCommon.device_transportType, false);
            }
        }


    }
}
