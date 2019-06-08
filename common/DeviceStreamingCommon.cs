// Copyright (c) Microsoft. All rights reserved.
// Licensed under the MIT license. See LICENSE file in the project root for full license information.

using Microsoft.Azure.Devices;
using System;
using System.Net.WebSockets;
using System.Threading;
using System.Threading.Tasks;


namespace AzIoTHubDeviceStreams
{
    public delegate bool KeepConnectionAlive();
    public delegate bool RespondToServer();
    public delegate bool ExpectResponseFromDevice();
    public delegate void ActionReceivedText(string recvTxt);

    public delegate string ActionReceivedTextIO(string recvTxt);
    public static class DeviceStreamingCommon
    {
        public static TransportType s_transportType = TransportType.Amqp;

        public static Microsoft.Azure.Devices.Client.TransportType device_transportType = Microsoft.Azure.Devices.Client.TransportType.Mqtt;

        public static TimeSpan _Timeout = TimeSpan.FromMilliseconds(1000);

        /// <summary>
        /// Creates a ClientWebSocket with the proper authorization header for Device Streaming.
        /// </summary>
        /// <param name="url">Url to the Streaming Gateway.</param>
        /// <param name="authorizationToken">Authorization token to connect to the Streaming Gateway.</param>
        /// <param name="cancellationToken">The token used for cancelling this operation if desired.</param>
        /// <returns>A ClientWebSocket instance connected to the Device Streaming gateway, if successful.</returns>
        public static async Task<ClientWebSocket> GetStreamingClientAsync(Uri url, string authorizationToken, CancellationToken cancellationToken)
        {

            ClientWebSocket wsClient = new ClientWebSocket();
            wsClient.Options.SetRequestHeader("Authorization", "Bearer " + authorizationToken);

            await wsClient.ConnectAsync(url, cancellationToken).ConfigureAwait(false);

            return wsClient;
        }
    }
}
