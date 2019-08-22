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
        public static string OpenWeatherAppKey { get; set; } = "df39100f7fe7b297c789818c5f2bb1bd";//Need this https://openweathermap.org/
        public static int WaitAtEndOfConsoleAppSecs { get; set; } = 5; //Secs
        public static int Timeout { get; set; } = 30000; //30 secs

        public delegate void ActionReceivedText(string recvTxt);
        public static ActionReceivedText OnStatusUpdateD { get; set; } = null;

        /// <summary>
        /// Hub Connections
        /// </summary>
        /// 
        public static bool EHMethod1 { get; set; }

        public static string AzureGroup { get; set; } = "MyNewGroup";

        public static string IoTHubName { get; set; } = "MyNewHub";

        public static string SKU { get; set; } = "F1";

        public static string DeviceId { get; set; } = "MyNewDevice";

        public static string IoTHubConnectionString { get; set; } = "";

        public static string DeviceConnectionString { get; set; } = "HostName=MyNewHub.azure-devices.net;DeviceId=MyNewDevice;SharedAccessKey=uuEXJ2WfzGE5/otMfsmkMhax/bpPCdN/fdcsz4jAw5k=";

        public static string EventHubsConnectionString { get; set; } = "";

        public static string EventHubsCompatibleEndpoint { get; set; } = "";

        // Event Hub-compatible name
        // az iot hub show --query properties.eventHubEndpoints.events.path --name {your IoT Hub name}
        public static string EventHubsCompatiblePath { get; set; } = "";

        public static string IotHubKeyName { get; set; } = "iothubowner";

        public static string HUbSku { get; set; } = "";

        public static string IoTHubLocation { get; set; } = "centralus";

        public static string EventHubsSasKey { get; set; } = "";


        /////////////////////////////////////////////////////////////////////////


        public static int DeviceAction = 3; //Uppercase. See OnDeviceRecvTextIO() below for options

        public static bool basicMode { get; set; } = false;
        public static bool UseCustomClass { get; set; } = false;
        public static bool ResponseExpected { get; set; } = true;
        public static bool KeepAlive { get; set; } = false;

        public static bool KeepDeviceListening { get; set; } = true;
        private static bool autoStartDevice = true;
        public static bool AutoStartDevice { get => autoStartDevice; set { autoStartDevice = value; if (value) KeepDeviceListening = true; } }


    }
}
