namespace SpotyPie.Services.Bluetooth.Enumerators
{
    public enum BlhStatus
    {
        Started,
        AdapterNotFound,
        AdapterDisabled,
        DeviceNotFound,
        Connecting,
        Connected,
        Disconnected,
        FailedCreateSocket,
        Error
    }
}