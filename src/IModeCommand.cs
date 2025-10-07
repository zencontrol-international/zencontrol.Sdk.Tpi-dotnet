namespace zencontrol.Sdk.Tpi;

/// <summary>
/// Base interface for all TPI mode commands. Provides methods to convert commands
/// to byte arrays for transmission over the TPI protocol.
/// </summary>
public interface IModeCommand
{
    /// <summary>
    /// Converts the command to a byte array suitable for transmission over TPI.
    /// </summary>
    /// <returns>Byte array containing the complete TPI packet including checksum</returns>
    public byte[] ToByteArray();

    /// <summary>
    /// Returns a hexadecimal string representation of the complete TPI packet.
    /// </summary>
    /// <returns>Hexadecimal string of the packet bytes (e.g., "FFAA55EE")</returns>
    public string ToString();
}