using System;
using System.Net.Sockets;
using System.Threading;
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
    /// Sets the timeout before attempting to read from the UDP port, if the time is too short,
    /// there is a chance that there are no bytes at the buffer.
    /// If the timer is too long - then there is wasted time. It will depend on the equipment.
    /// </summary>
    public TimeSpan TimeoutMilliSeconds { get; set; } = new TimeSpan(0, 0, 0, 0, 1000);
    
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
    /// <param name="ipAddress">The IP address of the device that the tansport should connect too: Example: 192.168.5.19</param>
    /// <param name="portNumber">The Port number of the device that the transport should connect too: Example: 8802</param>
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
        _udpClient.Client.ReceiveTimeout = (int)TimeoutMilliSeconds.TotalMilliseconds;
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
    /// <exception cref="TimeoutException">Thrown when no data is received within timeout period</exception>
    public byte[] ReadBytes()
    {
        if (_udpClient is null)
        {
            throw new UdpException($"UdpClient is null, Call {nameof(Open)} before using this method");
        }
        Thread.Sleep(TimeoutMilliSeconds);
        Task<UdpReceiveResult>? rx = _udpClient.ReceiveAsync();
        return rx.Result.Buffer;
    }

    /// <inheritdoc />
    public void Dispose()
    {
        _udpClient?.Dispose();
    }
}