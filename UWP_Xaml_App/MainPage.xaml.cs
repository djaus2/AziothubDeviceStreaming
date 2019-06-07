using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using AzIoTHubDeviceStreams;
using System.Threading.Tasks;
using Windows.UI.Core;

// The Blank Page item template is documented at https://go.microsoft.com/fwlink/?LinkId=402352&clcid=0x409

namespace UWPXamlApp
{
    public class EnumToStringConverter : IValueConverter
    {
        //public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    return "";
        //}

        //public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        //{
        //    throw new NotImplementedException();
        //}

        object IValueConverter.Convert(object value, Type targetType, object parameter, string language)
        {
            if (value is Microsoft.Azure.Devices.Client.TransportType)
            {
                Microsoft.Azure.Devices.Client.TransportType trans = (Microsoft.Azure.Devices.Client.TransportType)value;
                return trans.ToString();
            }
            else
                return "unknown";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotImplementedException();
        }
    }
    public sealed partial class MainPage : Page
    {
        string service_cs = AzureConnections.MyConnections.IoTHubConnectionString;
        string device_id = AzureConnections.MyConnections.DeviceId;
        string device_cs = AzureConnections.MyConnections.DeviceConnectionString;

        AzureConnections.DeviceCurrentSettings deviceSettings = null;

        public  List<Microsoft.Azure.Devices.Client.TransportType> ListEnum { get { return typeof(Microsoft.Azure.Devices.Client.TransportType).GetEnumValues().Cast<Microsoft.Azure.Devices.Client.TransportType>().ToList(); } }

        public MainPage()
        {
            this.InitializeComponent();
            deviceSettings = new AzureConnections.DeviceCurrentSettings();
        }

        private void OnrecvText(string recvTxt)
        {
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tbSvcMsgIn.Text = recvTxt;
                });
            });
        }

        private string OnDeviceRecvText(string msgIn)
        {
            //A simple implmentation of settings. Device calls GetKeepAlive() and GetRespond() to get these.
            deviceSettings.SetKeepAlive(msgIn.ToLower().Contains('k'));
            deviceSettings.SetRespond(msgIn.ToLower().Contains('r'));

            string msgOut =msgIn.ToUpper();
            Task.Run(async () => {
                await Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
                {
                    tbDeviceMsgIn.Text = msgIn;
                    tbDeiceMsgOut.Text = msgOut;
                });
            });
            return msgOut;
        }

        private async void Button_Click_Svc(object sender, RoutedEventArgs e)
        {
            double to;
            if (double.TryParse(tbSvcTimeout.Text, out to))
                DeviceStreamingCommon._Timeout = TimeSpan.FromMilliseconds(to);
            string msgOut = tbSvcMsgOut.Text;
            try
                {
                    await Task.Run(() =>
                    {
                        try
                        {

                            DeviceStream_Svc.RunSvc(service_cs, device_id, msgOut, OnrecvText).GetAwaiter().GetResult();

                        }
                        catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                        {
                            System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Hub connection failure");
                        }
                        catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                        {
                            System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Device not found");
                        }
                        catch (TaskCanceledException)
                        {
                            System.Diagnostics.Debug.WriteLine("0Error App.RunSvc(): Task canceled");
                        }
                        catch (OperationCanceledException)
                        {
                            System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Operation canceled");
                        }
                        catch (Exception ex)
                        {
                            if (ex.Message.Contains("Timeout"))
                                System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): " + ex.Message);
                            else
                            {
                                System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Timeout");
                            }
                        }
                    });

                }
                catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                {
                    System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Hub connection failure");
                }
                catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                {
                    System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Device not found");
                }
                catch (TaskCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("00 Error RunSvc(): Task canceled");
                }
                catch (OperationCanceledException)
                {
                    System.Diagnostics.Debug.WriteLine("00 Error RunSvc(): Operation canceled");
                }
                catch (Exception ex)
                {
                    if (ex.Message.Contains("Timeout"))
                        System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): " + ex.Message);
                    else
                    {
                        System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Timeout");
                    }
                }
            
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
                        DeviceStream_Device.RunDevice(device_cs, OnDeviceRecvText, deviceSettings.GetKeepAlive, deviceSettings.GetRespond).GetAwaiter().GetResult();
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

        private void Page_Loaded(object sender, RoutedEventArgs e)
        {
            ListviewTransports.ItemsSource = ListEnum;
            ListviewTransports.SelectedItem = AzIoTHubDeviceStreams.DeviceStreamingCommon.device_transportType;
            ListviewTransports.ScrollIntoView(ListviewTransports.SelectedItem);
        }

        private void ListviewTransports_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ListviewTransports.SelectedIndex != -1)
            {
                AzIoTHubDeviceStreams.DeviceStreamingCommon.device_transportType = (Microsoft.Azure.Devices.Client.TransportType) ListviewTransports.SelectedItem;
                System.Diagnostics.Debug.WriteLine(AzIoTHubDeviceStreams.DeviceStreamingCommon.device_transportType);
            }
        }

        private void Page_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            GeneralTransform gt = tbSvcMsgOut.TransformToVisual(this);
            Point offset = gt.TransformPoint(new Point(0, 0));
            double controlTop = offset.Y;
            double controlLeft = offset.X;
            double newWidth =  e.NewSize.Width - controlLeft - 20;
            if (newWidth > tbSvcMsgOut.MinWidth)
            {
                tbSvcMsgOut.Width = newWidth;
                tbDeviceMsgIn.Width = newWidth;
                tbSvcMsgIn.Width = newWidth;
                tbDeiceMsgOut.Width = newWidth;
            }
        }
    }
}
