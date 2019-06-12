using System;
using System.Collections.Generic;
using System.Linq;
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
        public string Name { get; set; } = "MainPage_DeviceSvcCurrentSettingsExample";
        public override string ProcessMsgIn(string msgIn)
        {
            if (msgIn[0] == Info.KeppAliveChar)
                KeepAlive = true;
            else
                KeepAlive = false;
            //etc.
            throw new NotImplementedException("NotImplemnted: DeviceSvcCurrentSettingsExample.ProcessMsgIn()");
        }

        public override string ProcessMsgOut(string msgOut, bool keepAlive = false, bool responseExpected = true)
        {
            KeepAlive = keepAlive;
            ResponseExpected = responseExpected;
            //etc.
            throw new NotImplementedException("Not Implemented: DeviceSvcCurrentSettingsExample.ProcessMsgOut()");
        }
    }
}
