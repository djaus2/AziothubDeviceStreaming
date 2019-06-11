using AzIoTHubDeviceStreams;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace UWPXamlApp
{
    sealed partial class MainPage : Page
    {
        private string OnDeviceRecvText(string msgIn)
        {
            string msgOut = msgIn.ToUpper();
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tbDeviceMsgIn.Text = msgIn;
                    tbDeiceMsgOut.Text = msgOut;
                });
            });
            return msgOut;
        }

        private void ButtonCanceLDevice_Click(object sender, RoutedEventArgs e)
        {
            DeviceStream_Device.deviceStream_Device?.Cancel();
        }

        private async void Button_Click_Device(object sender, RoutedEventArgs e)
        {
            double to;
            if (double.TryParse(tbDeviceTimeout.Text, out to))
                DeviceStreamingCommon._Timeout = TimeSpan.FromMilliseconds(to);
            try
            {
                //va
                await Task.Run(() =>
                {
                    try
                    {
                        DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvText).GetAwaiter().GetResult();
                    }
                    catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                    {
                        System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Hub connection failure");
                    }
                    catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                    {
                        System.Diagnostics.Debug.WriteLine("0 Error App.RunClient(): Device not found");
                    }
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
            catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error App.RunClient(): Hub connection failure");
            }
            catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            {
                System.Diagnostics.Debug.WriteLine("00 Error App.RunClient(): Device not found");
            }
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
