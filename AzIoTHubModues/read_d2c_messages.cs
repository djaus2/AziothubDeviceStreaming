// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

// This application uses the Microsoft Azure Event Hubs Client for .NET
// For samples see: https://github.com/Azure/azure-event-hubs/tree/master/samples/DotNet
// For documenation see: https://docs.microsoft.com/azure/event-hubs/
using System;
using Microsoft.Azure.EventHubs;
using System.Threading.Tasks;
using System.Threading;
using System.Text;
using System.Collections.Generic;

namespace read_d2c_messages
{
    public class ReadDeviceToCloudMessages
    {
        public delegate void ActionReceivedText(string recvTxt);
        // Event Hub-compatible endpoint
        // az iot hub show --query properties.eventHubEndpoints.events.endpoint --name {your IoT Hub name}
        //private readonly static string s_eventHubsCompatibleEndpoint = "sb://ihsuproddmres016dednamespace.servicebus.windows.net/";

        //// Event Hub-compatible name
        //// az iot hub show --query properties.eventHubEndpoints.events.path --name {your IoT Hub name}
        //private readonly static string s_eventHubsCompatiblePath = "iothub-ehub-mynewhub-1918909-a3ba8a9102";

        //// az iot hub policy show --name service --query primaryKey --hub-name {your IoT Hub name}
        //private readonly static string s_iotHubSasKey = "Ek6Mw8PvsQQV8pdJZj+LxALA0pGB+9f0rorJKDrjfoU=";
        //private readonly static string s_iotHubSasKeyName = "service";
        private static EventHubClient s_eventHubClient;

        // Asynchronously create a PartitionReceiver for a partition and then start 
        // reading any messages sent from the simulated client.
        private static async Task ReceiveMessagesFromDeviceAsync(string partition, CancellationToken ct)
        {
            // Create the receiver using the default consumer group.
            // For the purposes of this sample, read only messages sent since 
            // the time the receiver is created. Typically, you don't want to skip any messages.
            var eventHubReceiver = s_eventHubClient.CreateReceiver("$Default", partition, EventPosition.FromEnqueuedTime(DateTime.Now));
            System.Diagnostics.Debug.WriteLine("Create receiver on partition: " + partition);
            while (true)
            {
                if (ct.IsCancellationRequested) break;
                System.Diagnostics.Debug.WriteLine("Listening for messages on: " + partition);
                // Check for EventData - this methods times out if there is nothing to retrieve.
                var events = await eventHubReceiver.ReceiveAsync(100);

                // If there is data in the batch, process it.
                if (events == null) continue;

                foreach (EventData eventData in events)
                {
                    string data = Encoding.UTF8.GetString(eventData.Body.Array);
                    System.Diagnostics.Debug.WriteLine("Message received on partition {0}:", partition);
                    System.Diagnostics.Debug.WriteLine("  {0}:", data);
                    System.Diagnostics.Debug.WriteLine("Application properties (set by device):");


                    foreach (var prop in eventData.Properties)
                    {
                        System.Diagnostics.Debug.WriteLine("  {0}: {1}", prop.Key, prop.Value);
                    }
                    System.Diagnostics.Debug.WriteLine("System properties (set by IoT Hub):");
                    foreach (var prop in eventData.SystemProperties)
                    {
                        System.Diagnostics.Debug.WriteLine("  {0}: {1}", prop.Key, prop.Value);
                    }

                    OnDeviceStatusUpdateD?.Invoke(AzIoTHubModules.SyntheticIoTMessage.EventData_ToString(eventData));
                }
            }
        }

        private static ActionReceivedText OnDeviceStatusUpdateD = null;

        public static async Task Run( ActionReceivedText onDeviceStatusUpdateD = null)
        {
            OnDeviceStatusUpdateD = onDeviceStatusUpdateD;
            
            System.Diagnostics.Debug.WriteLine("IoT Hub Quickstarts - Read device to cloud messages.\n");

            // Create an EventHubClient instance to connect to the
            // IoT Hub Event Hubs-compatible endpoint.
            //var connectionString1 = new EventHubsConnectionStringBuilder(new Uri(s_eventHubsCompatibleEndpoint), s_eventHubsCompatiblePath, s_iotHubSasKeyName, s_iotHubSasKey);


            //Get some of event cs properties from hub cs
            var hubccs = AzureConnections.MyConnections.IoTHubConnectionString;
            string[] split = hubccs.Split(new char[] { ';' });
            string saskey = "";
            foreach (var xx in split)
            {
                string[] split2 = xx.Split(new char[] { '=' });
                if (split2[0].ToLower() == "SharedAccessKey".ToLower())
                {
                    saskey = split2[1];
                    //The second split mat have removed = from end of saskey
                    if (hubccs[hubccs.Length - 1] == '=')
                            saskey += "=";
                    break;
                }

            }
            string iotHubSasKeyName  = AzureConnections.MyConnections.IotHubKeyName;


            EventHubsConnectionStringBuilder EventHubConnectionString = null;

            if (AzureConnections.MyConnections.EHMethod1)
                EventHubConnectionString = new EventHubsConnectionStringBuilder(AzureConnections.MyConnections.EventHubsConnectionString);
            else
            EventHubConnectionString = new EventHubsConnectionStringBuilder(
                new Uri(AzureConnections.MyConnections.EventHubsCompatibleEndpoint),
                AzureConnections.MyConnections.EventHubsCompatiblePath,
                AzureConnections.MyConnections.IotHubKeyName,
                AzureConnections.MyConnections.EventHubsSasKey);


            s_eventHubClient = EventHubClient.CreateFromConnectionString(EventHubConnectionString.ToString());

            // Create a PartitionReciever for each partition on the hub.
            var runtimeInfo = await s_eventHubClient.GetRuntimeInformationAsync();
            var d2cPartitions = runtimeInfo.PartitionIds;
            CancellationTokenSource cts = new CancellationTokenSource();

            //System.Diagnostics.Debug.CancelKeyPress += (s, e) =>
            //{
            //    e.Cancel = true;
            //    cts.Cancel();
            //    System.Diagnostics.Debug.WriteLine("Exiting...");
            //};

            var tasks = new List<Task>();
            foreach (string partition in d2cPartitions)
            {
                tasks.Add(ReceiveMessagesFromDeviceAsync(partition, cts.Token));
            }

            // Wait for all the PartitionReceivers to finsih.
            Task.WaitAll(tasks.ToArray());
        }
    }
}
