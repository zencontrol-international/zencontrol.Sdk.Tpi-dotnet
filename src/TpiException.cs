using System;

namespace zencontrol.Sdk.Tpi;

/// <summary>
/// Represents an exception that is thrown when an error occurs in the TPI API operations.
/// </summary>
public class TpiException : Exception
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TpiException"/> class with a specified error message.
    /// </summary>
    /// <param name="message">The message that describes the error.</param>
    public TpiException(string message) : base(message)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="TpiException"/> class with a specified error message
    /// and a reference to the inner exception that is the cause of this exception.
    /// </summary>
    /// <param name="message">The error message that explains the reason for the exception.</param>
    /// <param name="innerException">The exception that is the cause of the current exception.</param>
    public TpiException(string message, Exception innerException) : base(message, innerException)
    {
    }
}