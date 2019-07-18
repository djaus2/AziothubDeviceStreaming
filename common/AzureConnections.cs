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
        public delegate void ActionReceivedText(string recvTxt);
        public static ActionReceivedText OnStatusUpdateD { get; set; } = null;

        public static string IoTHubConnectionString { get; set; } = "";
        public static string DeviceId { get; set; } = "MyDevice";
        public static string DeviceConnectionString { get; set; } = "";

        public static string AddDeviceAsync(string IoTHubOwnerconnectionString, string deviceId)
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(IoTHubOwnerconnectionString);
            registryManager.OpenAsync().GetAwaiter().GetResult();
            Device device = null;

            try
            {
                device = registryManager.AddDeviceAsync(new Device(deviceId)).GetAwaiter().GetResult();
                if (device != null)
                    OnStatusUpdateD?.Invoke("AddDeviceAsync: Device created OK.");
                else
                    OnStatusUpdateD?.Invoke("AddDeviceAsync: Unable to create Device.");
            }
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceAlreadyExistsException)
            {
                device = registryManager.GetDeviceAsync(deviceId).GetAwaiter().GetResult();
                if (device != null)
                    OnStatusUpdateD?.Invoke("AddDeviceAsync: Device already exists but has been found OK.");
                else
                    OnStatusUpdateD?.Invoke("AddDeviceAsync: Device already exists but HASN'T been found.");

            }
            catch (Exception)
            {
                OnStatusUpdateD?.Invoke("AddDeviceAsync: Device creation failed and/or device doesn't already exist.");
                System.Diagnostics.Debug.WriteLine("AddDeviceAsync: Device creation failed and/or device doesn't already exist.");
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

        public static string RemoveDeviceAsync(string IoTHubOwnerconnectionString, string deviceId)
        {
            if (deviceId=="")
            {
                OnStatusUpdateD?.Invoke(string.Format("RemoveDeviceAsync: Device Not deleted. Blank DeviceId."));

                return "";
            }
            else if (IoTHubOwnerconnectionString == "")
            {
                OnStatusUpdateD?.Invoke(string.Format(string.Format("RemoveDeviceAsync: Device {0} Not deleted. Blank IoTHubOwnerconnectionString.", deviceId)));

                return "";
            }
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(IoTHubOwnerconnectionString);
            registryManager.OpenAsync().GetAwaiter().GetResult();
            Device device = null;
            try
            {
                device = registryManager.GetDeviceAsync(deviceId).GetAwaiter().GetResult();
                if (device != null)
                {
                    registryManager.RemoveDeviceAsync(device).GetAwaiter().GetResult();
                }
                else
                {
                    OnStatusUpdateD?.Invoke(string.Format("RemoveDeviceAsync: Device {0} Not deleted. Unable to find Device.", deviceId));
                    return "";
                }

                device = null;
                OnStatusUpdateD?.Invoke(string.Format("RemoveDeviceAsync: Device {0} Deleted.", deviceId));

            }
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            {
                OnStatusUpdateD?.Invoke(string.Format("RemoveDeviceAsync: Device {0} Not deleted. Unable to find Device.", deviceId));
            }
            catch (Exception ex)
            {
                OnStatusUpdateD?.Invoke(string.Format("RemoveDeviceAsync: Device {0} deletion failed: {1} " , deviceId, ex.Message));
                System.Diagnostics.Debug.WriteLine(string.Format("RemoveDeviceAsync: Device {0} deletion failed: {1} ", deviceId, ex.Message));
                device = null;
            }
            
            DeviceId = "";
            DeviceConnectionString = "";
            return "";
        }

        public static string GetDeviceAsync(string IoTHubOwnerconnectionString, string deviceId)
        {
            RegistryManager registryManager = RegistryManager.CreateFromConnectionString(IoTHubOwnerconnectionString);
            registryManager.OpenAsync().GetAwaiter().GetResult();
            Device device = null;

            try
            {
                device = registryManager.GetDeviceAsync(deviceId).GetAwaiter().GetResult();
                if (device != null)
                    OnStatusUpdateD?.Invoke("AddDeviceAsync: Device exists and has been found OK.");
                else
                    OnStatusUpdateD?.Invoke("AddDeviceAsync: Device not been found.");
            }
            catch (Exception ex)
            {
                OnStatusUpdateD?.Invoke("AddDeviceAsync: Failed to get device- " + ex.Message);
                System.Diagnostics.Debug.WriteLine("AddDeviceAsync: Failed to get device: ", ex.Message);
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
