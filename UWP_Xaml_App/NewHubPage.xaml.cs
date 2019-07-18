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

    }
}
