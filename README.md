## Azure IoT Hub Device Streaming
A set of projects examining refactoring of the Azure IoTHub SDK Device Streaming Echo sample functionality as .Net Core and UWP reusable libraries. The aim is to develop a library taht implements the device and service functionality of IoT Hub Device Streaming that can be used in UWP apps.

# We now have a solution to issues raised here
> Update Microsoft.Azure.Amqp to the latest version (2.4.2)
> You do this by adding it, using Nuget, to the project
> This was mentioned towards the end of the issue on GitHub [azure-iot-sdk-csharp/issues/784](https://github.com/Azure/azure-iot-sdk-csharp/issues/784)

_**This is now Version 1 of the repository. With the solution implemented here the second version removes some of the test projects and builds upon the repository's functionality, uncluding only having .Net Stanard libraries as they can be used in both .Net Core apps and UWP apps.**_ <br>Version 2 of the repository in here: [djaus2/AziothubDeviceStreaming-Master](https://github.com/djaus2/AziothubDeviceStreaming)

<hr>

# Background
Azure IoT Hub Device Streaming, although in Preview, is a cool technology. It enables an IoT device to receive messages from another system (the service: for example a user app) and for device to respond by sending back a message to the service. An Azure IoT Hub acts as the intermediary in the communications. No modules are needed to be installed in the hub. The functionality is implemented by calls to the IoT Hub SDK by both the device and service apps.

_I’ve developed a GitHub C# Library project for UWP Sockets:  [djaus2/SocketsUWP](https://github.com/djaus2/SocketsUWP). 
With this repository, I am attempting to use Azure IoTHub as the conduit in another project that mimics the socket stream functionality.

# The GitHub Repository
![Solution Explorer](https://github.com/djaus2/AziothubDeviceStreaming/blob/master/images/Capture001.PNG)<br>
**The repository projects.** <br>
There are three sets of projects, as per the class library projects they use:
- .Net Core
- .Net Standard
- UWP (.Net Framework)
There is also some crossing of boundaries. For eaxample, there isa a UWP apps that uses the.Net Standard version of the libraries.

The original Sample Azure IoTHub Device Streaming apps are included as standalone .NetCore apps. There is some slight modification of the given code but it essentially as supplied on GitHub (except for two changes as noted below.))

 There are two types of libraries, built for each of the SDK Framework types:
- The Device Streaming Functionality: Encap[sulted from the sample apps.
- The Connections: The Hub conenction strings etc. (_Nb: In the repository left blank)_

All common code is located in a folder common off the Solution folder. Each of the libraries use the same source code form here; just built for a different SDK.

_The device functionality with the SDK's sample DeviceStreaming apps is to just echo back the message received. In this suite of apps, the device uppercases the received string and sends that back to the service. This functionality is implemented in the AzDeviceStreamingLib class libraries._

# Status
The .Net Core library implements Azure IoT Hub connectivity for the Device and Service sample console apps. These both function correctly ( that is as per the supplied examples) with the functionality moved to the library. Originally it was found that when the app is a UWP app though, whether as an XAML or Console app, the service references work OK but the device references did not. This was true whether the library is implemented as a .Net Core class library or as a UWP class library.

> [!NOTE]
> Later on it was found that the UWP apps work at both ends when the Device Transport is MQTT instead of the default AMQPP transport which does not work with UWP.

When the connection fails, it is the Device that fails to connect to the IoTHub:
```
Exception thrown: 'Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException' in System.Private.CoreLib.dll
1 Error RunDeviceAsync(): Hub connection failure
Exception thrown: 'Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException' in System.Private.CoreLib.dll
2 Error RunSvcAsync(): Device not found
```

**For this suite of projects I am attempting the refactor the Echo Example into a library that can be used in UWP apps.**

...<br>
[Read more of my blog](https://davidjones.sportronics.com.au/azure/Azure-IoT-Hub-Device-Streaming-azure.html)

