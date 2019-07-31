using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using AzIoTHubDeviceStreams;
using System.Threading.Tasks;
using SimulatedDevice_ns;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace BGAppAzDeviceStream_Device
{
    public sealed class StartupTask : IBackgroundTask
    {
        private static int DeviceAction = AzureConnections.MyConnections.DeviceAction;

        private static bool basicMode = AzureConnections.MyConnections.basicMode;
        private static bool UseCustomClass = AzureConnections.MyConnections.UseCustomClass;
        private static bool ResponseExpected = AzureConnections.MyConnections.ResponseExpected;
        private static bool KeepAlive = AzureConnections.MyConnections.KeepAlive;

        private static string service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
        private static string device_id = AzureConnections.MyConnections.DeviceId;
        private static string device_cs = AzureConnections.MyConnections.DeviceConnectionString;

        private static bool KeepDeviceListening = AzureConnections.MyConnections.KeepDeviceListening;

        //The next is superfulous as this device app will always autostart.
        private static bool AutoStartDevice = AzureConnections.MyConnections.AutoStartDevice;

        public void Run(IBackgroundTaskInstance taskInstance)
        {
            Console.WriteLine("Device starting.");

            RunDevice(device_cs, 1000000);

            Console.WriteLine("Device Done.\n\nPress any key to finish.\n");
            //Console.ReadKey();
            return;
        }

        private  string OnrecvTextIO(string msgIn)
        {
            Console.WriteLine(msgIn);
            string msgOut = msgIn.ToUpper();
            Console.WriteLine(msgOut);
            return msgOut;
        }

        private string OnDeviceRecvTextIO(string msgIn, out Microsoft.Azure.Devices.Client.Message message)
        {
            message = null;
            //Perform device side processing here. Eg read sensors.
            string msgOut = msgIn;
            switch (DeviceAction)
            {
                case 0:
                    msgOut = msgIn;
                    break;
                case 1:
                    msgOut = msgIn.ToUpper();
                    break;
                case 2:
                    switch (msgIn.Substring(0, 3).ToLower())
                    {
                        case "tem":
                            msgOut = "45 C";
                            break;
                        case "pre":
                            msgOut = "1034.0 hPa";
                            break;
                        case "hum":
                            msgOut = "67%";
                            break;
                        default:
                            msgOut = "Invalid request";
                            break;
                    }
                    break;
                case 3:
                    msgOut = AzIoTHubDeviceStreams.DeviceStreamingCommon.DeiceInSimuatedDeviceModeStrn + SimulatedDevice_ns.SimulatedDevice.Run().GetAwaiter().GetResult();
                    message = SimulatedDevice.Message;
                    break;
                case 4:
                    msgOut = "Coming. Not yet implemented. This is a pace holder for now.";
                    break;
            }


            System.Diagnostics.Debug.WriteLine(msgIn);
            System.Diagnostics.Debug.WriteLine(msgOut);
            return msgOut;
        }

        private static void OnDeviceStatusUpdate(string recvTxt)
        {
            //AppendMsg += recvTxt +"\r\n";
            System.Diagnostics.Debug.WriteLine(recvTxt);
        }

        private static void ActionCommand(bool flag, string msg, int al, int cmd)
        {
            switch (cmd)
            {
                case 0:
                    //if (chkAutoStart.IsChecked != isChecked)
                    //    chkAutoStart.IsChecked = isChecked;
                    break;
                case 1:
                    //if (chKeepDeviceListening.IsChecked != isChecked)
                    //    chKeepDeviceListening.IsChecked = isChecked;
                    break;
            }
        }


        private void RunDevice(string device_cs, double ts)
        {
            DeviceStreamingCommon.DeviceTimeout = TimeSpan.FromMilliseconds(ts);

            try
            {

                if (basicMode)
                    DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO).GetAwaiter().GetResult();
                else if (!UseCustomClass)
                    DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO, OnDeviceStatusUpdate, ActionCommand, KeepDeviceListening).GetAwaiter().GetResult();
                else
                    DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvTextIO, OnDeviceStatusUpdate, ActionCommand, KeepDeviceListening, new DeviceSvcCurrentSettings_Example()).GetAwaiter().GetResult();
            }
            //catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
            //{
            //    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Hub connection failure");
            //}
            //catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            //{
            //    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Device not found");
            //}
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Task canceled");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Operation canceled");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Timeout"))
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): " + ex.Message);
                else
                {
                    System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Timeout");
                }
            }  
        }
    }
}
