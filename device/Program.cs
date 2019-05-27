// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using AzIoTHubDeviceStreams;

namespace Microsoft.Azure.Devices.Client.Samples
{
    public static class Program
    {
        static string device_cs = AzureConnections.MyConnections.IoTHubConnectionString;
        static string service_cs = AzureConnections.MyConnections.DeviceId;
        static string device_id = AzureConnections.MyConnections.DeviceConnectionString;

        public static int Main(string[] args)
        {
            Console.WriteLine("Device Start.\n");
          // sample.RunSampleAsync().GetAwaiter().GetResult();
            //}
            RunDevice(device_cs, 1000000).GetAwaiter().GetResult(); ;

            Console.WriteLine("Device Done.\n");
            Console.ReadKey();
            return 0;
        }

        private static string OnrecvTextIO( string msgIn)
        {
            Console.WriteLine(msgIn);
            string msgOut = msgIn.ToUpper();
            Console.WriteLine(msgOut);
            return msgOut;
        }

        private static async Task RunDevice(string device_cs,double ts)
        {
            DeviceStreamingCommon._Timeout = TimeSpan.FromMilliseconds(ts);
            try
            {
                //va
                await Task.Run(() =>
                {
                    try
                    {
                        DeviceStreamClient.RunClient(device_cs, OnrecvTextIO);
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
                });
            }
            catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error App.RunClient(): Hub connection failure");
            }
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error App.RunClient(): Device not found");
            }
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error App.RunClient(): Task canceled");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error App.RunClient(): Operation canceled");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Timeout"))
                    System.Diagnostics.Debug.WriteLine("00 Error App.RunClient(): " + ex.Message);
                else
                {
                    System.Diagnostics.Debug.WriteLine("00 Error App.RunClient(): Timeout");
                }
            }
        }
    }
}
