using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AzureConnections;
using System.Threading.Tasks;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPXamlApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class HubConnections : Page
    {

        public static class Az
        {
            public static string ResourceGroupName { get; set; }
            public static string IoTHubName { get; set; }
            public static string location { get; set; }
            public static string sku { get; set; }

            public static string LoginCode
            {
                get { return "az login"; }
            }
            public static string NewGroupCode
            {
                get { return string.Format("az group create --name {0} --location centralus", ResourceGroupName, location); }
            }

            public static string NewHubCode
            {

                get { return string.Format("az iot hub create --name {0}    --resource-group {1} --sku {2}", IoTHubName, ResourceGroupName, sku); }
            }

            public static string DeleteHubCode
            {
                get { return string.Format("az iot hub delete --name {0}   --resource-group {1}", IoTHubName, ResourceGroupName); }
            }

            public static string DeleteGroupCode
            {
                get { return string.Format("az group delete --name {0}", ResourceGroupName); }
            }

            public static string iotownerconstring
            {
                get { return string.Format("az iot hub show-connection-string --name {0} --policy-name iothubowner --key primary  --resource-group {1}", IoTHubName, ResourceGroupName); }
            }

            public static string serviceconstring
            {
                get { return string.Format("az iot hub show-connection-string --name {0} --policy-name service --key primary  --resource-group {1}", IoTHubName, ResourceGroupName); }
            }
        }
        public static class Cons{
        public static string ResourceGroupName { get { return MyConnections.AzureGroup; } set { MyConnections.AzureGroup = value; } }
        public static string IoTHubName { get { return MyConnections.IoTHubName; } set { MyConnections.IoTHubName = value; } }
        public static string DeviceId { get { return MyConnections.DeviceId; } set { MyConnections.DeviceId = value; } }

        public static string IoTHubConnectionString { get { return MyConnections.IoTHubConnectionString; } set { MyConnections.IoTHubConnectionString = value; } }

        public static string DeviceConnectionString { get { return MyConnections.DeviceConnectionString; } set { MyConnections.DeviceConnectionString = value; } }

        public static string EventHubsConnectionString { get { return MyConnections.EventHubsConnectionString; } set { MyConnections.EventHubsConnectionString = value; } }

        public static string EventHubsCompatibleEndpoint { get { return MyConnections.EventHubsCompatibleEndpoint; } set { MyConnections.EventHubsCompatibleEndpoint = value; } }

        public static string EventHubsCompatiblePath { get { return MyConnections.EventHubsCompatiblePath; } set { MyConnections.EventHubsCompatiblePath = value; } }

         public static string IotHubKeyName { get { return MyConnections.IotHubKeyName; } set { MyConnections.IotHubKeyName = value; } }

        public static string IoTHubLocation { get { return MyConnections.IoTHubLocation; } set { MyConnections.IoTHubLocation = value; } }

    }


        public HubConnections()
        {
            this.InitializeComponent();

            //NH1.ValueChanged = ValueChangedGroup1;
            //NH2.ValueChanged = ValueChangedGroup2;
            //NH3.ValueChanged = ValueChangedGroup3;
            //NH4.ValueChanged = ValueChangedGroup4;
            //NH5.ValueChanged = ValueChangedGroup5;
            //NH6.ValueChanged = ValueChangedGroup6;
            //NH7.ValueChanged = ValueChangedGroup7;
            //NH8.ValueChanged = ValueChangedGroup8;
            //NH9.ValueChanged = ValueChangedGroup9;
            //NH10.ValueChanged = ValueChangedGroup10;
            //NH11.ValueChanged = ValueChangedGroup11;
        }

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            Update();

            this.DataContext = this;
        }

        public void ValueChangedGroup1(string val)
        {
            Cons.ResourceGroupName = val;
        }
        public void ValueChangedGroup2(string val)
        {
            Cons.IoTHubName = val;
        }
        public void ValueChangedGroup3(string val)
        {
            Cons.DeviceId = val;
        }
        public void ValueChangedGroup4(string val)
        {
            Cons.IoTHubConnectionString = val;
        }
        public void ValueChangedGroup5(string val)
        {
            Cons.DeviceConnectionString = val;
        }
        public void ValueChangedGroup6(string val)
        {
            Cons.EventHubsConnectionString = val;
        }

        public void ValueChangedGroup7(string val)
        {
            Cons.EventHubsCompatibleEndpoint = val;
        }


        public void ValueChangedGroup8(string val)
        {
            Cons.EventHubsCompatiblePath = val;

        }

        public void ValueChangedGroup9(string val)
        {
            Cons.IotHubKeyName = val;
        }

        public void ValueChangedGroup10(string val)
        {
            Cons.IoTHubLocation= val;

        }

        //public void ValueChangedGroup11(string val)
        //{
        //    Cons= val;
        //}




        private void Update()
        {
            NH1.InfoValue = "Enter new or existing Azure Resource GroupName";
            NH1.Code = Cons.ResourceGroupName;
  
        }

        public void Update2()
        {
            Task.Run(async () =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    NH1.Code = Az.LoginCode;
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


    }
}
