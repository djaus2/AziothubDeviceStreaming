using System;
using System.Collections.Generic;
using System.Text;

namespace AzureConnections
{
    public class DeviceCurrentSettings
    {
        private bool _Respond { get; set; } = false;
        public bool _KeepAlive { get; set; } = false;

        public bool GetKeepAlive()
        { return _KeepAlive; }

        public bool GetRespond()
        { return _Respond; }

        public void SetKeepAlive(bool val)
        { _KeepAlive = val; }

        public void SetRespond(bool val)
        { _Respond = val; }
    }
}
