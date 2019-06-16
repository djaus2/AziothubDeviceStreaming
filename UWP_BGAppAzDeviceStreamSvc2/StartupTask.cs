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
        string service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
        string device_id = AzureConnections.MyConnections.DeviceId;
        string device_cs = AzureConnections.MyConnections.DeviceConnectionString;
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Console.WriteLine("Svc: Starting.\n");

            RunSvc(service_cs, device_id, "Hello Word", 100000);

            Console.WriteLine("Svc Done.\n\nPress any key to finish.\n");
            //Console.ReadKey();
        }

        private  void OnrecvText(string msg)
        {
            Console.WriteLine(msg);
        }

        private  void RunSvc(string servvicecs, string devid, string msgOut, double ts)
        {

            DeviceStreamingCommon.DeviceTimeout = TimeSpan.FromMilliseconds(ts);

            try
            {
                DeviceStream_Svc.RunSvc(servvicecs, devid, msgOut, OnrecvText).GetAwaiter().GetResult();
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
