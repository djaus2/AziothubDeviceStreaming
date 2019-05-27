# AziothubDeviceStreaming
A set of projects examining refactoring of the Azure IoTHub SDK Device Streaming Echo sample functionality as .Net Core and UWP reusable libraries. The aim is to develop a library taht implements the device and service functionality of IoT Hub DEvice Streaming that can be used in UWP apps.

# NB: Code coming tomorrow

# Status
The .Net Core library implements Azure IoT Hub connectivity for the Device and Service sample console apps. These both work with the functionality moved to the library. When the app is a UWP app though, whether as an XAML or Console app, the service reference works OK but the device references does not. This is true whether the library is implemeneted as a .Net Core class library or as a UWP class library.

# Background
Azure IoT Hub Device Streaming, although in Preview, is a cool technology.

I’ve developed a GitHub Library project for UWP Sockets:  https://github.com/djaus2/SocketsUWP
I am attempting to using Azure IoTHub as the conduit in another project AzureSocketsUWP.
For that I have been looking at DeviceStreaming:
https://docs.microsoft.com/en-us/azure/iot-hub/quickstart-device-streams-echo-csharp
and
https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-device-streams-overview

For my new project I am attempting the refactor the Echo Example 
https://github.com/Azure-Samples/azure-iot-samples-csharp/tree/master/iot-hub/Quickstarts/device-streams-echo
into a library that can be used in UWP apps.

