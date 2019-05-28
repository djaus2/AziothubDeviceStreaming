using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using AzIoTHubDeviceStreams;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace BGAppAzDeviceStream_Device
{
    public sealed class StartupTask : IBackgroundTask
    {
        string service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
        string device_id = AzureConnections.MyConnections.DeviceId;
        string device_cs = AzureConnections.MyConnections.DeviceConnectionString;

        public void Run(IBackgroundTaskInstance taskInstance)
        {


            RunDevice(device_cs, 1000000).GetAwaiter().GetResult(); ;

            Console.WriteLine("Device Done.\n");
            Console.ReadLine();
            return;
        }

        private  string OnrecvTextIO(string msgIn)
        {
            Console.WriteLine(msgIn);
            string msgOut = msgIn.ToUpper();
            Console.WriteLine(msgOut);
            return msgOut;
        }

        private  async Task RunDevice(string device_cs, double ts)
        {
            DeviceStreamingCommon._Timeout = TimeSpan.FromMilliseconds(ts);
            try
            {
                //va
                await Task.Run(() =>
                {
                    try
                    {
                        DeviceStreamClient.RunClient(device_cs, OnrecvTextIO);
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
                });
            }
            //catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
            //{
            //    System.Diagnostics.Debug.WriteLine("00 Error App.RunClient(): Hub connection failure");
            //}
            //catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            //{
            //    System.Diagnostics.Debug.WriteLine("00 Error App.RunClient(): Device not found");
            //}
            catch (TaskCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error App.RunClient(): Task canceled");
            }
            catch (OperationCanceledException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error App.RunClient(): Operation canceled");
            }
            catch (Exception ex)
            {
                if (ex.Message.Contains("Timeout"))
                    System.Diagnostics.Debug.WriteLine("00 Error App.RunClient(): " + ex.Message);
                else
                {
                    System.Diagnostics.Debug.WriteLine("00 Error App.RunClient(): Timeout");
                }
            }
        }
    }
}
