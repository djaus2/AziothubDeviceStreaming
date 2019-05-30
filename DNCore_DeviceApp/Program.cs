// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using AzIoTHubDeviceStreams;

namespace DeviceDNCoreApp
{
    public static class Program
    {
        static string service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
        static string device_id = AzureConnections.MyConnections.DeviceId;
        static string device_cs = AzureConnections.MyConnections.DeviceConnectionString;
        public static int Main(string[] args)
        {
            Console.WriteLine("Device Start.\n");
            // sample.RunSampleAsync().GetAwaiter().GetResult();
            //}
            RunDevice(device_cs, 1000000);

            Console.WriteLine("Device Done.\n\nPress any key to finish.\n");
            Console.ReadKey();
            return 0;
        }

        private static string OnrecvTextIO( string msgIn)
        {
            Console.WriteLine("Recvd: " + msgIn);
            string msgOut = msgIn.ToUpper();
            Console.WriteLine("Sent: " + msgOut);
            return msgOut;
        }

        private static void RunDevice(string device_cs,double ts)
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
