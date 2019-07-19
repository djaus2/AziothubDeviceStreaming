using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Threading.Tasks;
using Windows.ApplicationModel.DataTransfer;
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

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=234238

namespace UWPXamlApp
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class NewHub : Page
    {
        public string ResourceGroupName { get; set; } = "GroupName";
        public string IoTHubName { get; set; } = "HubName";
        public string location { get => location1; set { location1 = value;Update(); } }
        public string sku { get => sku1; set { sku1 = value; Update(); }  }

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
            get { return string.Format("az iot hub delete --name {0}   --resource-group {1}",IoTHubName, ResourceGroupName); }
        }

        public string DeleteGroupCode
        {
            get { return string.Format("az group delete --name {0}", ResourceGroupName); }
        }

        public string iotownerconstring {
            get { return string.Format("az iot hub show-connection-string --name {0} --policy-name iothubowner --key primary  --resource-group {1}", IoTHubName, ResourceGroupName); }
        }

        public string serviceconstring
        {
            get { return string.Format("az iot hub show-connection-string --name {0} --policy-name service --key primary  --resource-group {1}", IoTHubName, ResourceGroupName); }
        }

        public NewHub()
        {


            this.InitializeComponent();
            //if (this.Frame.CanGoBack)
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            //else
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
            NG.ValueChanged = ValueChangedGroup;
            NH.ValueChanged = ValueChangedHub;
        }

        public void Update()
        {
            Task.Run(async () =>
            {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    Login.Code = LoginCode;
                    NGCode.Code = NewGroupCode;
                    NHCode.Code = NewHubCode;
                    DelHub.Code = DeleteHubCode;
                    DelGrp.Code = DeleteGroupCode;
                    HubOwnerConString.Code = iotownerconstring;
                    HubServoceConString.Code = serviceconstring;
                    Multicom.Code = "To create a new Device connection to the Hub you need the iothubowner ConnectionString."
                    +"To run the DeviceStreaming functionality you only need the Service ConnectionString but can use the iothubowner ConnectionString. " 
                    +"To create a new Device, return and choose [ADD New IoT Hub Device ...] from the Service menu.";

                });
            });
        }

        public void ValueChangedGroup(string val)
        {
            ResourceGroupName = val;
            Update();         
        }

        public void ValueChangedHub(string val)
        {
            IoTHubName = val;
            Update();
        }

        string msg = "Hello Hub";
        private string sku1 = "F1";
        private string location1 = "centralus";

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var dataPackage = new DataPackage();
            dataPackage.SetText(msg);
            Windows.ApplicationModel.DataTransfer.Clipboard.SetContent(dataPackage);
        }


        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            Update();
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
                                sku1 = "F1";
                                break;
                            case "1":
                                sku1 = "S1";
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
                AzureConnections.MyConnections.IoTHubConnectionString = HubconString;
                AzureConnections.MyConnections.DeviceConnectionString = "";
                AzureConnections.MyConnections.DeviceId = "";
            }
        }

        private void Button_Click_2(object sender, RoutedEventArgs e)
        {
            Multicom.MultiCommentExpand(null,null);
        }
    }
}
