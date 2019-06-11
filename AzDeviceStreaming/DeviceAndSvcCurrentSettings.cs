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
    public class DeviceCurrentSettings : IDeviceCurrentSettings
    {

        private bool respond = false;
        private bool keepAlive = false;
        /// <summary>
        /// If true then device needs to respond when message received.
        /// </summary>
        public bool Respond { get => respond; set => respond = value; }
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
        public string ProcessMsgIn(string msgIn)
        {
            KeepAlive = false;
            Respond = false;

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
                            KeepAlive = false;
                    }
                }
            }
            return msgIn;
        }
    }

    public class SvcCurrentSettings : ISvcCurrentSettings
    {

        private bool expectResponse = false;
        private bool keepAlive = false;

        public bool ExpectResponse { get => expectResponse; set => expectResponse = value; }
        public bool KeepAlive { get => keepAlive; set => keepAlive = value; }



        //
        /// <summary>
        /// Called prior to message being sent by svc. Embed property flags in message
        /// </summary>
        /// <param name="msgOut"></param>
        /// <param name="keepAlive"></param>
        /// <param name="expectResponse"></param>
        /// <returns>Message to be sent</returns>
        public string ProcessMsgOut(string msgOut, bool keepAlive = false, bool expectResponse = true)
        {
            KeepAlive = keepAlive;
            ExpectResponse = expectResponse;
            //Prepend message with indicative chars (for device to interpret as abvoe) if relevant flags are true
            if (keepAlive)
                msgOut = Info.KeppAliveChar + msgOut;
            if (expectResponse)
                msgOut = Info.RespondChar + msgOut;
            return msgOut;
        }

    }
}
