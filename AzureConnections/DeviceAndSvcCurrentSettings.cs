using System;
using System.Collections.Generic;
using System.Text;

namespace AzureConnections
{
    public static class Info
    {
        //Signals from svc to device 
        //Prepended to sent message if relevant state required
        public const char KeppAliveChar = '`';
        public const char RespondChar = '~';

        //Defaults
        public const bool KeppAliveDef = false;
        public const bool RespondDef = true;

    }
    public class DeviceCurrentSettings
    {
        private  char keepAliveChar = Info.KeppAliveChar;
        private  char respondChar = Info.RespondChar;

        private bool respond = Info.RespondDef; //default
        public bool keepAlive = Info.KeppAliveDef; //default

        /// <summary>
        /// The following 3 methods are used as delegates
        /// </summary>

        // Called once msg is received to determine if a response to service is expected.
        public bool GetRespond()
        { return respond; }

        // Called once msg received and optionally processed to determine whether to loop again and wait for another msg from svc
        public bool GetKeepAlive()
        { return keepAlive; }

        // Called if response required
        public string ProcessMsgIn(string msgIn)
        {
            //Use defaults for empty string and if flag chars not in prepend
            keepAlive = Info.KeppAliveDef;
            respond = Info.RespondDef;

            if ( !string.IsNullOrEmpty(msgIn))
            {
                // ?Keepalive possibly followed by respond chars
                if (msgIn.ToLower()[0] == keepAliveChar)
                {
                    keepAlive = true;
                    msgIn = msgIn.Substring(1);
                    if (!string.IsNullOrEmpty(msgIn))
                    {
                        if (msgIn.ToLower()[0] == respondChar)
                        {
                            respond = true;
                            msgIn = msgIn.Substring(1);
                        }
                        else
                            respond = false;
                    }
                }

                // ?Respond possibly followed by keepalive chars
                if (msgIn.ToLower()[0] == respondChar)
                {
                    respond = true;
                    msgIn = msgIn.Substring(1);
                    if (!string.IsNullOrEmpty(msgIn))
                    {
                        if (msgIn.ToLower()[0] == keepAliveChar)
                        {
                            keepAlive = true;
                            msgIn = msgIn.Substring(1);
                        }
                        else
                            keepAlive= false;
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
        private char keepAliveChar = Info.KeppAliveChar;
        private char respondChar = Info.RespondChar;

        private bool expectResponse  = Info.RespondDef; //default
        private bool keepAlive = Info.KeppAliveDef; //default


        /// <summary>
        /// The following 4 methods are used as delegates
        /// </summary>

        // Called once msg received to determine whether to loop again and wait for another send request from app
        public bool GetKeepAlive()
        { return keepAlive; }

        // Called once msg is sent to determine if a response from device is expected.
        public bool GetExpectResponse()
        { return expectResponse; }

        //This is called prior to message being sent by svc
        public string ProcessMsgOut(string msgOut, bool _keepAlive = false, bool _expectResponse = true)
        {
            //Defaut is to signal not keep alive and for device to send response.
            keepAlive = _keepAlive;
            expectResponse = _expectResponse;
            //Prepend message with indicative to device chars if relevant flags are true
            if (keepAlive)
                msgOut = keepAliveChar + msgOut;
            if (expectResponse)
                msgOut = respondChar + msgOut;
            return msgOut;
        }

        // Called when optional response is received
        public string ProcessMsgIn(string msgIn)
        {
            //Include any received message processing here.
            string msgOut = msgIn;
            return msgOut;
        }

    }
}
