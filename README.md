# Azure IoT Hub Device Streaming
A set of projects examining refactoring of the Azure IoTHub SDK Device Streaming Echo sample functionality as .Net Core and UWP reusable libraries. The aim is to develop a library taht implements the device and service functionality of IoT Hub Device Streaming that can be used in UWP apps.

# NB: Code coming tomorrow

# Status
The .Net Core library implements Azure IoT Hub connectivity for the Device and Service sample console apps. These both function correctly ( that is as per the supplied examples) with the functionality moved to the library. When the app is a UWP app though, whether as an XAML or Console app, the service references work OK but the device references does not. This is true whether the library is implemeneted as a .Net Core class library or as a UWP class library.

# Background
Azure IoT Hub Device Streaming, although in Preview, is a cool technology. It enables an IoT device to receive messages from another system (the service: for example a user app) and for device to respond by sending back a message to the service. An Azure IoT Hub acts as the intermediatary in the communinications. No modules are needed to be instaled in the hub. The functionality is implemented by calls to the IoT Hub SDK by both the device and service apps.

The device functionality with the SDK's sample DeviceStreaming app is to just echo back the message received. 

_I’ve developed a GitHub C# Library project for UWP Sockets:  [djaus2/SocketsUWP](https://github.com/djaus2/SocketsUWP). 
With this repository, I am attempting to use Azure IoTHub as the conduit in another project that mimics the socket stream functionality.The functionality of the device in this suite of projects is to uppercase the received message._

# Links
- The repository for this project:  [djaus2/AziothubDeviceStreaming](https://github.com/djaus2/AziothubDeviceStreaming) (_this_)
- The Az IoTHub SDK Device streaming Echo qucikstart sample
  - [How to tutorial](https://docs.microsoft.com/en-us/azure/iot-hub/quickstart-device-streams-echo-csharp)
  - [Code](https://github.com/Azure-Samples/azure-iot-samples-csharp/archive/master.zip)
    - Unzip and navigate from the unzip folder to /azure-iot-samples-csharp-master/iot-hub/Quickstarts/device-streams-echo/
- [Az IoT Hub Device Streams Overview](https://docs.microsoft.com/en-us/azure/iot-hub/iot-hub-device-streams-overview)
- There is also a Device Streaming Proxy example (not discussed here, _yet_) where an arbitrary port on the client side can be funneled to the IoTHub using SSLs
  - [How to tutorial](https://docs.microsoft.com/en-us/azure/iot-hub/quickstart-device-streams-proxy-csharp)
  - [Code](https://github.com/Azure-Samples/azure-iot-samples-csharp/archive/master.zip)
    - Unzip and navigate from the unzip folder to /azure-iot-samples-csharp-master/iot-hub/Quickstarts/device-streams-proxy/

**For this suite of projects I am attempting the refactor the Echo Example into a library that can be used in UWP apps.**

# The Projects
There are .NetCore and UWP projects. The .NetCore libraries are for the .NetCore apps. 
Similarly, UWP libraries are for the UWP apps

## .Net Core Projects
- Libraries
	- AzDeviceStreamsDNCore
	- AzureConnectionsDNCore
- Apps
	- DeviceClientStreamingSample
	- ServiceClientStreamingSample

## UWP Projects
- Libraries
	- AzDeviceStreamsUWPLib
	- AzureConnectionsUWP
- Apps
	- UWPXamlApp
	- UWPConsoleDevicetApp
	- UWPConsoleSvcApp
	- BGAppAzDeviceStream_Device
	- BGAppAzDeviceStreamSvc2

With each target there is a Connection class which contains the IoT Hub connectivity settings:
- IoT Hub Connection string
- IoT Hub Device Connection string
- IoT Hub Device Id (Eg "MyDevice")
You can then set up one Az IoT Hub set one device for it, than can then be used throughout all projects.

The other class library for each target contains the Azure IoTHub SDK functioanlity.

## .NetCore Apps
To run in .NetCore mode set the target to AnyCPU, whereas for the UWP mode set the target for the target systems CPU.

The apps for the .NetCore target are exactly the same as the device and server apps in the Echo Quickstart except that:
- The IoT Hub functionality is removed and encapsulated in the IoTHub clas library
- A callback delagate is used to pipe received messages to the app.
- The app calls the library's API to send a message back
- The device functionality echoes back teh string to the service in uppercase
Once the IoT Hub is setup you can run both apps simullataneously on the the same system.

## UWP Apps
To run in UWP mode set the  target for the target system's CPU (x86,x64 etc).

The UWP class library is an exact copy of the .NetCore library, but configured as a UWP class library.
The UWP apps do work if they reference the .NetCore library instead but yoy have manually set the reference to to a specific DLL via browsing rather than refering to an included project in the solution. You may (??) need to also include some of the Nuget packages in the app tha the library uses. 

- UWPXamlApp<br>
Implements both the Device and Service functionality
- UWPConsoleDevicetApp<br>
Implements the Device functionality as a UWP console app.
- UWPConsoleSvcApp<br>
Implements the Device functionality as a UWP console app.
- BGAppAzDeviceStream_Device<br>
Implements the Device functionality as a UWP Background Task. Runs only on IoT-Core
- BGAppAzDeviceStreamSvc2<br>
Implements the Service functionality as a UWP Background Task. Runs only on IoT-Core

# Testing
- Setup and Azure IoT Hub and add a device to it as per the [QuciskStart]((https://docs.microsoft.com/en-us/azure/iot-hub/quickstart-device-streams-echo-csharp))
- Get the Connection strings (both) and the DeviceId
- Edit the AzureConnections classes
- Set the Target and build
- Run any pair of apps (Device and Stream) .Nb the UWPXamlApp can perform both ends

Note that if run from Visual Studio, there are debug messages.

## Outcomes Thus Far
**_As per Staus above, it is found that the Device when run as a UWP app does not connect._**

Ideas welcome, thx