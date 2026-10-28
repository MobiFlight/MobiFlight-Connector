namespace MobiFlight
{
    /// <summary>Outcome of trying to connect to a controller's serial port.</summary>
    public enum ConnectResult
    {
        Connected,
        // Another application holds the port.
        PortBusy,
        // Port not found, failed to open, or no usable response.
        NotResponding
    }
}