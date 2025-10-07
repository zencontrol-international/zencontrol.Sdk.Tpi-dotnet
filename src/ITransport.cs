using System;

namespace zencontrol.Sdk.Tpi;

/// <summary>
/// Interface for transporting data to the TPI controller. Provides abstraction
/// for different transport mechanisms (UDP, serial, etc.) used to communicate
/// with zencontrol TPI-enabled devices.
/// </summary>
public interface ITransport : IDisposable
{
    /// <summary>
    /// Writes bytes to the transport medium for transmission to the TPI controller.
    /// </summary>
    /// <param name="bytes">Byte array containing the TPI packet to send</param>
    public void WriteBytes(byte[] bytes);
    
    /// <summary>
    /// Reads bytes from the transport medium, typically a response from the TPI controller.
    /// </summary>
    /// <returns>Byte array containing the response from the controller</returns>
    public byte[] ReadBytes();

    /// <summary>
    /// Opens the Transport medium for communication.
    /// </summary>
    public void Open();
    
    /// <summary>
    /// Closes the Transport medium and releases any resources.
    /// </summary>
    public void Close();
}