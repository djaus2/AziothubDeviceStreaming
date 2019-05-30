using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AzIoTHubDeviceStreams;
using System.Threading.Tasks;

//Ref: https://github.com/PieEatingNinjas/UWPConsoleSvcApp/tree/master/Source/UwpConsole

namespace UWPConsoleSvcApp
{
    public static class Program
    {
        static string service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
        static string device_id = AzureConnections.MyConnections.DeviceId;
        static string device_cs = AzureConnections.MyConnections.DeviceConnectionString;

        public static int Main(string[] args)
        {
            Console.WriteLine("Svc: Starting.\n");

            RunSvc(service_cs, device_id, "Hello Word", 100000);

            Console.WriteLine("Svc Done.\n\nPress any key to finish.\n");
            Console.ReadKey();
            return 0;
        }

        private static void OnrecvText(string msg)
        {
            Console.WriteLine(msg);
        }

        private static void RunSvc(string servvicecs, string devid, string msgOut, double ts)
        {

            DeviceStreamingCommon._Timeout = TimeSpan.FromMilliseconds(ts);

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
