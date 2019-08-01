// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;

namespace SimulatedDevice_ns
{
   
    public class SimulatedDevice
    {
        public delegate void ActionReceivedText(string recvTxt);

        private static DeviceClient s_deviceClient;

        // The device connection string to authenticate the device with your IoT hub.
        // Using the Azure CLI:
        // az iot hub device-identity show-connection-string --hub-name {YourIoTHubName} --device-id MyDotnetDevice --output table
        private static string s_connectionString = "{Your device connection string here}";

        public static bool ContinueLoop {get; set;}=false;

        public static string MessageString { get; set; } = "";

        public static Microsoft.Azure.Devices.Client.Message Message = null;
        public static string IOTMess { get; set; } = "";

        // Async method to send simulated telemetry
        private static async Task SendDeviceToCloudMessagesAsync()
        {
            // Initial telemetry values
            double minTemperature = 20;
            double minHumidity = 60;
            Random rand = new Random();
            ContinueLoop = true;
            while (ContinueLoop)
            {
                double currentTemperature = minTemperature + rand.NextDouble() * 15;
                double currentHumidity = minHumidity + rand.NextDouble() * 20;

                // Create JSON message
                var telemetryDataPoint = new
                {
                    temperature = currentTemperature,
                    humidity = currentHumidity
                };
                MessageString = JsonConvert.SerializeObject(telemetryDataPoint);
                
                Message = new Message(Encoding.ASCII.GetBytes(MessageString));

                //Stuff:
                //var mess2 = Encoding.ASCII.GetBytes(MessageString);
                //var qwe = Message.GetBytes();
                //string MessageString2= Encoding.UTF8.GetString(qwe, 0, qwe.Length);
                // Add a custom application property to the message.
                // An IoT hub can filter on these properties without access to the message body.

                Message.Properties.Add("temperatureAlert", (currentTemperature > 30) ? "true" : "false");
                Message.Properties.Add("temperatureAlert2", (currentTemperature > 40) ? "true" : "false");
                AzIoTHubModules.SyntheticIoTMessage iotmessage = new AzIoTHubModules.SyntheticIoTMessage(Message);
                MessageString = iotmessage.Serialise();

                

                System.Diagnostics.Debug.WriteLine("{0} > Sending message: {1}", DateTime.Now, MessageString);
                OnDeviceStatusUpdateD?.Invoke(string.Format("{0} > Sending message: {1}", DateTime.Now, MessageString));

                // Send the telemetry message
                if (!IsDeviceStreaming)
                {
                    await s_deviceClient.SendEventAsync(Message);
                    await Task.Delay(Delay);
                }
                else
                {
                    ContinueLoop= false;
                }
                
            }
        }

        private static int Delay = 1000;
        private static bool IsDeviceStreaming = false;

        public static bool IsConfigured { get; set; } = false;

        private static ActionReceivedText OnDeviceStatusUpdateD;


        public static void Configure(string device_cs, bool isDeviceStreaming, TransportType transportType, bool loop, ActionReceivedText onDeviceStatusUpdateD = null, int delay=1000)
        {
            Delay = delay;
            OnDeviceStatusUpdateD = onDeviceStatusUpdateD;
            IsDeviceStreaming = isDeviceStreaming;

            s_connectionString = device_cs;

            if (!IsDeviceStreaming)
            {
                s_deviceClient = DeviceClient.CreateFromConnectionString(s_connectionString, transportType);
            }
            ContinueLoop = true;
        }
        public static async Task<string>  Run()
        {
            MessageString = "";
            System.Diagnostics.Debug.WriteLine("IoT Hub Quickstarts #1 - Simulated device started.");
            // Connect to the IoT hub using the MQTT protocol
           
            await SendDeviceToCloudMessagesAsync();
            if (!IsDeviceStreaming)
            {
                System.Diagnostics.Debug.WriteLine("Simulated Device Done");
                await s_deviceClient.CloseAsync();
                MessageString = "";
            }

            return MessageString;
        }
    }
}

