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
        public static int WaitAtEndOfConsoleAppSecs { get; set; } = 5; //Secs
        public static int Timeout { get; set; } = 30000; //30 secs

        public delegate void ActionReceivedText(string recvTxt);
        public static ActionReceivedText OnStatusUpdateD { get; set; } = null;

        /// <summary>
        /// Hub Connections
        /// </summary>
        /// 
        public static bool EHMethod1 { get; set; } 

        public static string AzureGroup { get; set; } = "";

        public static string IoTHubName { get; set; } = "";

        public static string SKU { get; set; } = "";

        public static string DeviceId { get; set; } = "";

        public static string IoTHubConnectionString { get; set; } = "";

        public static string DeviceConnectionString { get; set; } = "";

        public static string EventHubsConnectionString { get; set; } = "";

        public static string EventHubsCompatibleEndpoint { get; set; } = "";

        // Event Hub-compatible name
        // az iot hub show --query properties.eventHubEndpoints.events.path --name {your IoT Hub name}
        public static string EventHubsCompatiblePath { get; set; } = "";

        public static string IotHubKeyName { get; set; }  = "";

        public static string HUbSku { get; set; } = "";

        public static string IoTHubLocation { get; set; } = "";

        public static string EventHubsSasKey { get; set; } = "";


        /////////////////////////////////////////////////////////////////////////


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
