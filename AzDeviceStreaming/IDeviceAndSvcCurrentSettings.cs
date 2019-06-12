namespace AzIoTHubDeviceStreams
{
    public interface IDeviceAndSvcCurrentSettings
    {
        bool KeepAlive { get; set; }
        bool ResponseExpected { get; set; }

        string ProcessMsgIn(string msgIn);
        string ProcessMsgOut(string msgOut, bool keepAlive = false, bool responseExpected = true);
    }
}