using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AzIoTHubDeviceStreams;
using System.Threading.Tasks;

//Ref: https://github.com/PieEatingNinjas/UwpConsoleApp/tree/master/Source/UwpConsole

namespace UWPConsoleDeviceApp
{
    public static class Program
    {
        static string service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
        static string  device_id = AzureConnections.MyConnections.DeviceId;
        static string device_cs = AzureConnections.MyConnections.DeviceConnectionString;

        public static int Main()
        {
            Console.WriteLine("Device Start.\n");

            RunDevice(device_cs, 1000000);

            Console.WriteLine("Device Done.\n\nPress any key to finish.\n");
            Console.ReadKey();
            return 0;
        }

        private static string OnrecvTextIO(string msgIn)
        {
            Console.WriteLine(msgIn);
            string msgOut = msgIn.ToUpper();
            Console.WriteLine(msgOut);
            return msgOut;
        }

        private static void RunDevice(string device_cs, double ts)
        {
            DeviceStreamingCommon._Timeout = TimeSpan.FromMilliseconds(ts);
 
            try
            {
                DeviceStream_Device.RunDevice(device_cs, OnrecvTextIO).GetAwaiter().GetResult();
            }
            //catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
            //{
            //    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Hub connection failure");
            //}
            //catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            //{
            //    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Device not found");
            //}
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Task canceled");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Operation canceled");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Timeout"))
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): " + ex.Message);
                else
                {
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Timeout");
                }
            }        
        }
    }
}
