using System;
using System.Collections.Generic;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using Microsoft.Azure.Devices.Client;
using Microsoft.Azure.EventHubs;
using Newtonsoft.Json;

namespace AzIoTHubModules
{
    // A Simple Class to pass Microsoft.Azure.Devices.Client.Message properties
    public class IoTMessage
    {
        public string MessageId { get; private set; }

        /*
         * 
         MessageString = JsonConvert.SerializeObject(telemetryDataPoint);
                [NonSerialized]
                Message = new Message(Encoding.ASCII.GetBytes(MessageString));
                var qwe = Message.GetBytes();
                string MessageString2= Encoding.UTF8.GetString(qwe, 0, qwe.Length);
         */
        public string MessageAsString { get; set; }

        [NonSerialized]
        public byte[] bytes;

        public List<Tuple<string,string>> Properties { get; set; }
  
        public string UserId { get; private set; }
        public uint DeliveryCount { get; private set; }
        public DateTime CreationTimeUtc { get; private set; }
        public string CorrelationId { get; private set; }
        public string ContentType { get; private set; }
        public string ContentEncoding { get; private set; }
        public string To { get; private set; }
        public string MessageSchema { get; private set; }

        public IoTMessage()
        {

        }

        public IoTMessage(Message message)
        {
            try
            {
                MessageId = message.MessageId;
                bytes = message.GetBytes();
                MessageAsString = Encoding.UTF8.GetString(bytes, 0, bytes.Length);
                Properties = new List<Tuple<string, string>>();

                foreach (var prop in message.Properties)
                {
                    Properties.Add(new Tuple<string, string>(prop.Key, prop.Value));
                }
                UserId = message.UserId;
                CreationTimeUtc = message.CreationTimeUtc;
                CorrelationId = message.CorrelationId;
                ContentType = message.ContentType;
                ContentEncoding = message.ContentEncoding;
                To = message.To;
                MessageSchema = message.MessageSchema;
            }
            catch (Exception)
            {
            }
        }

        public Message ToMessage()
        {
            try
            {
                Message message = new Message(Encoding.ASCII.GetBytes(MessageAsString));
                message.MessageId = MessageId;
                foreach (var prop in Properties)
                    message.Properties.Add(prop.Item1, prop.Item2);
                message.UserId = UserId;
                message.CreationTimeUtc = CreationTimeUtc;
                message.CorrelationId = CorrelationId;
                message.ContentType = ContentType;
                message.ContentEncoding = ContentEncoding;
                message.To = To;
                message.MessageSchema = MessageSchema;
                return message;
            }
            catch (Exception )
            {
                return null;
            }
        }

        public EventData ToEventData()
        {
            try
            {
                Message message = this.ToMessage();
                EventData eventData = new EventData(message.GetBytes());
                foreach (var prop in message.Properties)
                    eventData.Properties.Add(prop.Key, prop.Value);
                return eventData;
            } catch (Exception)
            {
                return null;
            }
        }

        public string Serialise()
        {
            return JsonConvert.SerializeObject(this);
            //var serializer = new XmlSerializer(this.GetType());
            //using (var writer = XmlWriter.Create("message.xml"))
            //{
            //    serializer.Serialize(writer, this);
            //}
        }

        public static IoTMessage Deserialsie(string msg)
        {
            try
            {
                object obj = JsonConvert.DeserializeObject(msg);
                IoTMessage iMsg = JsonConvert.DeserializeObject<IoTMessage>(msg);
                //iMsg.Properties = new List<string, string>();

                return (IoTMessage)iMsg;
            }
            catch (Exception)
            {
                return null;
            }

        }


    }
}
