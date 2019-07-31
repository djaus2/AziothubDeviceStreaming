﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AzIoTHubDeviceStreams;
using System.Threading.Tasks;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPXamlApp
{

    public sealed partial class MainPage : Page
    {
        string service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
        string device_id = AzureConnections.MyConnections.DeviceId;
        string device_cs = AzureConnections.MyConnections.DeviceConnectionString;


        public  List<Microsoft.Azure.Devices.Client.TransportType> ListEnum { get { return typeof(Microsoft.Azure.Devices.Client.TransportType).GetEnumValues().Cast<Microsoft.Azure.Devices.Client.TransportType>().ToList(); } }
        public List<string> ListEnum2 = new List<string> { "Echo", "Uppercase", "Sim Environ", "Sim Telemetry" };

        public MainPage()
        {
            this.InitializeComponent();
            IsFirstTime = true;
            this.NavigationCacheMode = NavigationCacheMode.Enabled;
        }


        private bool IsFirstTime = false;
        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            if (IsFirstTime)
                LoadConSettings();
            else
            {
                //Last two items are cleared because new Hub has no devices.
                if (""== AzureConnections.MyConnections.DeviceId+ AzureConnections.MyConnections.DeviceConnectionString)
                    SaveSettingsToAppData();
            }

            service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
            device_id = AzureConnections.MyConnections.DeviceId;
            device_cs = AzureConnections.MyConnections.DeviceConnectionString;


            if (IsFirstTime)
            {
                ListviewTransports2.ItemsSource = ListEnum;
                ListviewTransports2.SelectedItem = AzIoTHubDeviceStreams.DeviceStreamingCommon.device_transportType;
                ListviewTransports2.ScrollIntoView(ListviewTransports2.SelectedItem);
                AzureConnections.MyConnections.OnStatusUpdateD = OnDeviceSvcUpdate;
                LstDeviceAction.ItemsSource = ListEnum2;
                LstDeviceAction.SelectedItem = ListEnum2[1];
                LstDeviceAction.ScrollIntoView(ListEnum2[1]);
                if (autoStartDevice)
                {
                    Button_Click_Device(null, null);
                }
            }
            IsFirstTime = false;
        }



        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GeneralTransform gt = tbSvcMsgOut.TransformToVisual(this);
            Point offset = gt.TransformPoint(new Point(0, 0));
            double controlTop = offset.Y;
            double controlLeft = offset.X;
            double newWidth =  e.NewSize.Width - controlLeft - 20;
            if (newWidth > tbSvcMsgOut.MinWidth)
            {
                tbSvcMsgOut.Width = newWidth;
                tbDeviceMsgIn.Width = newWidth;
                tbSvcMsgIn.Width = newWidth;
                tbDeiceMsgOut.Width = newWidth;
            }
        }


        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            Control cntrl = (Control)sender;
            if ("1" == (string)cntrl.Tag)
            {
                conDetail = new ConDetail(AzureConnections.MyConnections.IoTHubConnectionString, AzureConnections.MyConnections.DeviceConnectionString, AzureConnections.MyConnections.DeviceId);
                Popup_SetConnectionDetails.DataContext = conDetail;
                Popup_SetConnectionDetails.IsOpen = false;
                Popup_SetConnectionDetails.IsOpen = true;
            }
            else if ("2" == (string)cntrl.Tag)
            {
                conDetail = new ConDetail(AzureConnections.MyConnections.IoTHubConnectionString, AzureConnections.MyConnections.DeviceConnectionString, AzureConnections.MyConnections.DeviceId);
                Popup_GetConnectionDetails.DataContext = conDetail;
                Popup_GetConnectionDetails.IsOpen = false;
                Popup_GetConnectionDetails.IsOpen = true;
            }
            else if ("3" == (string)cntrl.Tag)
            {
                conDetail = new ConDetail(AzureConnections.MyConnections.IoTHubConnectionString, AzureConnections.MyConnections.DeviceConnectionString, AzureConnections.MyConnections.DeviceId);
                Popup_CreateDeviceDetails.DataContext = conDetail;
                Popup_CreateDeviceDetails.IsOpen = false;
                Popup_CreateDeviceDetails.IsOpen = true;
            }
            else if ("4" == (string)cntrl.Tag)
            {
                conDetail = new ConDetail(AzureConnections.MyConnections.IoTHubConnectionString, AzureConnections.MyConnections.DeviceConnectionString, AzureConnections.MyConnections.DeviceId);
                Popup_Delete.DataContext = conDetail;
                Popup_Delete.IsOpen = false;
                Popup_Delete.IsOpen = true;
            }//Popup_NewIoTHub
            else if ("5" == (string)cntrl.Tag)
            {
                this.Frame.Navigate(typeof(NewHub), null);
            }

        }

        public class ConDetail
        {
            public string ConString { get; set; }
            public string DevString { get; set; }
            public string DevId { get; set; }
            public ConDetail(string a, string b, string c)
            {
                ConString = a;
                DevString = b;
                DevId = c;
            }

            public ConDetail()
            {
            }

        }

        private ConDetail conDetail =null;

        private void LoadConSettings()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.Keys.Contains("ConDetail"))
            {
                Windows.Storage.ApplicationDataCompositeValue composite =
   (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["ConDetail"];
                if (composite != null)
                {
                    ConDetail details = new ConDetail();
                    details.ConString = (string)composite["ConString"];
                    details.DevString = (string)composite["DevString"];
                    details.DevId = (string)composite["DevId"];
                    SaveConnectionSettingsToAzureConnections(details);
                }
            }
            //else use existing.
            if (localSettings.Values.Keys.Contains("AutoStartDevice"))
            {
                chkAutoStart.IsChecked = (bool) localSettings.Values["AutoStartDevice"];
            }
            if (localSettings.Values.Keys.Contains("KeepDeviceListening"))
            {
                chKeepDeviceListening.IsChecked = (bool)localSettings.Values["KeepDeviceListening"];
            }

            if (localSettings.Values.Keys.Contains("DeviceTimeout"))
            {
                if (localSettings.Values["DeviceTimeout"] is double)
                {
                    double _deviceTimeout = (double)localSettings.Values["DeviceTimeout"];
                    tbDeviceTimeout.Text = _deviceTimeout.ToString();
                }
                else
                    tbDeviceTimeout.Text = DeviceStreamingCommon.DeviceTimeoutDef.ToString();
            }
            else
                tbDeviceTimeout.Text = DeviceStreamingCommon.DeviceTimeoutDef.ToString();
            if (localSettings.Values.Keys.Contains("SvcTimeout"))
            {
                if (localSettings.Values["SvcTimeout"] is double)
                {
                    double _svcTimeout = (double)localSettings.Values["SvcTimeout"];
                    tbSvcTimeout.Text = _svcTimeout.ToString();
                }
                else
                    tbSvcTimeout.Text = DeviceStreamingCommon.SvcTimeoutDef.ToString();
            }
            else
                tbSvcTimeout.Text = DeviceStreamingCommon.SvcTimeoutDef.ToString();
        }

        private void SaveConnectionSettingsToAzureConnections(ConDetail ccondetail)
        {
            conDetail = ccondetail;
            AzureConnections.MyConnections.IoTHubConnectionString = ccondetail.ConString;
            AzureConnections.MyConnections.DeviceConnectionString = ccondetail.DevString;
            AzureConnections.MyConnections.DeviceId = ccondetail.DevId;
            service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
            device_id = AzureConnections.MyConnections.DeviceId;
            device_cs = AzureConnections.MyConnections.DeviceConnectionString;
        }
        private void DoneSetConnectionDetails_Click(object sender, RoutedEventArgs e)
        {
            if (Popup_SetConnectionDetails.IsOpen)
            {
                SaveConnectionSettingsToAzureConnections(conDetail);
                Popup_SetConnectionDetails.IsOpen = false;
            }
            else if (Popup_GetConnectionDetails.IsOpen)
            {
                conDetail.DevString = AzureConnections.MyConnections.GetDeviceAsync(conDetail.ConString, conDetail.DevId);
                Popup_GetConnectionDetails.IsOpen = false;
            }
            else if (Popup_CreateDeviceDetails.IsOpen)
            {
                conDetail.DevString = AzureConnections.MyConnections.AddDeviceAsync(conDetail.ConString, conDetail.DevId);
                Popup_CreateDeviceDetails.IsOpen = false;
            }
            else if (Popup_Delete.IsOpen)
            {
                conDetail = new ConDetail(AzureConnections.MyConnections.IoTHubConnectionString, AzureConnections.MyConnections.DeviceConnectionString, AzureConnections.MyConnections.DeviceId);
                conDetail.DevString = AzureConnections.MyConnections.RemoveDeviceAsync(conDetail.ConString, conDetail.DevId);
                Popup_Delete.IsOpen = false;
                conDetail.DevId = "";
                conDetail.DevString = "";
            }
            SaveSettingsToAppData();
        }

        private void SaveSettingsToAppData()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.Keys.Contains("ConDetail"))
            {
                localSettings.Values.Remove("ConDetail");
            }
            Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();
            composite["ConString"] = conDetail.ConString;
            composite["DevString"] = conDetail.DevString;
            composite["DevId"] = conDetail.DevId;
            localSettings.Values.Add("ConDetail", composite);
        }

        private void CancelSetConnectionDetails_Click(object sender, RoutedEventArgs e)
        {
            Popup_SetConnectionDetails.IsOpen = false;
            Popup_GetConnectionDetails.IsOpen = false;
            Popup_CreateDeviceDetails.IsOpen = false;
            Popup_Delete.IsOpen = false;
        }

 

        private async void Button_RemoveDBLQuotes_Click(object sender, RoutedEventArgs e)
        {
            Button butt = (Button)sender;
            if (butt != null)
            { 
                var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
                if (dataPackageView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Text))
                {
                    string strn = await dataPackageView.GetTextAsync();
                    if (!string.IsNullOrEmpty(strn))
                    {
                        if (strn[0] == '\"')
                            strn = strn.Substring(1);
                        if (strn != "")
                        {
                            if (strn[strn.Length - 1] == ';')
                                strn = strn.Substring(0, strn.Length - 1);
                            if (strn != "")
                            {
                                if (strn[strn.Length - 1] == '\"')
                                    strn = strn.Substring(0, strn.Length - 1);
                            }
                        }
                    }
                    string tag = (string)butt.Tag;
                    switch (tag)
                    {
                        case "0":
                            conDetail.ConString = strn;
                            break;
                        case "1":
                            conDetail.DevString = strn;
                            break;
                        case "2":
                            conDetail.DevId = strn;
                            break;
                    }
                   
                }
                if (Popup_SetConnectionDetails.IsOpen)
                {
                    Popup_SetConnectionDetails.DataContext = null;
                    Popup_SetConnectionDetails.DataContext = conDetail;
                }
                else if (Popup_GetConnectionDetails.IsOpen)
                {
                    Popup_GetConnectionDetails.DataContext = null;
                    Popup_GetConnectionDetails.DataContext = conDetail;
                }
                else if (Popup_CreateDeviceDetails.IsOpen)
                {
                    Popup_CreateDeviceDetails.DataContext = null;
                    Popup_CreateDeviceDetails.DataContext = conDetail;
                }
            }
        }

        TimeSpan DeviceTimeout { get; set; } = TimeSpan.FromMilliseconds(10000);
        private void TbDeviceTimeout_TextChanged(object sender, TextChangedEventArgs e)
        {
            double timeout;
            if (double.TryParse(tbDeviceTimeout.Text, out timeout))
            {
                
                DeviceTimeout = TimeSpan.FromMilliseconds( timeout);
                DeviceStreamingCommon.DeviceTimeout = DeviceTimeout;
                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values.Keys.Contains("DeviceTimeout"))
                {
                    if (localSettings.Values["DeviceTimeout"] is double)
                        localSettings.Values["DeviceTimeout"] = timeout;
                    else
                        localSettings.Values.Remove("DeviceTimeout");
                }
                if (!localSettings.Values.Keys.Contains("DeviceTimeout"))
                    localSettings.Values.Add("DeviceTimeout", timeout);
            }
        }

        TimeSpan SvcTimeout { get; set; } = TimeSpan.FromMilliseconds(10000);
        private void TbSvcTimeout_TextChanged(object sender, TextChangedEventArgs e)
        {
            double timeout;
            if (double.TryParse(tbSvcTimeout.Text, out timeout))
            {
               SvcTimeout = TimeSpan.FromMilliseconds(timeout);
                DeviceStreamingCommon.SvcTimeout = SvcTimeout;
                Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
                if (localSettings.Values.Keys.Contains("SvcTimeout"))
                {
                    if (localSettings.Values["SvcTimeout"] is double)
                        localSettings.Values["SvcTimeout"] = timeout;
                    else
                        localSettings.Values.Remove("SvcTimeout");
                }
                if (!localSettings.Values.Keys.Contains("SvcTimeout"))
                    localSettings.Values.Add("SvcTimeout", timeout);
            }
        }

        private async void PasteAllConnectionDetails_Click(object sender, RoutedEventArgs e)
        {
            var dataPackageView = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            if (dataPackageView.Contains(Windows.ApplicationModel.DataTransfer.StandardDataFormats.Text))
            {
                string strn = await dataPackageView.GetTextAsync();
                string[] lines = strn.Split(new char[] { '\r', '\n' });
                conDetail = new ConDetail();
                foreach (string _line in lines)
                {
                    var line =_line.Trim();
                    if (!string.IsNullOrEmpty(line))
                    {
                        string[] parts = line.Split(new char[] { '=' },2,StringSplitOptions.RemoveEmptyEntries);
                        if (parts.Length == 2)
                        {
                            string propName = parts[0].Trim();
                            string propValue = parts[1].Trim();
                            if (propValue == null)
                                propValue = "";
                            if (!string.IsNullOrEmpty(propName))
                            {
                                if (!string.IsNullOrEmpty(propValue))
                                {
                                    if (propValue[0] == '\"')
                                        propValue = propValue.Substring(1);
                                    if (propValue != "")
                                    {
                                        if (propValue[propValue.Length - 1] == ';')
                                            propValue = propValue.Substring(0, propValue.Length - 1);
                                        if (propValue != "")
                                        {
                                            if (propValue[propValue.Length - 1] == '\"')
                                                propValue = propValue.Substring(0, propValue.Length - 1);
                                        }
                                    }
                                    
                                    switch (propName.ToLower())
                                    {
                                        case "iothubconnectionstring":
                                            conDetail.ConString = propValue;
                                            break;
                                        case "deviceconnectionstring":
                                            conDetail.DevString = propValue;
                                            break;
                                        case "deviceid":
                                            conDetail.DevId = propValue;
                                            break;
                                    }
                                    Popup_SetConnectionDetails.DataContext = null;
                                    Popup_SetConnectionDetails.DataContext = conDetail;
                                }
                            }
                        }
                    }
                }
            }
        }


        private void ClearSetConnectionDetails_Click(object sender, RoutedEventArgs e)
        {
           
            if (Popup_SetConnectionDetails.IsOpen)
            {
                tbSvcConString.Text = ""; //This only needs to be Service Connection String
                tbDeviceConString.Text = "";
                tbDeviceId.Text = "";
            }
            else if (Popup_GetConnectionDetails.IsOpen)
            {
                tbIoTHubOwnerConString.Text = "";  //Needs to be Owner Connection string
                tbDeviceId2.Text = "";
            }
            else if (Popup_CreateDeviceDetails.IsOpen)
            {
                tbIoTHubOwnerConString3.Text = ""; //Needs to be Owner Connection string.
                tbDeviceId3.Text = "";
            }
        }

        private void DeviceProcessingModeCommands_Opening(object sender, object e)
        {

        }

        bool IsRunningTelem = false;
        private void BtnTelemDevice_Click(object sender, RoutedEventArgs e)
        {
            if(IsRunningTelem)
            {
                IsRunningTelem = false;
                SimulatedDevice_ns.SimulatedDevice.ContinueLoop = false;
                return;
            }
            IsRunningTelem = true;
            SimulatedDevice_ns.SimulatedDevice.Configure(AzureConnections.MyConnections.DeviceConnectionString, false, AzIoTHubDeviceStreams.DeviceStreamingCommon.device_transportType, true, TelemMsg);
            string msg = SimulatedDevice_ns.SimulatedDevice.Run().GetAwaiter().GetResult();
        }

        private void TelemMsg(string msg)
        {
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tbDeiceMsgOut.Text = msg;
                });
            });
        }

        private void BtnTelemSvc_Click(object sender, RoutedEventArgs e)
        {

        }

        private async void BtnTelemSvc_Click_1(object sender, RoutedEventArgs e)
        {
            await read_d2c_messages.ReadDeviceToCloudMessages.Run(AzureConnections.MyConnections.IoTHubConnectionString, null);
        }

        private void AppBarButton_Click_1(object sender, RoutedEventArgs e)
        {
            Environment.SetEnvironmentVariable("IOTHUB_DEVICE_CONN_STRING", AzureConnections.MyConnections.IoTHubConnectionString, EnvironmentVariableTarget.User);
            var xxx = Environment.GetEnvironmentVariable("IOTHUB_DEVICE_CONN_STRING");

            Environment.SetEnvironmentVariable("DEVICE_ID", AzureConnections.MyConnections.DeviceId);
            Environment.SetEnvironmentVariable("IOTHUB_CONN_STRING_CSHARP", AzureConnections.MyConnections.IoTHubConnectionString);
        }
    }
}
