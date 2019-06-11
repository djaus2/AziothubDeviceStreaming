using System;
using System.Collections.Generic;
using System.Text;

namespace AzIoTHubDeviceStreams
{
    public static class Info
    {
        //Signals from svc to device 
        //Prepended to sent message if relevant state required
        public const char KeppAliveChar = '`';
        public const char RespondChar = '~';

    }
    public class DeviceCurrentSettings
    {

        private bool respond = false;
        private bool keepAlive = false;

        public bool Respond { get => respond; set => respond = value; }
        public bool KeepAlive { get => keepAlive; set => keepAlive = value; }

        /// <summary>
        /// The following 3 methods are used as delegates
        /// </summary>


        // Called if response required
        public string ProcessMsgIn(string msgIn)
        {
            KeepAlive = false;
            Respond = false;

            if ( !string.IsNullOrEmpty(msgIn))
            {
                // ?Keepalive possibly followed by respond chars
                if (msgIn.ToLower()[0] == Info.KeppAliveChar)
                {
                    KeepAlive = true;
                    msgIn = msgIn.Substring(1);
                    if (!string.IsNullOrEmpty(msgIn))
                    {
                        if (msgIn.ToLower()[0] == Info.RespondChar)
                        {
                            Respond = true;
                            msgIn = msgIn.Substring(1);
                        }
                        else
                            Respond = false;
                    }
                }

                // ?Respond possibly followed by keepalive chars
                if (msgIn.ToLower()[0] == Info.RespondChar)
                {
                    Respond = true;
                    msgIn = msgIn.Substring(1);
                    if (!string.IsNullOrEmpty(msgIn))
                    {
                        if (msgIn.ToLower()[0] == Info.KeppAliveChar)
                        {
                            KeepAlive = true;
                            msgIn = msgIn.Substring(1);
                        }
                        else
                            KeepAlive= false;
                    }
                }   
            }


            //Generate required message out here
            //Could, for example, read a sensor
            string msgOut = msgIn.ToUpper();
            return msgOut;
        }
    }

    public class SvcCurrentSettings
    {

        private bool expectResponse = false;
        private bool keepAlive = false;

        public bool ExpectResponse { get => expectResponse; set => expectResponse = value; }
        public bool KeepAlive { get => keepAlive; set => keepAlive = value; }



        //This is called prior to message being sent by svc
        public string ProcessMsgOut(string msgOut, bool keepAlive = false, bool expectResponse = true)
        {
            KeepAlive = keepAlive;
            ExpectResponse = expectResponse;
            //Prepend message with indicative to device chars if relevant flags are true
            if (keepAlive)
                msgOut = Info.KeppAliveChar + msgOut;
            if (expectResponse)
                msgOut = Info.RespondChar + msgOut;
            return msgOut;
        }

    }
}
