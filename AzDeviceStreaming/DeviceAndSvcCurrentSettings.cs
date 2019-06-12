using System;
using System.Collections.Generic;
using System.Text;

namespace AzIoTHubDeviceStreams
{

    public class DeviceAndSvcCurrentSettings 
    {
        public static class Info
        {
            //Signals from svc to device 
            //Prepended to sent message if relevant state required
            public static char KeppAliveChar = '`';
            public static char RespondChar = '~';

        }

        private bool responseExpected = false;
        private bool keepAlive = false;
        /// <summary>
        /// If true then device needs to respond when message received.
        /// </summary>
        public bool ResponseExpected { get => responseExpected; set => responseExpected = value; }
        /// <summary>
        /// If true then keep socket alive and wait for another message when post reception processing is finished.
        /// </summary>
        public bool KeepAlive { get => keepAlive; set => keepAlive = value; }

        // Called to get flags
        /// <summary>
        /// Called on reception of message to get flags
        /// </summary>
        /// <param name="msgIn">Message with flags embedded</param>
        /// <returns>Message without "flags"</returns>
        public virtual string ProcessMsgIn(string msgIn)
        {
            KeepAlive = false;
            ResponseExpected = false;

            if (!string.IsNullOrEmpty(msgIn))
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
                            ResponseExpected = true;
                            msgIn = msgIn.Substring(1);
                        }
                        else
                            ResponseExpected = false;
                    }
                }

                // ?Respond possibly followed by keepalive chars
                if (msgIn.ToLower()[0] == Info.RespondChar)
                {
                    ResponseExpected = true;
                    msgIn = msgIn.Substring(1);
                    if (!string.IsNullOrEmpty(msgIn))
                    {
                        if (msgIn.ToLower()[0] == Info.KeppAliveChar)
                        {
                            KeepAlive = true;
                            msgIn = msgIn.Substring(1);
                        }
                        else
                            KeepAlive = false;
                    }
                }
            }
            return msgIn;
        }


        //
        /// <summary>
        /// Called prior to message being sent by svc. Embed property flags in message
        /// </summary>
        /// <param name="msgOut"></param>
        /// <param name="keepAlive"></param>
        /// <param name="responseExpected"></param>
        /// <returns>Message to be sent</returns>
        public virtual string ProcessMsgOut(string msgOut, bool keepAlive = false, bool responseExpected = true)
        {
            KeepAlive = keepAlive;
            ResponseExpected = responseExpected;
            //Prepend message with indicative chars (for device to interpret as abvoe) if relevant flags are true
            if (keepAlive)
                msgOut = Info.KeppAliveChar + msgOut;
            if (responseExpected)
                msgOut = Info.RespondChar + msgOut;
            return msgOut;
        }

    }
}
