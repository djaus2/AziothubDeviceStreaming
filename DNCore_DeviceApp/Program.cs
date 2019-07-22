// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using System;
using System.Threading.Tasks;
using AzIoTHubDeviceStreams;

namespace DeviceDNCoreApp
{
    public static class Program
    {
        private static int iGroupDeviceAction = 1;

        public static bool KeepDeviceListening { get; private set; } = true;

        private static bool autoStartDevice = true;
        //The next is superfulous as this device app will always autostart.
        private static bool AutoStartDevice { get => autoStartDevice; set { autoStartDevice = value; if (value) KeepDeviceListening = true; } }
        static string service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
        static string device_id = AzureConnections.MyConnections.DeviceId;
        static string device_cs = AzureConnections.MyConnections.DeviceConnectionString;

        //If true uses the original mode
        private static bool basicMode = false;
        //For tuture expansion:
        private static bool useCustomClass = false;
        

        public static int Main(string[] args)
        {
            //Set these here:
            AzureConnections.MyConnections.DeviceId = ""; 
            AzureConnections.MyConnections.DeviceConnectionString = "";
            Console.WriteLine("Device starting.\n");

            RunDevice(device_cs, 10000000);

            Console.WriteLine("Device Done.\n\nPress any key to finish.\n");
            Console.ReadKey();
            return 0;
        }

 
        private static string AppendMsg ="";
        private static string OnDeviceRecvTextIO(string msgIn)
        {
            if (AppendMsg != "")
            {
                Console.WriteLine("Recvd: " + AppendMsg);
                AppendMsg = "";
            }
            Console.WriteLine("Recvd: " + msgIn);
            //Perform device side processing here. Eg read sensors.
            string msgOut = msgIn;
            switch (iGroupDeviceAction)
            {
                case 0:
                    msgOut = msgIn;
                    break;
                case 1:
                    msgOut = msgIn.ToUpper();
                    break;
                case 2:
                    switch (msgIn.Substring(0, 3).ToLower())
                    {
                        case "tem":
                            msgOut = "45 C";
                            break;
                        case "pre":
                            msgOut = "1034.0 hPa";
                            break;
                        case "hum":
                            msgOut = "67%";
                            break;
                        default:
                            msgOut = "Invalid request";
                            break;
                    }
                    break;
                case 3:
                    msgOut = "Coming. Not yet implemented. This is a pace holder for now.";
                    break;
                case 4:
                    msgOut = "Coming. Not yet implemented. This is a pace holder for now.";
                    break;
            }
            Console.WriteLine("Sent: " + msgOut);
            return msgOut;
        }

        private static void RunDevice(string device_cs, double ts)
        {
            DeviceStreamingCommon.DeviceTimeout = TimeSpan.FromMilliseconds(ts);
            try
            {
                if (basicMode)
                    DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO).GetAwaiter().GetResult();
                else if (!useCustomClass)
                    DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO, OnDeviceStatusUpdate, ActionCommand, KeepDeviceListening).GetAwaiter().GetResult();
                else
                    DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO, OnDeviceStatusUpdate, ActionCommand, KeepDeviceListening, new DeviceSvcCurrentSettings_Example()).GetAwaiter().GetResult();
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

        private static void OnDeviceStatusUpdate(string recvTxt)
        {
            //AppendMsg += recvTxt +"\r\n";
            System.Diagnostics.Debug.WriteLine(recvTxt);
            Console.WriteLine(recvTxt);
        }
    }
}
