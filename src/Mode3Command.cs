namespace zencontrol.Sdk.Tpi;

/// <summary>
/// In Mode select 3, Commands can be sent to perform Quick Queries on the controller.
/// </summary>
public class Mode3Command : BaseModeCommand
{
    /// <summary>
    /// Quick Query Commands for retrieving status information from the controller
    /// </summary>
    public enum Command : byte
    {
        /// <summary>Query the last heard scene from the controller</summary>
        LastHeardScene = 0x10,
        /// <summary>Query the current active scene</summary>
        CurrentScene = 0x11,
        /// <summary>Query the actual light level of a device</summary>
        QueryActualLevel = 0xA0,
    }

    /// <summary>
    /// Creates a Mode 3 quick query command.
    /// </summary>
    /// <param name="address">TPI address of the target device to query</param>
    /// <param name="command">Quick query command to execute</param>
    public Mode3Command(TpiAddress address, Command command) : base(ControlByteType.QuickQuery, (byte)((byte)address.AddressNumber << 1), (byte)command)
    {
    }
}