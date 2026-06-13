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
    /// The address should already be encoded with the appropriate bit layout (bits 7-1 = DALI address, bit 0 = 0).
    /// </summary>
    /// <param name="address">Raw address byte (pre-encoded)</param>
    /// <param name="commandByte">Command byte specifying the operation</param>
    public Mode1Command(byte address, byte commandByte) : 
        base(BaseModeCommand.ControlByteType.Controller, address, commandByte)
    {
    }

    /// <summary>
    /// Creates a Mode 1 command for a specific device address.
    /// The address is encoded as per the TPI spec: bits 7-1 = DALI address, bit 0 = 0.
    /// </summary>
    /// <param name="address">TPI address of the target device</param>
    /// <param name="commandByte">Command byte specifying the operation</param>
    public Mode1Command(TpiAddress address, byte commandByte) : 
        base(BaseModeCommand.ControlByteType.Controller, (byte)(address.AddressNumber << 1), commandByte)
    {
    }

    /// <summary>
    /// Creates an Inhibit command that prevents the controller from sending DALI commands
    /// to the target in response to sensor stimuli for the specified duration.
    /// Set duration to 0 to end the inhibit condition. Maximum duration is 65535 seconds (~18.2 hours).
    /// </summary>
    /// <param name="address">TPI address of the target to inhibit</param>
    /// <param name="durationSeconds">Duration in seconds (0-65535)</param>
    /// <returns>A Mode1Command configured for inhibit</returns>
    public static Mode1Command CreateInhibit(TpiAddress address, ushort durationSeconds)
    {
        var cmd = new Mode1Command(address, (byte)Mode1CommandType.Inhibit);
        // Duration in big-endian across MID and LO data bytes (HI remains 0 per spec)
        cmd.Data = new byte[] { 0x00, (byte)(durationSeconds >> 8), (byte)(durationSeconds & 0xFF) };
        return cmd;
    }

    /// <summary>
    /// Creates a Profile Change command that enacts an unscheduled profile change on the controller.
    /// The profile must be pre-configured on the controller. Use profile number 0 to return
    /// the controller to its scheduled profile.
    /// </summary>
    /// <param name="profileNumber">Profile number (0-65535). 0 returns to scheduled profile.</param>
    /// <returns>A Mode1Command configured for profile change</returns>
    public static Mode1Command CreateProfile(ushort profileNumber)
    {
        // Profile always uses address 0 per spec
        var cmd = new Mode1Command((byte)0x00, (byte)Mode1CommandType.Profile);
        // Profile number in big-endian across MID and LO data bytes (HI remains 0 per spec)
        cmd.Data = new byte[] { 0x00, (byte)(profileNumber >> 8), (byte)(profileNumber & 0xFF) };
        return cmd;
    }
}