using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Http;
using Windows.ApplicationModel.Background;
using AzDeviceStreamsUWPLib;
using System.Threading.Tasks;

// The Background Application template is documented at http://go.microsoft.com/fwlink/?LinkID=533884&clcid=0x409

namespace BGAppAzDeviceStream_Svc
{
    public sealed class StartupTask : IBackgroundTask
    {
        public void Run(IBackgroundTaskInstance taskInstance)
        {
            // 
            // TODO: Insert code to perform background work
            //
            // If you start any asynchronous methods here, prevent the task
            // from closing prematurely by using BackgroundTaskDeferral as
            // described in http://aka.ms/backgroundtaskdeferral
            //
        }

        private  void OnrecvText(string msg)
        {
            Console.WriteLine(msg);
        }

        private  async Task DoIt(string servvicecs, string devid, string msgOut, double ts)
        {

            DeviceStreamingCommon._Timeout = TimeSpan.FromMilliseconds(ts);
            try
            {
                //var svc =
                await Task.Run(() =>
                {
                    try
                    {

                        DeviceStreamSvc.RunSvc(servvicecs, devid, msgOut, OnrecvText);

                    }
                    //catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
                    //{
                    //    System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Hub connection failure");
                    //}
                    //catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
                    //{
                    //    System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Device not found");
                    //}
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
                        if (!ex.Message.Contains("Timeout"))
                            System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): " + ex.Message);
                        else
                        {
                            System.Diagnostics.Debug.WriteLine("0 Error App.RunSvc(): Timeout");
                        }
                    }
                });

            }
            //catch (Microsoft.Azure.Devices.Client.Exceptions.IotHubCommunicationException)
            //{
            //    System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Hub connection failure");
            //}
            //catch (Microsoft.Azure.Devices.Common.Exceptions.DeviceNotFoundException)
            //{
            //    System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Device not found");
            //}
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
                if (!ex.Message.Contains("Timeout"))
                    System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): " + ex.Message);
                else
                {
                    System.Diagnostics.Debug.WriteLine("00 Error App.RunSvc(): Timeout");
                }
            }

        }
    }
}
