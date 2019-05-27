# AziothubDeviceStreaming
A set of projects examining refactoring of the Azure IoTHub SDK Device Streaming Echo sample functionality as .Net Core and UWP reusable libraries. The aim is to develop a library taht implements the device and service functionality of IoT Hub Device Streaming that can be used in UWP apps.

# NB: Code coming tomorrow

# Status
The .Net Core library implements Azure IoT Hub connectivity for the Device and Service sample console apps. These both work with the functionality moved to the library. When the app is a UWP app though, whether as an XAML or Console app, the service reference works OK but the device references does not. This is true whether the library is implemeneted as a .Net Core class library or as a UWP class library.

# Background
Azure IoT Hub Device Streaming, although in Preview, is a cool technology. It enables an IoT device to receive messages from another system (the service: for example a user app) and for device to respond by sending back a message to the service. An Azure IoT Hub acts as the intermediatary in the communinications. No modules are needed to be instaled in the hub. The functionality is implemented by calls to the IoT Hub SDK by both the device and service apps.

The device functionality with the SDK's sample DeviceStreaming app is to just echo back the message received. 

I’ve developed a GitHub Library project for UWP Sockets:  (djaus2/SocketsUWP)[https://github.com/djaus2/SocketsUWP]
I am attempting to use Azure IoTHub as the conduit in another project that mimics the socket stream functionality.The functionality of the device in this suite of projects is to uppercase the received message.

# Links
- The repository for this project:  [djaus2/AziothubDeviceStreaming](https://github.com/djaus2/AziothubDeviceStreaming)
- The [Az IoTHub SDK Device streaming Echo qucikstart sample](https://docs.microsoft.com/en-us/azure/iot-hub/quickstart-device-streams-echo-csharp)
- [Az IoT Hub Device Streams Overview](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-device-streams-overview)

For my new project I am attempting the refactor the Echo Example into a library that can be used in UWP apps.

