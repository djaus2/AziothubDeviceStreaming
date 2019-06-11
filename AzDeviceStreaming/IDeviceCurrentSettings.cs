namespace AzIoTHubDeviceStreams
{
    public interface IDeviceCurrentSettings
    {
        bool KeepAlive { get; set; }
        bool Respond { get; set; }

        //Get properties above from msgIn
        string ProcessMsgIn(string msgIn);
    }
}