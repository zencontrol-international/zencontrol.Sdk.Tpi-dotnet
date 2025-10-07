using System;

namespace zencontrol.Sdk.Tpi;

/// <summary>
/// Represents an exception that is thrown when an error occurs with the UDP communication.
/// </summary>
public class UdpException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="UdpException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public UdpException(string message) : base(message)
    {
        
    }
}