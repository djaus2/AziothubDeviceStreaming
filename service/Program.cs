// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using AzIoTHubDeviceStreams;

namespace Microsoft.Azure.Devices.Samples
{
    public static class Program
    {
        static string service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
        static string device_id = AzureConnections.MyConnections.DeviceId;
        static string device_cs = AzureConnections.MyConnections.DeviceConnectionString;


        public static int Main(string[] args)
        {
            Console.WriteLine("Svc: Start.\n");

            RunSvc(service_cs, device_id, "Hello Word", 100000).GetAwaiter().GetResult();

            Console.WriteLine("Svc: Done.\n");
            Console.ReadKey();
            return 0;
        }

        private static void OnrecvText(string msg)
        {
            Console.WriteLine(msg);
        }

        private static async Task RunSvc(string servvicecs, string devid,string msgOut,double ts)
        {

            DeviceStreamingCommon._Timeout = TimeSpan.FromMilliseconds(ts);
            try
            {
                //var svc =
                await Task.Run(() =>
                {
                    try
                    {

                        DeviceStreamSvc.RunSvc(servvicecs, devid,msgOut, OnrecvText);

                    }
                    //catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                    //{
                    //    System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Hub connection failure");
                    //}
                    catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                    {
                        System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Device not found");
                    }
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
                });

            }
            //catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
            //{
            //    System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Hub connection failure");
            //}
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Device not found");
            }
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error RunSvc(): Task canceled");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error RunSvc(): Operation canceled");
            }
            catch (Exception ex)
            {
                if (!ex.Message.Contains("Timeout"))
                    System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): " + ex.Message);
                else
                {
                    System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Timeout");
                }
            }

        }
    }
}
