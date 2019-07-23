using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UWPXamlApp
{
    /// <summary>
    /// Write a custom handler of KeepAlive and ResponseExpected flag handling.
    /// 
    /// In MainPage_Service.cs, for device change:
    /// DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvText).GetAwaiter().GetResult();
    /// to: 
    /// DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvText, new DeviceSvcCurrentSettings_Example()).GetAwaiter().GetResult();
    /// 
    /// In MainPage_Device.cs, for svc change:
    /// DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText, keepAlive, responseExpected).GetAwaiter().GetResult();
    /// to: 
    /// DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnSvcRecvText, keepAlive, responseExpected, new DeviceSvcCurrentSettings_Example()).GetAwaiter().GetResult();
    /// </summary>
    public class DeviceSvcCurrentSettings_Example : AzIoTHubDeviceStreams.DeviceAndSvcCurrentSettings
    {
        public Load_DeviceAndSvcCurrentSettings plugin = null;
        public void LoadPlugIn(string path)
        {
            plugin = new Load_DeviceAndSvcCurrentSettings(path);
        }
        public string Name { get; set; } = "MainPage_DeviceSvcCurrentSettingsExample";
        public override string ProcessMsgIn(string msgIn)
        {
            if (plugin == null)
            {
                if (msgIn[0] == Info.KeepAliveChar)
                    KeepAlive = true;
                else
                    KeepAlive = false;
                //etc.
                throw new NotImplementedException("NotImplemented: DeviceSvcCurrentSettingsExample.ProcessMsgIn()");
            }
            else
            {
                return ProcessMsgIn(msgIn);
            }
        }

        public override string ProcessMsgOut(string msgOut, bool keepAlive = false, bool responseExpected = true, int DevKeepListening = 2, int DevAutoStart = 2
        {
            if (plugin == null)
            {
                KeepAlive = keepAlive;
                ResponseExpected = responseExpected;
                //etc.
                throw new NotImplementedException("Not Implemented: DeviceSvcCurrentSettingsExample.ProcessMsgOut()");
            }
            else
            {
                return (string) plugin.ProcessMsgOut( msgOut);
            }
        }
    }

    public class Load_DeviceAndSvcCurrentSettings
    {
        public string ProcessMsgIn(string msgIn)
        {
            if(instance==null)
               return  "device error1";
            if (processMsgIn == null)
                return "device error2";
            return (string)processMsgIn?.Invoke(instance, new string[] { msgIn });
        }
        public string ProcessMsgOut(string msgOut)
        {
            if (instance == null)
                return "svc error1";
            if (processMsgOut == null)
                return "svc error2";
            return (string)processMsgOut?.Invoke(instance, new string[] { msgOut });
        }

        private MethodInfo processMsgIn = null;
        private MethodInfo processMsgOut = null;

        private object instance = null;

        //The following is a work in progress ..
        //Ref: https://stackoverflow.com/questions/3679812/c-how-do-i-dynamically-load-instantiate-a-dll
        //Also: https://docs.microsoft.com/en-us/dotnet/framework/misc/how-to-run-partially-trusted-code-in-a-sandbox?view=netframework-4.8
        public Load_DeviceAndSvcCurrentSettings(string path)
        {
            // load assembly
            var assemblyWithReport = Assembly.LoadFrom(path);

            // or another Loadxx to get the assembly you'd 
            // like, whether it's referenced or not

            // load type
            var deviceSvcCurrentSettings = assemblyWithReport.GetTypes().ToList()
               .Where(t => t.Name == "DeviceSvcCurrentSettings").Single();

            // create instance of type
            instance = Activator.CreateInstance(deviceSvcCurrentSettings);

            // get getrecords method of the type
            processMsgIn = deviceSvcCurrentSettings.GetMethod("ProcessMsgIn");
            processMsgOut = deviceSvcCurrentSettings.GetMethod("ProcessMsgOut");

            // invoke getrecords method on the instance
            //object result = ProcessMsgIn.Invoke(instance, null);
        }
    }
}
