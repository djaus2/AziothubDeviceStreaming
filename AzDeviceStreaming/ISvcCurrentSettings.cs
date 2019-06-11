namespace AzIoTHubDeviceStreams
{
    public interface ISvcCurrentSettings
    {
        bool ExpectResponse { get; set; }
        bool KeepAlive { get; set; }

        //Embedded above properties in msgOut
        string ProcessMsgOut(string msgOut, bool keepAlive = false, bool expectResponse = true);
    }
}