using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client.Exceptions;


namespace AzureConnections
{
    public static partial class MyConnections
    {
        public delegate void ActionReceivedText(string recvTxt);
        public static ActionReceivedText OnStatusUpdateD { get; set; } = null;

        public static string DeviceId { get; set; } = "MyNewDevice";

        public static string IoTHubConnectionString { get; set; } = "";

        public static string DeviceConnectionString { get; set; } = "";



        public static int DeviceAction = 3; //Uppercase. See OnDeviceRecvTextIO() below for options

        public static bool basicMode { get; set; } = false;
        public  static bool UseCustomClass { get; set; } = false;
        public  static bool ResponseExpected { get; set; } = true;
        public  static bool KeepAlive { get; set; } = false;

        public static bool KeepDeviceListening { get; set; }  = true;
        private static bool autoStartDevice  = true;
        public static bool AutoStartDevice { get => autoStartDevice; set { autoStartDevice = value; if (value) KeepDeviceListening = true; } }


   }
}
