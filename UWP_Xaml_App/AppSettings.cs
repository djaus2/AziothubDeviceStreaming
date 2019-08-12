using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UWPXamlApp
{
    public static class AppSettings
    {
        public static  void LoadConSettings()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.Keys.Contains("ConDetail"))
            {
                Windows.Storage.ApplicationDataCompositeValue composite =
                        (Windows.Storage.ApplicationDataCompositeValue)localSettings.Values["ConDetail"];
                if (composite != null)
                {
                    //Ref: https://stackoverflow.com/questions/9404523/set-property-value-using-property-name
                    Type type = typeof(IoTHubConnectionDetails); // IoTHubConnectionDetails is static class with public static properties
                    foreach (var property in type.GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static)) //(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic))
                    {
                        string propertyName = property.Name;
                        if (composite.Keys.Contains(propertyName))
                        {
                            //Want to implement Cons.propertyName = composite[propertyName];
                            var propertyInfo = type.GetProperty(propertyName); //, System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Static);
                            propertyInfo.SetValue(type, composite[propertyName], null);
                        }
                    }
                }
            }
        }

        public static void SaveSettingsToAppData()
        {
            Windows.Storage.ApplicationDataContainer localSettings = Windows.Storage.ApplicationData.Current.LocalSettings;
            if (localSettings.Values.Keys.Contains("ConDetail"))
            {
                localSettings.Values.Remove("ConDetail");
            }
            Windows.Storage.ApplicationDataCompositeValue composite = new Windows.Storage.ApplicationDataCompositeValue();

            //Ref: https://stackoverflow.com/questions/12480279/iterate-through-properties-of-static-class-to-populate-list
            Type type = typeof(IoTHubConnectionDetails); // IoTHubConnectionDetails is static class with public static properties
            foreach (var property in type.GetProperties()) //(System.Reflection.BindingFlags.Static | System.Reflection.BindingFlags.NonPublic))
            {
                string propertyName = property.Name;
                var val = property.GetValue(null); // static classes cannot be instanced, so use null...

                System.Diagnostics.Debug.WriteLine(string.Format("{0} {1}", propertyName, val));
                composite[propertyName] = val;
            }
            localSettings.Values.Add("ConDetail", composite);

        }
    }
}
