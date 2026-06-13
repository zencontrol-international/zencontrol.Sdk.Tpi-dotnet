namespace zencontrol.Sdk.Tpi;

/// <summary>
/// Class to hold the Address space as defined by the TPI documentation 
/// </summary>
public class TpiAddress
{
    private byte _addressNumber;

    private TpiAddress(byte addressNumber)
    {
        _addressNumber = addressNumber;
    }
    /// <summary>
    /// Creates a TPI address for an individual device.
    /// </summary>
    /// <param name="addressNumber">Device address number (0-63)</param>
    /// <returns>TPIAddress instance for the specified device</returns>
    /// <exception cref="TpiException">Thrown when address number is out of range</exception>
    public static TpiAddress GetAddress(byte addressNumber)
    {
        if (addressNumber > 63)
        {
            throw new TpiException("Address is out of range requires a Number between 0-63.");
        }

        return new TpiAddress(addressNumber);
    }

    /// <summary>
    /// Creates a TPI address for a device group.
    /// </summary>
    /// <param name="groupNumber">Group number (0-15)</param>
    /// <returns>TPIAddress instance for the specified group</returns>
    /// <exception cref="TpiException">Thrown when group number is out of range</exception>
    public static TpiAddress GetGroup(byte groupNumber)
    {
        if (groupNumber > 15)
        {
            throw new TpiException("Group Number is invalid and requires to be between the numbers 0-15");
        }

        return new TpiAddress((byte)(0x40 + groupNumber));
    }

    /// <summary>
    /// Creates a broadcast TPI address that targets all devices.
    /// </summary>
    /// <returns>TPIAddress instance for broadcast communication</returns>
    public static TpiAddress GetBroadcast()
    {
        return new TpiAddress(0x7F);
    }
    
    /// <summary>
    /// Creates a TPI address from a predefined address enum value.
    /// </summary>
    /// <param name="address">Predefined address from the Address enum</param>
    /// <returns>TPIAddress instance for the specified enum value</returns>
    public static TpiAddress GetAddressFromEnum(Address address)
    {
        return new TpiAddress((byte)address);
    }
    
    /// <summary>
    /// The Byte value of the Address.
    /// </summary>
    public byte AddressNumber => _addressNumber;

    /// <summary>
    /// Predefined TPI addresses for devices, groups, and broadcast
    /// </summary>
    public enum Address : byte
    {
#pragma warning disable CS1591
        Address0 = 0x00,
        Address1 = 0x01,
        Address2 = 0x02,
        Address3 = 0x03,
        Address4 = 0x04,
        Address5 = 0x05,
        Address6 = 0x06,
        Address7 = 0x07,
        Address8 = 0x08,
        Address9 = 0x09,
        Address10 = 0x0A,
        Address11 = 0x0B,
        Address12 = 0x0C,
        Address13 = 0x0D,
        Address14 = 0x0E,
        Address15 = 0x0F,
        Address16 = 0x10,
        Address17 = 0x11,
        Address18 = 0x12,
        Address19 = 0x13,
        Address20 = 0x14,
        Address21 = 0x15,
        Address22 = 0x16,
        Address23 = 0x17,
        Address24 = 0x18,
        Address25 = 0x19,
        Address26 = 0x1A,
        Address27 = 0x1B,
        Address28 = 0x1C,
        Address29 = 0x1D,
        Address30 = 0x1E,
        Address31 = 0x1F,
        Address32 = 0x20,
        Address33 = 0x21,
        Address34 = 0x22,
        Address35 = 0x23,
        Address36 = 0x24,
        Address37 = 0x25,
        Address38 = 0x26,
        Address39 = 0x27,
        Address40 = 0x28,
        Address41 = 0x29,
        Address42 = 0x2A,
        Address43 = 0x2B,
        Address44 = 0x2C,
        Address45 = 0x2D,
        Address46 = 0x2E,
        Address47 = 0x2F,
        Address48 = 0x30,
        Address49 = 0x31,
        Address50 = 0x32,
        Address51 = 0x33,
        Address52 = 0x34,
        Address53 = 0x35,
        Address54 = 0x36,
        Address55 = 0x37,
        Address56 = 0x38,
        Address57 = 0x39,
        Address58 = 0x3A,
        Address59 = 0x3B,
        Address60 = 0x3C,
        Address61 = 0x3D,
        Address62 = 0x3E,
        Address63 = 0x3F,
        Group0 = 0x40,
        /// <summary>Group address 1</summary>
        Group1 = 0x41,
        /// <summary>Group address 2</summary>
        Group2 = 0x42,
        /// <summary>Group address 3</summary>
        Group3 = 0x43,
        /// <summary>Group address 4</summary>
        Group4 = 0x44,
        /// <summary>Group address 5</summary>
        Group5 = 0x45,
        /// <summary>Group address 6</summary>
        Group6 = 0x46,
        /// <summary>Group address 7</summary>
        Group7 = 0x47,
        /// <summary>Group address 8</summary>
        Group8 = 0x48,
        /// <summary>Group address 9</summary>
        Group9 = 0x49,
        /// <summary>Group address 10</summary>
        Group10 = 0x4A,
        /// <summary>Group address 11</summary>
        Group11 = 0x4B,
        /// <summary>Group address 12</summary>
        Group12 = 0x4C,
        /// <summary>Group address 13</summary>
        Group13 = 0x4D,
        /// <summary>Group address 14</summary>
        Group14 = 0x4E,
        /// <summary>Group address 15</summary>
        Group15 = 0x4F,
        
        /// <summary>Broadcast address for all devices</summary>
        BroadCast = 0x7F,
    }

    public override string ToString()
    {
        return $"0x{_addressNumber}";
    }
}