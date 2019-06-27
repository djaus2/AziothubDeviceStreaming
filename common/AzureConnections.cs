using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Azure.Devices;
using Microsoft.Azure.Devices.Client.Exceptions;

namespace AzureConnections
{
    public static class MyConnections
    {

        
        public static string IoTHubConnectionString { get; set; } = "HostName=djausWithStreams.azure-devices.net;SharedAccessKeyName=service;SharedAccessKey=eFIz7j/Kw1nmtPuQl6Rw/duezdCJCEF2Q3tng7TMc0w=";
        public static string DeviceId { get; set; } = "MyDevice";
        public static string DeviceConnectionString { get; set; } = "HostName=djausWithStreams.azure-devices.net;DeviceId=MyDevice;SharedAccessKey=J2ZP7sGyQ6S7o28caQaoBnsZCTZ3BJcXDzS+AcexCYc=";

        public static string AddDeviceAsync(string IoTHubOwnerconnectionString, string deviceId)
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(IoTHubOwnerconnectionString);
            registryManager.OpenAsync().GetAwaiter().GetResult();
            Device device = null;

            try
            {
                device = registryManager.AddDeviceAsync(new Device(deviceId)).GetAwaiter().GetResult();
            }
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceAlreadyExistsException)
            {
                device = registryManager.GetDeviceAsync(deviceId).GetAwaiter().GetResult();
            }
            catch (Exception)
            {
                System.Diagnostics.Debug.WriteLine("Device creation failed and device doesn't already exist.");
                device = null;
            }
            //Console.WriteLine("Generated device key: {0}", device.Authentication.SymmetricKey.PrimaryKey);

            if (device != null)
            {
                string[] conparts = IoTHubOwnerconnectionString.Split(new char[] { ';' });
                if (conparts.Length == 3)
                {
                    DeviceConnectionString = string.Format("{0};DeviceId={1};SharedAccessKey={2}",
                     conparts[0], deviceId, device.Authentication.SymmetricKey.PrimaryKey);
                    IoTHubConnectionString = IoTHubOwnerconnectionString;
                    DeviceId = deviceId;
                    return DeviceConnectionString;
                }
            }
            return "";
        }
    }
}
