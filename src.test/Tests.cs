using System.Collections.Generic;
using System.Threading;
using Xunit;
using Xunit.Abstractions;
using zencontrol.Sdk.Tpi;


namespace src.test;

public class Tests
{
    private readonly ITestOutputHelper _testOutputHelper;

    public Tests(ITestOutputHelper testOutputHelper)
    {
        _testOutputHelper = testOutputHelper;
    }

    private const string ControllerIpAddress = "192.168.2.123";
    private readonly List<byte> _addresses = [7, 26, 32, 33];

    [Fact]
    public void RawControllerTest()
    {
        Udp udp = new Udp(ControllerIpAddress, 5108);
        udp.TimeoutMilliSeconds = new System.TimeSpan(0, 0, 0, 0, 500);
        udp.Open();
        Client client = new(udp);
        for (int i = 0; i < 10; i++)
        {
            client.SendModeCommand(new Mode0Command(TpiAddress.GetAddress(_addresses[0]),
                Mode0Command.IndirectCommands.RecallMax));
            Thread.Sleep(1000);
            client.SendModeCommand(new Mode0Command(TpiAddress.GetAddress(_addresses[0]),
                Mode0Command.IndirectCommands.Off));
            Thread.Sleep(1000);
        }
    }

}