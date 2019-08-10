using AzureConnections;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPXamlApp
{
    public static class IoTHubConnectionDetails
    {

        public static bool EHMethod1 { get { return MyConnections.EHMethod1; } set { MyConnections.EHMethod1 = value; } }
        public static string ResourceGroupName { get { return MyConnections.AzureGroup; } set { MyConnections.AzureGroup = value; } }
        public static string IoTHubName { get { return MyConnections.IoTHubName; } set { MyConnections.IoTHubName = value; } }
        public static string DeviceId { get { return MyConnections.DeviceId; } set { MyConnections.DeviceId = value; } }

        public static string IoTHubConnectionString { get { return MyConnections.IoTHubConnectionString; } set { MyConnections.IoTHubConnectionString = value; } }

        public static string DeviceConnectionString { get { return MyConnections.DeviceConnectionString; } set { MyConnections.DeviceConnectionString = value; } }

        public static string EventHubsConnectionString { get { return MyConnections.EventHubsConnectionString; } set { MyConnections.EventHubsConnectionString = value; } }

        public static string EventHubsCompatibleEndpoint { get { return MyConnections.EventHubsCompatibleEndpoint; } set { MyConnections.EventHubsCompatibleEndpoint = value; } }

        public static string EventHubsCompatiblePath { get { return MyConnections.EventHubsCompatiblePath; } set { MyConnections.EventHubsCompatiblePath = value; } }

        public static string EventHubsSasKey { get { return MyConnections.EventHubsSasKey; } set { MyConnections.EventHubsSasKey = value; } }

        public static string IotHubKeyName { get { return MyConnections.IotHubKeyName; } set { MyConnections.IotHubKeyName = value; } }

        public static string IoTHubLocation { get { return MyConnections.IoTHubLocation; } set { MyConnections.IoTHubLocation = value; } }

        public static string SKU { get { return MyConnections.SKU; } set { MyConnections.SKU = value; } }

    }
    [Windows.UI.Xaml.Data.Bindable]
    public class Data : INotifyPropertyChanged
    {

        //public string Code { get; set; } = "MyCode";
        //public string TextInfo { get; set; } = "MyText";
        //public string InfoValue { get; set; } = "MyInfoValue";

        private string sku1 = "F1";
        private string location1 = "centralus";
        private string resourceGroupName = "GroupName";
        private string ioTHubName = "HubName";
        private string deviceId = "";
        private string deviceConnectionString = "";
        private string ioTHubConnectionString = "";

        public Data()
        {
            ResetData();
        }

        public void ResetData()
        {
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.ResourceGroupName))
                ResourceGroupName = IoTHubConnectionDetails.ResourceGroupName;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.IoTHubName))
                IoTHubName = IoTHubConnectionDetails.IoTHubName;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.DeviceId))
                DeviceId = IoTHubConnectionDetails.DeviceId;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.IoTHubConnectionString))
                IoTHubConnectionString = IoTHubConnectionDetails.IoTHubConnectionString;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.DeviceConnectionString))
                DeviceConnectionString = IoTHubConnectionDetails.DeviceConnectionString;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.EventHubsCompatiblePath))
                EventHubsCompatiblePath = IoTHubConnectionDetails.EventHubsCompatiblePath;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.EventHubsCompatibleEndpoint))
                EventHubsCompatibleEndpoint = IoTHubConnectionDetails.EventHubsCompatibleEndpoint;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.EventHubsConnectionString))
                EventHubsConnectionString = IoTHubConnectionDetails.EventHubsConnectionString;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.IotHubKeyName))
                IotHubKeyName = IoTHubConnectionDetails.IotHubKeyName;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.IotHubKeyName))
                IotHubKeyName = IoTHubConnectionDetails.IotHubKeyName;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.EventHubsSasKey))
                EventHubsSasKey = IoTHubConnectionDetails.EventHubsSasKey;
            EHMethod1 = IoTHubConnectionDetails.EHMethod1;
        }

        public void Commit()
        {
            if (!string.IsNullOrEmpty(ResourceGroupName))
                IoTHubConnectionDetails.ResourceGroupName = ResourceGroupName;
            if (!string.IsNullOrEmpty(IoTHubName))
                IoTHubConnectionDetails.IoTHubName = IoTHubName;
            if (!string.IsNullOrEmpty(DeviceId))
                IoTHubConnectionDetails.DeviceId = DeviceId;
            if (!string.IsNullOrEmpty(IoTHubConnectionString))
                IoTHubConnectionDetails.IoTHubConnectionString = IoTHubConnectionString;
            if (!string.IsNullOrEmpty(DeviceConnectionString))
                IoTHubConnectionDetails.DeviceConnectionString = DeviceConnectionString;
            if (!string.IsNullOrEmpty(EventHubsCompatiblePath))
                IoTHubConnectionDetails.EventHubsCompatiblePath = EventHubsCompatiblePath;
            if (!string.IsNullOrEmpty(EventHubsCompatibleEndpoint))
                IoTHubConnectionDetails.EventHubsCompatibleEndpoint = EventHubsCompatibleEndpoint;
            if (!string.IsNullOrEmpty(EventHubsConnectionString))
                IoTHubConnectionDetails.EventHubsConnectionString = EventHubsConnectionString;
            if (!string.IsNullOrEmpty(EventHubsSasKey))
                IoTHubConnectionDetails.EventHubsSasKey = EventHubsSasKey;
            IoTHubConnectionDetails.EHMethod1 = EHMethod1;
        }

        private bool eHMethod1 = true;
        public bool EHMethod1 { get => eHMethod1; set { if (eHMethod1 != value) { eHMethod1 = value; OnPropertyChanged(); OnPropertyChanged("EHMethod2"); } } }
        public bool EHMethod2 { get { return !eHMethod1; } }

        private string eventHubsConnectionString = "";
        private string eventHubsCompatibleEndpoint = "";
        private string eventHubsCompatiblePath = "";
        private string eventHubsSasKey = "";
  

        private string iotHubKeyName = "";

        public string IotHubKeyName { get => iotHubKeyName; set { if (iotHubKeyName != value) { iotHubKeyName = value; OnPropertyChanged(); } } }

        /* Relevant Code:
            var EventHubConnectionString = new EventHubsConnectionStringBuilder(
            new Uri(AzureConnections.MyConnections.EventHubsCompatibleEndpoint),
            AzureConnections.MyConnections.EventHubsCompatiblePath,
            AzureConnections.MyConnections.IotHubKeyName,
            AzureConnections.MyConnections.Saskey);

            OR

            EventHubConnectionString = new EventHubsConnectionStringBuilder("Endpoint=sb://ihsuproddmres016dednamespace.servicebus.windows.net/;SharedAccessKeyName=iothubowner;SharedAccessKey=onANjo3Aj7/ess/UO9dcnmBeZkCbr1WPXFz6x0HQdc0=;EntityPath=iothub-ehub-mynewhub-1918909-a3ba8a9102");

            s_eventHubClient = EventHubClient.CreateFromConnectionString(EventHubConnectionString.ToString());
         */

        //Use if Method1:
        public string EventHubsConnectionString { get => eventHubsConnectionString; set { if (eventHubsConnectionString != value) { eventHubsConnectionString = value; OnPropertyChanged(); } } }

        //Use if Method 2:
        public string EventHubsCompatibleEndpoint { get => eventHubsCompatibleEndpoint; set { if (eventHubsCompatibleEndpoint != value) { eventHubsCompatibleEndpoint = value; OnPropertyChanged(); } } }
        public string EventHubsCompatiblePath
        {
            get => eventHubsCompatiblePath; set { if (eventHubsCompatiblePath != value) { eventHubsCompatiblePath = value; OnPropertyChanged(); } }
        }
        public string EventHubsSasKey { get => eventHubsSasKey; set { if (eventHubsSasKey != value) { eventHubsSasKey = value; OnPropertyChanged(); } } }


        public string DeviceConnectionString { get => deviceConnectionString; set { if (deviceConnectionString != value) { deviceConnectionString = value; OnPropertyChanged(); } } }
        public string IoTHubConnectionString { get => ioTHubConnectionString; set { if (ioTHubConnectionString != value) { ioTHubConnectionString = value; OnPropertyChanged(); } } }
        public string ResourceGroupName { get => resourceGroupName; set { if (resourceGroupName != value) { resourceGroupName = value; OnPropertyChanged(); } } }
        public string IoTHubName { get => ioTHubName; set { if (ioTHubName != value) { ioTHubName = value; OnPropertyChanged(); } } }
        public string DeviceId { get => deviceId; set { if (deviceId != value) { deviceId = value; OnPropertyChanged(); } } }

        public string location { get => location1; set { if (location1 != value) { location1 = value; OnPropertyChanged(); } } }
        public string sku { get => sku1; set { if (sku1 != value) { sku1 = value; OnPropertyChanged(); } } }










        public string EventHubEnpointhCode
        {
            set { }
            get { return string.Format("az iot hub show --query properties.eventHubEndpoints.events.endpoint --name {0}", IoTHubName); }
        }
        public string EventHubCompatiblePathCode
        {
            set { }
            get { return string.Format("az iot hub show --query properties.eventHubEndpoints.events.path --name {0}", IoTHubName); }
        }

        public string EventHubPrimaryKeyCode
        {
            set { }
            get { return string.Format("string.Format(az iot hub policy show --name {0} --query primaryKey --hub-name {1}", IotHubKeyName, IoTHubName); }
        }

        public string LoginCode
        {
            get { return "az login"; }
        }
        public string NewGroupCode
        {
            get { return string.Format("az group create --name {0} --location centralus", ResourceGroupName, location); }
        }
        public string NewHubCode
        {

            get { return string.Format("az iot hub create --name {0}    --resource-group {1} --sku {2}", IoTHubName, ResourceGroupName, sku); }
        }
        public string DeleteHubCode
        {
            get { return string.Format("az iot hub delete --name {0}   --resource-group {1}", IoTHubName, ResourceGroupName); }
        }
        public string DeleteGroupCode
        {
            get { return string.Format("az group delete --name {0}", ResourceGroupName); }
        }
        public string iotownerconstring
        {
            get { return string.Format("az iot hub show-connection-string --name {0} --policy-name iothubowner --key primary  --resource-group {1}", IoTHubName, ResourceGroupName); }
        }
        public string serviceconstring
        {
            get { return string.Format("az iot hub show-connection-string --name {0} --policy-name service --key primary  --resource-group {1}", IoTHubName, ResourceGroupName); }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(
        [System.Runtime.CompilerServices.CallerMemberName] string propertyName = "")
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));

                switch (propertyName)
                {
                    case "ResourceGroupName":
                        handler(this, new PropertyChangedEventArgs("NewGroupCode"));
                        handler(this, new PropertyChangedEventArgs("NewHubCode"));
                        handler(this, new PropertyChangedEventArgs("DeleteGroupCode"));
                        handler(this, new PropertyChangedEventArgs("DeleteHubCode"));
                        handler(this, new PropertyChangedEventArgs("iotownerconstring"));
                        handler(this, new PropertyChangedEventArgs("serviceconstring"));
                        break;
                    case "sku":
                        handler(this, new PropertyChangedEventArgs("NewHubCode"));
                        break;
                    case "location":
                        handler(this, new PropertyChangedEventArgs("NewGroupCode"));
                        break;
                    case "IoTHubName":
                        handler(this, new PropertyChangedEventArgs("NewHubCode"));
                        handler(this, new PropertyChangedEventArgs("DeleteHubCode"));
                        handler(this, new PropertyChangedEventArgs("iotownerconstring"));
                        handler(this, new PropertyChangedEventArgs("serviceconstring"));
                        //handler(this, new PropertyChangedEventArgs("EventHubEnpointhCode"));
                        //handler(this, new PropertyChangedEventArgs("EventHubCompatiblePathCode"));
                        //handler(this, new PropertyChangedEventArgs("EventHubPrimaryKeyCode"));
                        break;
                    case "DeviceId":
                        handler(this, new PropertyChangedEventArgs(propertyName));
                        handler(this, new PropertyChangedEventArgs("iotownerconstring"));
                        handler(this, new PropertyChangedEventArgs("serviceconstring"));
                        break;
                }
            }
            //if (!string.IsNullOrEmpty(Property))
            //    ValueChanged?.Invoke(Property);
        }
    }



    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewHub : Page
    {
        public void CreateNewEntity(string g)
        {
            System.Diagnostics.Debug.WriteLine("CreateNewEntity " + g);
            switch (g)
            {
                case "Device":
                    AzureConnections.MyConnections.AddDeviceAsync(Data1.IoTHubConnectionString, Data1.DeviceId);
                    Data1.DeviceConnectionString = IoTHubConnectionDetails.DeviceConnectionString;
                    break;
            }
        }

        private void MainPage_BackRequested(object sender, BackRequestedEventArgs e)
        {
            Data1.Commit();
        }

        public void DeleteEntity(string g)
        {
            System.Diagnostics.Debug.WriteLine("DeleteEntity " + g);
            switch (g)
            {
                case "Device":
                    AzureConnections.MyConnections.RemoveDeviceAsync(Data1.IoTHubConnectionString, Data1.DeviceId);
                    Data1.DeviceConnectionString = "";
                    break;
            }
        }

        public void GenerateEntityInfo(string g)
        {
            System.Diagnostics.Debug.WriteLine("GenerateEntityInfo " + g);
            switch (g)
            {
                case "Hub":
                    GetHubNameFromHubCS();
                    break;
                case "Device":
                    GetDeviceIdFromDeviceCS();
                    break;
                case "CSHub":
                    //GetHubNameFromHubCS();
                    break;
                case "CSDevice":
                    //GetHubNameFromHubCS();
                    break;
            }
        }

        public Data Data1 = new Data();

        







        public NewHub()
        {
            //NGCode.DataContext = Data1.NewGroupCode;
            //this.DataContext = Data1;

            this.InitializeComponent();
            //if (this.Frame.CanGoBack)
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            //else
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            //NG.ValueChanged = ValueChangedGroup;
            //NH.ValueChanged = ValueChangedHub;

            SystemNavigationManager.GetForCurrentView().BackRequested += MainPage_BackRequested;

        }

        public void Update()
        {
            Task.Run(async () =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    //if (!string.IsNullOrEmpty(Cons.ResourceGroupName))
                    //    NG.Text = Cons.ResourceGroupName;
                    //if (!string.IsNullOrEmpty(Cons.IoTHubName))
                    //    NH.Text = Cons.IoTHubName;
                    //if (!string.IsNullOrEmpty(Cons.DeviceId))
                    //    ND.Text = Cons.DeviceId;
                    //if (!string.IsNullOrEmpty(Cons.IoTHubConnectionString))
                    //    NCS1.Text = Cons.IoTHubConnectionString;
                    //if (!string.IsNullOrEmpty(Cons.DeviceConnectionString))
                    //    NCS2.Text = Cons.DeviceConnectionString;
                    //ResourceGroupName = NG.Text;
                    //IoTHubName = NH.Text;

                    //Login.Code = LoginCode;
                    //NGCode.Code = NewGroupCode;
                    //NHCode.Code = NewHubCode;
                    //DelHub.Code = DeleteHubCode;
                    //DelGrp.Code = DeleteGroupCode;
                    //HubOwnerConString.Code = iotownerconstring;
                    //HubServoceConString.Code = serviceconstring;
                    //Multicom.Code = "To create a new Device connection to the Hub you need the iothubowner ConnectionString."
                    //+"To run the DeviceStreaming functionality you only need the Service ConnectionString but can use the iothubowner ConnectionString. " 
                    //+"To create a new Device, return and choose [ADD New IoT Hub Device ...] from the Service menu.";

                });
            });
        }

        //public void ValueChangedGroup(string val)
        //{
        //    ResourceGroupName = val;
        //    Update();         
        //}

        //public void ValueChangedHub(string val)
        //{
        //    IoTHubName = val;
        //    Update();
        //}

        string msg = "Hello Hub";


        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(msg);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            h1.SubRegion = this.GettingStarted;
            h2.SubRegion = this.Info;
            h2.IsExpanded = true;
            h3.SubRegion = this.ConnectingAndGroup;
            h4.SubRegion = this.NewHubRegion;
            h5.SubRegion = this.NewHDevice;
            h6.SubRegion = this.Cleanup;
            h7.SubRegion = this.Misc;
            h2ev.SubRegion = this.EventHubInfo;
            EventHubMethod1Heading.SubRegion = this.EventHubMethod1;
            EventHubMethod2Heading.SubRegion = this.EventHubMethod2;
            //if (!string.IsNullOrEmpty(Cons.IoTHubConnectionString))
            //    NCS1.TextInfo = Cons.IoTHubConnectionString;
            //if (!string.IsNullOrEmpty(Cons.DeviceConnectionString))
            //    NCS2.TextInfo = Cons.DeviceConnectionString;
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.SKU))
            {
                if (IoTHubConnectionDetails.SKU == "F1")
                {
                    rbF1.IsChecked = true;
                }
                else
                    rbS1.IsChecked = true;
            }
            else
                rbS1.IsChecked = true;
            //if (!string.IsNullOrEmpty(Cons.ResourceGroupName))
            //    NG.Text = Cons.ResourceGroupName;
            //if (!string.IsNullOrEmpty(Cons.IoTHubName))
            //    NH.Text = Cons.IoTHubName;
            //if (!string.IsNullOrEmpty(Cons.DeviceId))
            //    ND.Text = Cons.DeviceId;
            //if (!string.IsNullOrEmpty(Cons.IoTHubConnectionString))
            //    NCS1.Text = Cons.IoTHubConnectionString;
            //if (!string.IsNullOrEmpty(Cons.DeviceConnectionString))
            //    NCS2.Text = Cons.DeviceConnectionString;
            //if (!string.IsNullOrEmpty(Cons.SKU))
            //{
            //    if (Cons.SKU == "F1")
            //    {
            //        rbF1.IsChecked = true;
            //    }
            //    else
            //        rbS1.IsChecked = true;
            //}
            //else
            //    rbS1.IsChecked = true;


            //Update();
            //NewHubElement.ValueChanged = ValueChangedD;

            NH.GenerateEntityInfo = GenerateEntityInfo;
            ND.CreateNewEntity = CreateNewEntity;
            ND.DeleteEntity = DeleteEntity;
            ND.GenerateEntityInfo = GenerateEntityInfo;
            NCS1.GenerateEntityInfo = GenerateEntityInfo;
            NCS2.GenerateEntityInfo = GenerateEntityInfo;
        }

        public void ValueChangedD(string propertyName)
        {
            return;
            //System.Diagnostics.Debug.WriteLine(propertyName);
            //switch (propertyName.ToLower())
            //{
            //    case "group":
            //        ResourceGroupName = NG.Text;
            //        NGCode.Code = NewGroupCode;
            //        DelGrp.Code = DeleteGroupCode;
            //        break;
            //    case "hub":
            //        IoTHubName = NH.Text;
            //        NHCode.Code = NewHubCode;
            //        DelHub.Code = DeleteHubCode;
            //        HubOwnerConString.Code = iotownerconstring;
            //        HubServoceConString.Code = serviceconstring;
            //        break;
            //    case "device":
            //        DeviceId = ND.Text;
            //        break;
            //}
        }

        private void RbF1_Checked(object sender, RoutedEventArgs e)
        {
            if (sender is Control)
            {
                if (((Control)sender).Tag != null)
                {
                    string tag = (string)((Control)sender).Tag;
                    if (!string.IsNullOrEmpty(tag))
                    {
                        switch (tag)
                        {
                            case "0":
                                Data1.sku = "F1";
                                break;
                            case "1":
                                Data1.sku = "S1";
                                break;
                        }
                        Update();
                    }
                }
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
   
            var cb = Windows.ApplicationModel.DataTransfer.Clipboard.GetContent();
            string HubconString = await cb.GetTextAsync();
            if (!string.IsNullOrEmpty(HubconString))
            {
                Data1.IoTHubName = HubconString;
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {

        }

        private void Button_Click_3(object sender, RoutedEventArgs e)
        {
            AzureConnections.MyConnections.AddDeviceAsync(Data1.IoTHubConnectionString, Data1.DeviceId);
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.DeviceConnectionString))
            {
                Data1.DeviceConnectionString = IoTHubConnectionDetails.DeviceConnectionString;
            }
        }




        private void Button_Click_6(object sender, RoutedEventArgs e)
        {
            AzureConnections.MyConnections.GetDeviceCSAsync(Data1.IoTHubConnectionString, Data1.DeviceId);
            if (!string.IsNullOrEmpty(IoTHubConnectionDetails.DeviceConnectionString))
            {
                Data1.DeviceConnectionString = IoTHubConnectionDetails.DeviceConnectionString;
            }
        }

        private async void GetHubNameFromHubCS()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (string.IsNullOrEmpty(Data1.IoTHubConnectionString))
                    return;
                string cshub = Data1.IoTHubConnectionString;
                string[] parts = cshub.Split(new char[] { '=' });
                if (parts.Length > 1)
                {
                    if (!string.IsNullOrEmpty(parts[1]))
                    {
                        parts = parts[1].Split(new char[] { '.' });
                        if (parts.Length > 0)
                        {
                            if (!string.IsNullOrEmpty(parts[0]))
                            {
                                Data1.IoTHubName = parts[0];
                            }
                        }
                    }
                }
            });
        }

        private async void GetDeviceIdFromDeviceCS()
        {
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                if (string.IsNullOrEmpty(Data1.DeviceConnectionString))
                    return;
                string cshub = Data1.DeviceConnectionString;
                string[] parts = cshub.Split(new char[] { ';' });
                if (parts.Length > 1)
                {
                    if (!string.IsNullOrEmpty(parts[1]))
                    {
                        parts = parts[1].Split(new char[] { '=' });
                        if (parts.Length > 1)
                        {
                            if (!string.IsNullOrEmpty(parts[1]))
                            {
                                Data1.DeviceId = parts[1];
                            }
                        }
                    }
                }
            });
        }

        private void Button_Click_5(object sender, RoutedEventArgs e)
        {
            Data1.ResetData();
        }

        private void Button_Click_7(object sender, RoutedEventArgs e)
        {
            Data1.Commit();
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // //var pageheight =  e.NewSize.Height;

            // //PageBody.Height = pageheight - PageHeading.Height;
            GeneralTransform gt = PageBody.TransformToVisual(this);
            Point offset = gt.TransformPoint(new Point(0, 0));
            double controlTop = offset.Y;
            double controlLeft = offset.X;
            double newHeight = e.NewSize.Height - controlTop -PageHeading.Height;
            //if (1=1)
            //{
            PageBody.Height = newHeight;

            // }
        }

        private void Button_Click_4(object sender, RoutedEventArgs e)
        {

        }

        private void RbEHMethod1_Checked(object sender, RoutedEventArgs e)
        {

        }
    }
}
