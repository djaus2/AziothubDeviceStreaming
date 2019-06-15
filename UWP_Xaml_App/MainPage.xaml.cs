using System;
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

        public MainPage()
        {
            this.InitializeComponent();
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            LoadConSettings();
            service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
            device_id = AzureConnections.MyConnections.DeviceId;
            device_cs = AzureConnections.MyConnections.DeviceConnectionString;
            ListviewTransports.ItemsSource = ListEnum;
            ListviewTransports.SelectedItem = AzIoTHubDeviceStreams.DeviceStreamingCommon.device_transportType;
            ListviewTransports.ScrollIntoView(ListviewTransports.SelectedItem);
        }

        private void ListviewTransports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListviewTransports.SelectedIndex != -1)
            {
                AzIoTHubDeviceStreams.DeviceStreamingCommon.device_transportType = (Microsoft.Azure.Devices.Client.TransportType) ListviewTransports.SelectedItem;
                System.Diagnostics.Debug.WriteLine(string.Format("Using Device Transport {0}", AzIoTHubDeviceStreams.DeviceStreamingCommon.device_transportType));
            }
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

        private bool checkboxActive = false;
        private void ChkUseCustomClassSvc_Checked(object sender, RoutedEventArgs e)
        {
            if (!checkboxActive)
            {
                CheckBox cb = (CheckBox)sender;
            if (cb != null)
            {
                checkboxActive = true;
                if (cb.IsChecked == true)
                    chkUseCustomClassDevice.IsChecked = true;
            }
            }
            else
                checkboxActive = false;
        }

        private void ChkUseCustomClassDevice_Checked(object sender, RoutedEventArgs e)
        {
            if (!checkboxActive)
            {
                CheckBox cb = (CheckBox)sender;
                if (cb != null)
                {
                    checkboxActive = true;
                    if (cb.IsChecked == true)
                        ChkUseCustomClassSvc.IsChecked = true;
                }
            }
            else
                checkboxActive = false;
        }

        private void AppBarButton_Click(object sender, RoutedEventArgs e)
        {
            conDetail = new ConDetail(AzureConnections.MyConnections.IoTHubConnectionString, AzureConnections.MyConnections.DeviceConnectionString, AzureConnections.MyConnections.DeviceId);
            Popup_SetConnectionDetails.DataContext = conDetail;
            Popup_SetConnectionDetails.IsOpen = false;
            Popup_SetConnectionDetails.IsOpen = true;
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
                    details.ConString = (string) composite["ConString"];
                    details.DevString = (string) composite["DevString"];
                    details.DevId = (string)  composite["DevId"];
                    SaveConnectionSettingsToAzureConnections(details);
                }
            }
            //else use existing.
        }

        private void SaveConnectionSettingsToAzureConnections(ConDetail ccondetail)
        {
            conDetail = ccondetail;
            AzureConnections.MyConnections.IoTHubConnectionString = ccondetail.ConString;
            AzureConnections.MyConnections.DeviceConnectionString = ccondetail.DevString;
            AzureConnections.MyConnections.DeviceId = ccondetail.DevId;
        }
        private void DoneSetConnectionDetails_Click(object sender, RoutedEventArgs e)
        {
            SaveConnectionSettingsToAzureConnections(conDetail);
            Popup_SetConnectionDetails.IsOpen = false;
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
                        if (strn[strn.Length - 1] == '\"')
                            strn = strn.Substring(0, strn.Length - 1);
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
                Popup_SetConnectionDetails.DataContext = null;
                Popup_SetConnectionDetails.DataContext = conDetail;
            }
        }
    }
}
