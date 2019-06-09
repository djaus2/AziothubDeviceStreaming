# Azure IoT Hub Device Streaming
In the previous version of this repository, some issues were raised wrt refactoring of the Azure IoTHub SDK Device Streaming Echo sample functionality as .Net Core and UWP reusable libraries. These issues have been resolved and the library is now presented as a .Net Standard library that can be used in .Net Core, UWP and Xamarin apps (last yet to be tested). The library implements the device and service functionality of IoT Hub Device Streaming that can be used in UWP and other types apps.
<!--more-->

My blog on this: [djaus2/AziothubDeviceStreaming](https://davidjones.sportronics.com.au/2020-06-09-Azure-IoT-Hub-Device-Streaming-A-Libray-azure.html)<br>
Link to [the previous version of the repository](https://github.com/djaus2/AziothubDeviceStreaming/tree/master_original)<br>
Link to [the blog on the previous version of this repository.](https://davidjones.sportronics.com.au//Azure-IoT-Hub-Device-Streaming-azure.html)<br>

# We have a solution!
The main problem, as discussed previously, was that the developed Device Streaming library, regardless of the SDK .Net Framework used (UWP, .Net Core or .Net Standard), did not support the AMQP transport (Device-IoTHub) when used with a UWP app. It was determined that the Nuget installation of Microsoft.Azure.Device.Client (Device Streaming requires the latest preview version) implicitly installed an earlier version of Microsoft.Azure.Ampq 2.3.7, whereas version 2.4.2 was required. This was resolved by explicitly installing Microsoft.Azure.Amqp version (2.4.2) using Nuget.

# Background
Azure IoT Hub Device Streaming, although in Preview, is a cool technology. It enables an IoT device app to receive messages from an app on another system (the service: for example a user app) and for device to respond by sending back a message to the service. An Azure IoT Hub acts as the intermediary in the communications. No modules are needed to be installed in the hub. The functionality is implemented by calls to the IoT Hub SDK by both the device and service apps.

I’ve developed a GitHub C# Library project for UWP Sockets:  [djaus2/SocketsUWP](https://github.com/djaus2/SocketsUWP). 
With _THIS_ new repository, I am attempting to use Azure IoTHub as the conduit in new library that mimics the socket stream functionality.

Whereas the previous version of the repository, implemented all three types of .Net libraries, the new version only implements the .Net Standard version as this can be used universally. The various types of test apps remain in the repository. The UWP app have been significantly extended to test some added features of the library:

- The original communications were single shot. The service create a socket send a message waits for a reply then closes the socket. The device listens for a connection, create a socket, reads teh message, processes it and sends it back. It then closes its socket. Whilst this is the default behaviour with the new version of the library, there is a Keep Alive option that keeps the sockets open at both ends, subject to timeout, until closed. This option is dictated by the Service end.
- The original device processing required a response to be sent back. For this repository the received message was uppercased. (The SDK Echo Sample app just sent back the received message). This is now optional, and is dictated by the Service end.
- Whilst not 100% debugged, there is now an option to cancel the socket at either end.

**Overall,the object here is to to create a conceptually high level library that encapsulates the Azure IoTHub Device Streaming functionality which is simple to use, but contains inbuilt extensibility _(read options)_ that can be exploited in host apps without reconstruction of the library.**
...<br>
[Read more of my blog](https://davidjones.sportronics.com.au/azure/Azure-IoT-Hub-Device-Streaming-azure.html)

