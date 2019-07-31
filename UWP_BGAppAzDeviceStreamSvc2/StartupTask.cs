using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using AzIoTHubDeviceStreams;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace BGAppAzDeviceStreamSvc2
{

    public sealed class StartupTask : IBackgroundTask
    {
        private static int DeviceAction = AzureConnections.MyConnections.DeviceAction;

        private static bool basicMode = AzureConnections.MyConnections.basicMode;
        private static bool UseCustomClass = AzureConnections.MyConnections.UseCustomClass;
        private static bool ResponseExpected = AzureConnections.MyConnections.ResponseExpected;
        private static bool KeepAlive = AzureConnections.MyConnections.KeepAlive;

        private static string service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
        private static string device_id = AzureConnections.MyConnections.DeviceId;
        private static string device_cs = AzureConnections.MyConnections.DeviceConnectionString;

        private static int DevKeepListening = 2; //No action
        private static int DevAutoStart = 2; //No action
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            System.Diagnostics.Debug.WriteLine("Svc: Starting.\n");

            RunSvc(service_cs, device_id, "Hello Word", 100000);

            System.Diagnostics.Debug.WriteLine("Svc Done.\n\nPress any key to finish.\n");
            //Console.ReadKey();
        }

        private  void OnSvcRecvText(string msg)
        {
            System.Diagnostics.Debug.WriteLine(msg);
        }

        private static void OnDeviceSvcUpdate(string recvTxt)
        {
            if (!string.IsNullOrEmpty(recvTxt))
                System.Diagnostics.Debug.WriteLine("Update: " + recvTxt);
        }

        private static void ActionCommand(bool flag, string msg, int al, int cmd)
        {
            switch (cmd)
            {
                case 0:
                    //if (chkAutoStart.IsChecked != isChecked)
                    //    chkAutoStart.IsChecked = isChecked;
                    break;
                case 1:
                    //if (chKeepDeviceListening.IsChecked != isChecked)
                    //    chKeepDeviceListening.IsChecked = isChecked;
                    break;
            }
        }

        private  void RunSvc(string servvicecs, string devid, string msgOut, double ts)
        {

            DeviceStreamingCommon.DeviceTimeout = TimeSpan.FromMilliseconds(ts);

            try
            {
                if (basicMode)
                    DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText).GetAwaiter().GetResult();
                else if (!UseCustomClass)
                    DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText, DevKeepListening, DevAutoStart, OnDeviceSvcUpdate, KeepAlive, ResponseExpected).GetAwaiter().GetResult();

                else
                    DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText, DevKeepListening, DevAutoStart, OnDeviceSvcUpdate, KeepAlive, ResponseExpected, new DeviceSvcCurrentSettings_Example()).GetAwaiter().GetResult();
            }
            //catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
            //{
            //    System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Hub connection failure");
            //}
            //catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            //{
            //    System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Device not found");
            //}
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("0Error App.RunSvc(): Task canceled");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Operation canceled");
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Timeout"))
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): " + ex.Message);
                else
                {
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Timeout");
                }
            }
        }
    }
}
