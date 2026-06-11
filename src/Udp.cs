using System;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace zencontrol.Sdk.Tpi;

/// <summary>
/// The Stripped down UDP class for communicating with a controller on the network.
/// </summary>
public class Udp : ITransport
{
     // Private fields.
     
    private UdpClient? _udpClient;
    
    // Public Properties
    
    /// <summary>
    /// The receive timeout for UDP socket read operations. If the controller does not respond
    /// within this period, a <see cref="SocketException"/> or <see cref="TimeoutException"/> will be thrown.
    /// Default is 1000ms. Reduce for faster error detection on local networks.
    /// </summary>
    public TimeSpan ReceiveTimeout { get; set; } = TimeSpan.FromMilliseconds(1000);
    
    /// <summary>
    /// The port number for the UDP connection to the controller.
    /// Generally shall be 5108
    /// </summary>
    public int PortNumber { get; set; } = 5108;

    /// <summary>
    /// Controllers IP Address to connect to.
    /// </summary>
    public string IpAddress { get; }

    // Constructors.


    /// <summary>
    /// Create a new Instance of the UDP transport type
    /// </summary>
    /// <param name="ipAddress">The IP address of the device that the transport should connect to: Example: 192.168.5.19</param>
    /// <param name="portNumber">The Port number of the device that the transport should connect to: Example: 8802</param>
    public Udp(string ipAddress, int portNumber)
    {
        IpAddress = ipAddress;
        PortNumber = portNumber;
    }
    


 
    // Public Methods
    
    /// <summary>
    /// Opens the UDP connection to the TPI controller
    /// </summary>
    /// <exception cref="SocketException">Thrown when connection fails</exception>
    public void Open()
    {
        _udpClient = new UdpClient(IpAddress, PortNumber);
        _udpClient.Connect(IpAddress, PortNumber);
        _udpClient.Client.ReceiveTimeout = (int)ReceiveTimeout.TotalMilliseconds;
    }


    /// <summary>
    /// Closes the UDP connection to the TPI controller
    /// </summary>
    public void Close()
    {
        _udpClient?.Close();
    }

 
    /// <summary>
    /// Writes bytes to the UDP connection for transmission to the TPI controller
    /// </summary>
    /// <param name="data">Byte array containing TPI packet to send</param>
    /// <exception cref="UdpException">Thrown when UDP client is not open</exception>
    /// <exception cref="SocketException">Thrown when send operation fails</exception>
    public void WriteBytes(byte[] data)
    {
        if (_udpClient is null)
        {
            throw new UdpException($"UdpClient is null, Call {nameof(Open)} before using this method");
        }
        _udpClient.Send(data, data.Length);
    }

    
    /// <summary>
    /// Reads bytes from the UDP connection, typically a response from the TPI controller
    /// </summary>
    /// <returns>Byte array containing response from controller</returns>
    /// <exception cref="UdpException">Thrown when UDP client is not open</exception>
    /// <exception cref="SocketException">Thrown when receive operation fails</exception>
    /// <exception cref="TimeoutException">Thrown when no data is received within the <see cref="ReceiveTimeout"/> period</exception>
    public byte[] ReadBytes()
    {
        if (_udpClient is null)
        {
            throw new UdpException($"UdpClient is null, Call {nameof(Open)} before using this method");
        }

        try
        {
            Task<UdpReceiveResult> rx = _udpClient.ReceiveAsync();
            rx.Wait(ReceiveTimeout);
            if (!rx.IsCompleted)
            {
                throw new TimeoutException($"UDP receive timed out after {ReceiveTimeout.TotalMilliseconds}ms");
            }
            return rx.Result.Buffer;
        }
        catch (AggregateException ex) when (ex.InnerException is SocketException)
        {
            throw ex.InnerException;
        }
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _udpClient?.Dispose();
    }
}