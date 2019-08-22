// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Azure IoT Hub device SDK for .NET
// For samples see: https://github.com/Azure/azure-iot-sdk-csharp/tree/master/iothub/device/samples

using System;
using Microsoft.Azure.Devices.Client;
using Newtonsoft.Json;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;

namespace SimulatedDevice_ns
{
    public static class Weather
    {
        public static async Task<string> GetAsync(string uri)
        {
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(uri);

            using (HttpWebResponse response = (HttpWebResponse)await request.GetResponseAsync())
            using (Stream stream = response.GetResponseStream())
            using (StreamReader reader = new StreamReader(stream))
            {
                return await reader.ReadToEndAsync();
            }
        }

        public class City
        {
            public City()
            { }

            public int id { get; set; }
            public string name { get; set; }
            public string country { get; set; }

            public Coords coord { get; set; }

            public class Coords
            {
                public float lon { get; set; }

                public float lat { get; set; }
            }
        }

        public class TelemetryDataPoint
        {
            public string city { get; set; }
            public int temperature { get; set; }
            public int pressure { get; set; }
            public int humidity { get; set; }

            public TelemetryDataPoint()
            {

            }
        }

        public static void GetNextCity()
        {
            CurrentCityIndex++;
            if (CurrentCityIndex >= Cities.Length)
                CurrentCityIndex = 0;
        }
        public static int CurrentCityIndex { get; set; } = 0;
        public static City[] Cities { get; set; } = null;
        public static void ReadCities()
        {
            string TempFile = "cities.json";
            var fileStream = new FileStream(TempFile, FileMode.Open, FileAccess.Read);
            using (var streamReader = new StreamReader(fileStream, Encoding.UTF8))
            {
                string weatherjson = streamReader.ReadToEnd();
                Cities = JsonConvert.DeserializeObject<City[]>(weatherjson);
            }
            CurrentCityIndex = 0;
        }


            public static async Task<TelemetryDataPoint> GetWeatherObj()
        {
            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?id={0}&appid={1}",Cities[CurrentCityIndex].id, AzureConnections.MyConnections.OpenWeatherAppKey);
            string weatherjson = await GetAsync(url);
            dynamic obj = JsonConvert.DeserializeObject(weatherjson);
            dynamic fgh = obj.main;

            dynamic temp = obj.main.temp;
            dynamic press = obj.main.pressure;
            dynamic humid = obj.main.humidity;
            var otemperature = (int)(float.Parse(temp.ToString()));// ((int)obj["main"].GetObject()["temp"].GetNumber()) - 273;
            var opressure = (int)(int.Parse(press.ToString()));// (int)obj["main"].GetObject()["pressure"].GetNumber();
            var ohumidity = (int)(int.Parse(humid.ToString()));// (int)obj["main"].GetObject()["humidity"].GetNumber();
            var telemetryDataPoint = new TelemetryDataPoint()
            {
                city = Weather.Cities[Weather.CurrentCityIndex].name,
                temperature = otemperature-273,
                pressure = opressure,
                humidity = ohumidity
            };
            return telemetryDataPoint;
        }



    }

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

                var telemetryDataPoint =await  Weather.GetWeatherObj();
                //Create JSON message
               //var telemetryDataPoint = new
               //{
               //    temperature = currentTemperature,
               //    humidity = currentHumidity
               //};
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
                Weather.GetNextCity();
                
            }
        }

        private static int Delay = 10000;
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
            if (Weather.Cities == null)
                Weather.ReadCities();
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

