using System;
using System.Collections.Generic;
using System.Text;

namespace AzureConnections
{
    public class DeviceCurrentSettings
    {
        private bool _Respond { get; set; } = true; //default
        public bool _KeepAlive { get; set; } = false; //default

        public bool GetKeepAlive()
        { return _KeepAlive; }

        public bool GetRespond()
        { return _Respond; }

        public void SetKeepAlive(bool val)
        { _KeepAlive = val; }

        public void SetRespond(bool val)
        { _Respond = val; }

        public string ProcessMsgIn(string msgIn)
        {
            if ( !string.IsNullOrEmpty(msgIn))
            {
                //A simple implmentation of settings. Device calls GetKeepAlive() and GetRespond() to get these.
                if (msgIn.ToLower()[0] == '~')
                {
                    SetKeepAlive(true);
                    msgIn = msgIn.Substring(1);
                    if (!string.IsNullOrEmpty(msgIn))
                    {
                        if (msgIn.ToLower()[0] == '`')
                        {
                            SetRespond(true);
                            msgIn = msgIn.Substring(1);
                        }
                        else
                            SetRespond(false);
                    }
                }
                else
                    SetKeepAlive(false);

                if (msgIn.ToLower()[0] == '`')
                {
                    SetRespond(true);
                    msgIn = msgIn.Substring(1);
                    if (!string.IsNullOrEmpty(msgIn))
                    {
                        if (msgIn.ToLower()[0] == '~')
                        {
                            SetKeepAlive(true);
                            msgIn = msgIn.Substring(1);
                        }
                        else
                            SetKeepAlive(false);
                    }
                }   
                else
                    SetRespond(false);
            }
            else
            {
                //Use defaults for empty string
                SetKeepAlive(false);
                SetRespond(true);
            }

            string msgOut = msgIn.ToUpper();
            return msgOut;
        }

 
    }

    public class SvcCurrentSettings
    {
        private bool expectResponse { get; set; } = true; //default
        private bool keepAlive { get; set; } = false; //default

        public bool GetKeepAlive()
        { return keepAlive; }

        public bool GetExpectResponse()
        { return expectResponse; }

        public void SetKeepAlive(bool val)
        { keepAlive = val; }

        public void SetExpectResponse(bool val)
        { expectResponse = val; }

        public string ProcessMsgOut(string msgOut, bool keepAlive = false, bool expectReply = true)
        {
            SetKeepAlive(keepAlive);
            SetExpectResponse(expectReply);
            if (keepAlive)
                msgOut = "~" + msgOut;
            if (expectReply)
                msgOut = "`" + msgOut;
            return msgOut;
        }
        public string ProcessMsgIn(string msgIn)
        {
            string msgOut = msgIn;
            return msgOut;
        }

    }
}
