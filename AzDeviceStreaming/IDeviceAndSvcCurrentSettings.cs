namespace AzIoTHubDeviceStreams
{
    public interface IDeviceAndSvcCurrentSettings
    {
        bool AutoStartDevice { get; set; }
        bool KeepAlive { get; set; }
        bool KeepDeviceListening { get; set; }
        bool ResponseExpected { get; set; }

        string ProcessMsgIn(string msgIn);
        string ProcessMsgOut(string msgOut, bool keepAlive = false, bool responseExpected = true);
    }
}