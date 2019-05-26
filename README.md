# AzIoTHubDeviceStreaming
A set of projects examining refactoring of the Azure IoTHub SDK Device Streaming Echo sample functionality as a .Net Core and as a UWP reusable library. The aim is to develop a library taht implements the device and service functionality of IoT Hub DEvice Streaming that can be used in UWP apps.

# NB: Code coming tomorrow

# Status
The .Net Core library implements Azure IoT Hub connectivity for the Device and Service sample console apps. These both work with the functionality moved to the library. When the app is a UWP app though, whether as an XAML or Console app, the service reference works OK but the device references does not. This is true whether the library is implemeneted as a .Net Core class library or as a UWP class library.

# Background

