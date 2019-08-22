using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.IO;
using Newtonsoft.Json;

namespace AzIoTHubModules
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
            string url = string.Format("http://api.openweathermap.org/data/2.5/weather?id={0}&appid={1}", Cities[CurrentCityIndex].id, AzureConnections.MyConnections.OpenWeatherAppKey);
            string weatherjson = await GetAsync(url);
            dynamic obj = JsonConvert.DeserializeObject(weatherjson);
            dynamic fgh = obj.main;

            dynamic temp = obj.main.temp;
            dynamic press = obj.main.pressure;
            dynamic humid = obj.main.humidity;
            var otemperature = (int)(float.Parse(temp.ToString()));
            var opressure = (int)(int.Parse(press.ToString()));
            var ohumidity = (int)(int.Parse(humid.ToString()));
            var telemetryDataPoint = new TelemetryDataPoint()
            {
                city = Weather.Cities[Weather.CurrentCityIndex].name,
                temperature = otemperature - 273,
                pressure = opressure,
                humidity = ohumidity
            };
            return telemetryDataPoint;
        }
    }
}
