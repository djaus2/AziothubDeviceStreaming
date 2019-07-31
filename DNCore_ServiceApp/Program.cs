// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using AzIoTHubDeviceStreams;

namespace ServiceDNCoreApp
{
    public static class Program
    {
        private static int DeviceAction = AzureConnections.MyConnections.DeviceAction;

        private static bool basicMode = AzureConnections.MyConnections.basicMode;
        private static bool UseCustomClass = AzureConnections.MyConnections.UseCustomClass;
        private static bool ResponseExpected = AzureConnections.MyConnections.ResponseExpected;
        private static bool KeepAlive = AzureConnections.MyConnections.KeepAlive;

        private static string service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
        private static string device_id = AzureConnections.MyConnections.DeviceId;
        private static string device_cs = AzureConnections.MyConnections.DeviceConnectionString;

        private static string msgOut = "Temp";

        private static int DevKeepListening =  2; //No action
        private static int DevAutoStart=2; //No action

        public static int Main(string[] args)
        {
            Console.WriteLine("Svc: Starting.\n");
            Console.WriteLine("Sending :" + msgOut);
            RunSvc(service_cs, device_id, msgOut, 100000);

            Console.WriteLine("Svc Done.\n\nPress any key to finish.\n");
            Console.ReadKey();
            return 0;
        }

        private static void OnSvcRecvText(string msg)
        {
            Console.WriteLine("Recvd back: " + msg);
        }

        private static void OnDeviceSvcUpdate(string recvTxt)
        {
            if (!string.IsNullOrEmpty(recvTxt))
                Console.WriteLine("Update: " + recvTxt);
        }

        private static void RunSvc(string servvice_cs, string device_id,string msgOut,double ts)
        {
            int devAutoStart = DevAutoStart;
            int devKeepListening = DevKeepListening;
            DevAutoStart = 2;
            DevKeepListening = 2;



            DeviceStreamingCommon.DeviceTimeout = TimeSpan.FromMilliseconds(ts);

            try
            {
                if (basicMode)
                    DeviceStream_Svc.RunSvc(Program.service_cs, Program.device_id, msgOut, OnSvcRecvText).GetAwaiter().GetResult();
                else if (!UseCustomClass)
                    DeviceStream_Svc.RunSvc(Program.service_cs, Program.device_id, msgOut, OnSvcRecvText, devKeepListening, devAutoStart, OnDeviceSvcUpdate, KeepAlive, ResponseExpected).GetAwaiter().GetResult();

                else
                    DeviceStream_Svc.RunSvc(Program.service_cs, Program.device_id, msgOut, OnSvcRecvText, devKeepListening, devAutoStart, OnDeviceSvcUpdate, KeepAlive, ResponseExpected, new DeviceSvcCurrentSettings_Example()).GetAwaiter().GetResult();

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
        }


    }
}
