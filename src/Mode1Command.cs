namespace zencontrol.Sdk.Tpi;

/// <summary>
/// In Mode select 1, Control commands can be sent to modify how the controller sends commands on the 
///    bus.
/// </summary>
public class Mode1Command : BaseModeCommand
{
    /// <summary>
    /// Mode 1 command types for controller configuration and control
    /// </summary>
    public enum Mode1CommandType : byte
    {
        /// <summary>Inhibit command - prevent controller from sending bus commands</summary>
        Inhibit,
        /// <summary>Profile command - configure controller behavior profiles</summary>
        Profile,
    }
    
    /// <summary>
    /// Creates a Mode 1 command using a raw byte address.
    /// </summary>
    /// <param name="address">Raw device address byte</param>
    /// <param name="commandByte">Command byte specifying the operation</param>
    public Mode1Command(byte address, byte commandByte) : 
        base(BaseModeCommand.ControlByteType.ElectronicControlGear, address, commandByte)
    {
    }
    /// <summary>
    /// Creates a Mode 1 command for a specific device address.
    /// </summary>
    /// <param name="address">TPI address of the target device</param>
    /// <param name="commandByte">Command byte specifying the operation</param>
    public Mode1Command(TpiAddress address, byte commandByte) : 
        base(BaseModeCommand.ControlByteType.ElectronicControlGear, address.AddressNumber, commandByte)
    {
    } 
}