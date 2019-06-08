using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureConnections
{
    public static class MyConnections
    {
        /*
         * public static string IoTHubConnectionString = "";
        public static string DeviceId = "";
        public static string DeviceConnectionString = "";
        */
        public static string IoTHubConnectionString = "HostName=djausWithStreams.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=ngFXYdxWUeJaLo8FxRtAQLgsE4H/3I0aN9bxvMH7ZeA=";
        public static string DeviceId = "MyDevice";
        public static string DeviceConnectionString = "HostName=djausWithStreams.azure-devices.net;DeviceId=MyDevice;SharedAccessKey=J2ZP7sGyQ6S7o28caQaoBnsZCTZ3BJcXDzS+AcexCYc=";
    }
}
