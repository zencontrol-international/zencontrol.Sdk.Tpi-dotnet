namespace zencontrol.Sdk.Tpi;

/// <summary>
/// In Mode select 2, Commands can be sent to emulate triggers on virtual instances. zencontrol room 
///    controllers can be configured to have up to two such virtual instances. Zencontrol application controllers 
///    can have up to 10. Both uses the instance addresses described in the Address Byte section. These 
///    instances can be of the type Push Button, Absolute Input or Occupancy Sensor.
/// </summary>
public class Mode2Command : BaseModeCommand
{
    /// <summary>
    /// Mode 2 commands for virtual instance emulation (buttons, inputs, sensors)
    /// </summary>
    public enum Command : byte
    {
        /// <summary>Emulate a short press on a virtual push button</summary>
        PushButtonShortPress = 0x01,
        /// <summary>Emulate a long press action on a virtual push button</summary>
        PushButtonLongPressAction = 0x02,
        /// <summary>Set absolute input to off state</summary>
        AbsoluteInputOff = 0x01,
        /// <summary>Set absolute input to on state</summary>
        AbsoluteInputOn = 0x02,
        /// <summary>Set occupancy sensor to unoccupied state</summary>
        OccupancyUnoccupied = 0x01,
        /// <summary>Set occupancy sensor to occupied state</summary>
        OccupancyOccupied = 0x02,
    }

    /// <summary>
    /// Creates a Mode 2 command for virtual instance emulation.
    /// </summary>
    /// <param name="instanceAddress">Virtual instance address to target</param>
    /// <param name="command">Virtual command to execute</param>
    public Mode2Command(byte instanceAddress, Command command) : base((ControlByteType)0x02, instanceAddress, (byte) command)
    {
    }
}