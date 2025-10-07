using System;
using System.Collections.Generic;

namespace zencontrol.Sdk.Tpi;

/// <summary>
/// The Abstract class for the all the commands that are used in the Tpi API.
/// </summary>
public abstract class BaseModeCommand : IModeCommand
{
    /// <summary>
    /// The Type of Packet that is being sent to the device
    /// </summary>
    public enum ControlByteType
    {
        /// <summary>
        /// Commands sent to Electronic Control Gear (ECG) devices like lights/dimmers
        /// </summary>
        ElectronicControlGear = 0x00,
        
        /// <summary>
        /// Commands sent to the controller itself for configuration
        /// </summary>
        Controller,
        
        /// <summary>
        /// Commands for virtual button instances
        /// </summary>
        VirtualButton,
        
        /// <summary>
        /// Quick query commands for status information
        /// </summary>
        QuickQuery,
    }
    
    /// <summary>
    /// the leading byte in the packet, this is the byte that determines the type of data for the controller
    /// </summary>
    public ControlByteType ControlByte { get; set; }

    /// <summary>
    /// used for extended data. Contains a High, middle and low byte.
    /// </summary>
    public byte[] Data { get; set; } = new byte[] { 0x00, 0x00, 0x00 };

    /// <summary>
    /// The device address or instance address depending on the selected Mode.
    /// </summary>
    public byte InstanceAddress { get; set; } = 0x00;

    /// <summary>
    /// The command for the ECG / Controller or virtual instance
    /// </summary>
    public byte CommandByte { get; set; } = 0x00;

    /// <summary>
    /// Base implementation of the TPI commands for the zencontrol TPI API. 
    /// </summary>
    /// <param name="controlByte">The type of control packet being sent</param>
    /// <param name="address">The device or instance address</param>
    /// <param name="commandByte">The specific command byte to execute</param>
    protected BaseModeCommand(ControlByteType controlByte, byte address, byte commandByte)
    {
        ControlByte = controlByte;
        InstanceAddress = address;
        CommandByte = commandByte;
    }

    /// <summary>
    /// creates a byte array of the packet for debugging purposes, or sending over another medium
    /// this contains the complete packet including the control byte, address, command byte and checksum.
    /// </summary>
    /// <returns></returns>
    public byte[] ToByteArray()
    {
        var list = new List<byte>();
        list.Add((byte)ControlByte);
        list.AddRange(Data);
        list.Add(InstanceAddress);
        list.Add(CommandByte);
        list.Add(Client.CalculateChecksum(list));
        return list.ToArray();
    }

    /// <summary>
    /// creates a string byte array of the packet. 
    /// </summary>
    /// <returns>A Byte Array of the packet Example: 0xFFAA55EE</returns>
    public override string ToString()
    {
        return $"0x{BitConverter.ToString(ToByteArray()).Replace("-", string.Empty)}";
    }
}