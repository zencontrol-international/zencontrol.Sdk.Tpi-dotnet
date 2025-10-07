using System;
using System.Collections.Generic;
using System.Linq;

namespace zencontrol.Sdk.Tpi;

/// <summary>
/// The connection to the Controller that has the Tpi feature enabled.
/// </summary>
public class Client : IDisposable
{
    // Private fields.
    private readonly ITransport _transport;

    // Constructors.
    /// <summary>
    /// Constructs a TPI client using a custom transport implementation.
    /// </summary>
    /// <param name="transport">The transport medium for TPI communication (UDP or other)</param>
    public Client(ITransport transport)
    {
        _transport = transport;
    }
    
    /// <summary>
    /// Constructs a TPI client using the built-in UDP transport.
    /// </summary>
    /// <param name="ipAddress">The IP address of the TPI-enabled controller</param>
    /// <param name="port">The UDP port of the controller (default: 5108)</param>
    public Client(string ipAddress, int port = 5108)
    {
        _transport = new Udp(ipAddress, port);
    }


    /// <summary>
    /// Sends a mode command to the TPI controller and returns the response.
    /// </summary>
    /// <param name="modeCommand">The mode command to send (Mode0Command, Mode1Command, etc.)</param>
    /// <returns>TpiReply containing the controller's response to the command</returns>
    /// <exception cref="ArgumentNullException">Thrown when modeCommand is null</exception>
    public TpiReply SendModeCommand(IModeCommand modeCommand)
    {
        _transport.Open();
        if (modeCommand == null) throw new ArgumentNullException(nameof(modeCommand));
        byte[] bytes = modeCommand.ToByteArray();
        _transport.WriteBytes(bytes);
        byte[] udpReply = new byte[10];
        byte[] replyBytes = _transport.ReadBytes();
        if (replyBytes.Length != 3) return new TpiReply();
        TpiReply rep = new TpiReply(replyBytes);
        _transport.Close();
        return rep;

    }

    // Private Methods.
    internal static byte CalculateChecksum(IReadOnlyCollection<byte> packet)
    {
        if (packet.Count != 6) throw new TpiException("Invalid packet length.");
        return packet.Aggregate<byte, byte>(0, (current, t) => (byte)(current ^ t));
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _transport?.Dispose();
    }
}