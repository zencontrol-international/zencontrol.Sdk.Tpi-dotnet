namespace zencontrol.Sdk.Tpi;

/// <summary>
/// In Mode select 0, DALI commands are separated in to Direct Lighting commands, and Indirect Lighting 
///    commands based on the indirect/direct selector bit in the Address Byte. Please ensure this bit is set 
///    correctly for the right command set. No Mode Select 0 commands use the Data bytes, which should be 
///    left unset (all zero).
///    Direct commands – used to specify a particular light level.
///    Indirect commands – provide relative instructions such as step up or step down.
///
/// </summary>
public class Mode0Command : BaseModeCommand
{
    /// <summary>
    /// Creates a direct arc level command for a specific device address.
    /// </summary>
    /// <param name="address">TPI address of the target device</param>
    /// <param name="arcLevel">Arc power level (0-254, where 255 is reserved for STOP)</param>
    public Mode0Command(TpiAddress address, byte arcLevel) :
        base(BaseModeCommand.ControlByteType.ElectronicControlGear, (byte)((byte)address.AddressNumber << 1), arcLevel)
    {
    }

    /// <summary>
    /// Creates a direct arc level command using a raw byte address.
    /// </summary>
    /// <param name="address">Raw device address byte</param>
    /// <param name="arcLevel">Arc power level (0-254, where 255 is reserved for STOP)</param>
    public Mode0Command(byte address, byte arcLevel) : base(BaseModeCommand.ControlByteType.ElectronicControlGear,
        address, arcLevel)
    {
    }

    /// <summary>
    /// Creates an indirect command using a raw byte address.
    /// </summary>
    /// <param name="address">Raw device address byte</param>
    /// <param name="command">Indirect command to execute</param>
    public Mode0Command(byte address, IndirectCommands command) : base(
        BaseModeCommand.ControlByteType.ElectronicControlGear, address, (byte)command)
    {
    }

    /// <summary>
    /// Creates an indirect command for a specific device address.
    /// </summary>
    /// <param name="address">TPI address of the target device</param>
    /// <param name="command">Indirect command to execute</param>
    public Mode0Command(TpiAddress address, IndirectCommands command) :
        base(BaseModeCommand.ControlByteType.ElectronicControlGear,
            (byte)(((byte)address.AddressNumber << 1) + 1), (byte)command)
    {
    }

    /// <summary>
    /// Indirect commands for DALI lighting control - relative commands that modify
    /// the current state rather than setting absolute values.
    /// </summary>
    public enum IndirectCommands : byte
    {
        /// <summary>Turn the light off immediately</summary>
        Off,

        /// <summary>Step the Device Down for 200mS</summary>
        Up,

        /// <summary>Step the Device Down for 200mS</summary>
        Down,

        /// <summary>Increase light level by one step</summary>
        StepUp,

        /// <summary>Decrease light level by one step</summary>
        StepDown,

        /// <summary>Recall the maximum light level preset</summary>
        RecallMax,

        /// <summary>Recall the minimum light level preset</summary>
        RecallMin,

        /// <summary>Step down and if at the min level then turn off</summary>
        StepDownAndOff,

        /// <summary>(if off) Turn on and then step up</summary>
        OnAndStepUp,

        /// <summary>Enable Direct Arc Power Control sequence</summary>
        EnabledDapcSequence,

        // Reserved values 10-15

        /// <summary>Recall scene 0 preset</summary>
        Scene0 = 16,

        /// <summary>Recall scene 1 preset</summary>
        Scene1,

        /// <summary>Recall scene 2 preset</summary>
        Scene2,

        /// <summary>Recall scene 3 preset</summary>
        Scene3,

        /// <summary>Recall scene 4 preset</summary>
        Scene4,

        /// <summary>Recall scene 5 preset</summary>
        Scene5,

        /// <summary>Recall scene 6 preset</summary>
        Scene6,

        /// <summary>Recall scene 7 preset</summary>
        Scene7,

        /// <summary>Recall scene 8 preset</summary>
        Scene8,

        /// <summary>Recall scene 9 preset</summary>
        Scene9,

        /// <summary>Recall scene 10 preset</summary>
        Scene10,

        /// <summary>Recall scene 11 preset</summary>
        Scene11,

        /// <summary>Recall scene 12 preset</summary>
        Scene12,

        /// <summary>Recall scene 13 preset</summary>
        Scene13,

        /// <summary>Recall scene 14 preset</summary>
        Scene14,

        /// <summary>Recall scene 15 preset</summary>
        Scene15,
    }
}