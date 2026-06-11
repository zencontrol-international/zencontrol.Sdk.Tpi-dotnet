using System;
using System.Diagnostics;
using System.Threading;
using Xunit;
using Xunit.Abstractions;
using zencontrol.Sdk.Tpi;

namespace src.test;

/// <summary>
/// Hardware integration tests for the TPI SDK.
/// These tests require a physical zencontrol controller on the local network.
///
/// Run with: dotnet test --filter "Category=Hardware"
/// </summary>
[Trait("Category", "Hardware")]
public class HardwareTests
{
    private readonly ITestOutputHelper _output;

    public HardwareTests(ITestOutputHelper output)
    {
        _output = output;
    }

    // ═══════════════════════════════════════════════════════════════
    // Configuration – update for your environment before running
    // ═══════════════════════════════════════════════════════════════
    private const string ControllerIpAddress = "192.168.6.21";
    private const int ControllerPort = 5108;
    private static readonly byte[] DeviceAddresses = [26, 27, 18, 28];

    // ═══════════════════════════════════════════════════════════════
    // Mode 0 – DALI lighting commands
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void Mode0_DirectArcLevel_Loop()
    {
        using var client = new Client(ControllerIpAddress, ControllerPort, keepOpen: true);

        foreach(var adr in DeviceAddresses)
        {
            var addr = TpiAddress.GetAddress(adr);
            var reply = client.SendModeCommand(new Mode0Command(addr, arcLevel: 254));
            _output.WriteLine($"Arc 254 reply: {reply}");
            Assert.NotNull(reply.Answer);

            Thread.Sleep(500);

            reply = client.SendModeCommand(new Mode0Command(addr, arcLevel: 1));
            _output.WriteLine($"Arc 1 reply: {reply}");
            Assert.NotNull(reply.Answer);

            Thread.Sleep(500);
        }
    }

    [Fact]
    public void Mode0_IndirectCommands_Loop()
    {
        using var client = new Client(ControllerIpAddress, ControllerPort, keepOpen: true);

        var commands = new[]
        {
            Mode0Command.IndirectCommands.RecallMax,
            Mode0Command.IndirectCommands.Down,
            Mode0Command.IndirectCommands.Down,
            Mode0Command.IndirectCommands.Down,
            Mode0Command.IndirectCommands.Up,
            Mode0Command.IndirectCommands.Up,
            Mode0Command.IndirectCommands.Up,
            Mode0Command.IndirectCommands.Off,
        };

        foreach (var adr in DeviceAddresses)
        {
            var addr = TpiAddress.GetAddress(adr);
            foreach (var cmd in commands)
            {
                var reply = client.SendModeCommand(new Mode0Command(addr, cmd));
                _output.WriteLine($"Addr {adr} {cmd} reply: {reply}");
                Thread.Sleep(500);
            }
        }
    }

    [Fact]
    public void Mode0_GroupBroadcast()
    {
        using var client = new Client(ControllerIpAddress, ControllerPort, keepOpen: true);

        // Group control
        var groupReply = client.SendModeCommand(
            new Mode0Command(TpiAddress.GetGroup(1), Mode0Command.IndirectCommands.RecallMax));
        _output.WriteLine($"Group max reply: {groupReply}");

        Thread.Sleep(1000);

        // Broadcast off
        var broadcastReply = client.SendModeCommand(
            new Mode0Command(TpiAddress.GetBroadcast(), Mode0Command.IndirectCommands.Off));
        _output.WriteLine($"Broadcast off reply: {broadcastReply}");
    }

    [Fact]
    public void Mode0_SceneRecall()
    {
        using var client = new Client(ControllerIpAddress, ControllerPort, keepOpen: true);

        foreach (var adr in DeviceAddresses)
        {
            var addr = TpiAddress.GetAddress(adr);

            // Recall scene 1
            var reply = client.SendModeCommand(
                new Mode0Command(addr, Mode0Command.IndirectCommands.Scene1));
            _output.WriteLine($"Addr {adr} Scene 1 reply: {reply}");

            Thread.Sleep(2000);

            // Recall scene 0
            reply = client.SendModeCommand(
                new Mode0Command(addr, Mode0Command.IndirectCommands.Scene0));
            _output.WriteLine($"Addr {adr} Scene 0 reply: {reply}");
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // Mode 1 – controller configuration
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void Mode1_Inhibit()
    {
        using var client = new Client(ControllerIpAddress, ControllerPort, keepOpen: true);

        foreach (var adr in DeviceAddresses)
        {
            var addr = TpiAddress.GetAddress(adr);

            // Inhibit address for 5 seconds
            var inhibitCmd = Mode1Command.CreateInhibit(addr, 5);
            _output.WriteLine($"Addr {adr} Inhibit packet: {inhibitCmd}");
            var reply = client.SendModeCommand(inhibitCmd);
            _output.WriteLine($"Addr {adr} Inhibit reply: {reply}");

            Thread.Sleep(500);

            // Release inhibit
            var releaseCmd = Mode1Command.CreateInhibit(addr, 0);
            reply = client.SendModeCommand(releaseCmd);
            _output.WriteLine($"Addr {adr} Release inhibit reply: {reply}");
        }
    }

    [Fact(Skip = "Requires physical controller and pre-configured profiles. Remove Skip.")]
    public void Mode1_ProfileChange()
    {
        using var client = new Client(ControllerIpAddress, ControllerPort, keepOpen: true);

        // Return to scheduled profile (profile 0)
        var profileCmd = Mode1Command.CreateProfile(0);
        _output.WriteLine($"Profile packet: {profileCmd}");
        var reply = client.SendModeCommand(profileCmd);
        _output.WriteLine($"Profile change reply: {reply}");
        Assert.NotNull(reply.Answer);

        // Reply-Answer means the controller responded with a value
        if (reply.Answer == TpiReply.AnswerType.ReplyAnswer)
        {
            _output.WriteLine($"  Answer byte: {reply.AnswerByte} (1=changed, 0=not changed)");
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // Mode 2 – virtual instance triggers
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void Mode2_VirtualInstance()
    {
        using var client = new Client(ControllerIpAddress, ControllerPort, keepOpen: true);

        // Simulate short press on virtual instance 0 (address 10 for Room Controller)
        var shortPress = new Mode2Command(0x0A, Mode2Command.Command.PushButtonShortPress);
        _output.WriteLine($"Short press packet: {shortPress}");
        var reply = client.SendModeCommand(shortPress);
        _output.WriteLine($"Short press reply: {reply}");

        Thread.Sleep(500);

        // Simulate long press
        var longPress = new Mode2Command(0x0A, Mode2Command.Command.PushButtonLongPressAction);
        _output.WriteLine($"Long press packet: {longPress}");
        reply = client.SendModeCommand(longPress);
        _output.WriteLine($"Long press reply: {reply}");
    }

    // ═══════════════════════════════════════════════════════════════
    // Mode 3 – quick queries
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void Mode3_QuickQueries()
    {
        using var client = new Client(ControllerIpAddress, ControllerPort, keepOpen: true);

        foreach (var adr in DeviceAddresses)
        {
            var addr = TpiAddress.GetAddress(adr);

            // Last Heard Scene
            var lastHeard = client.SendModeCommand(new Mode3Command(addr, Mode3Command.Command.LastHeardScene));
            _output.WriteLine($"Addr {adr} Last Heard Scene reply: {lastHeard}");

            Thread.Sleep(200);

            // Current Scene
            var currentScene = client.SendModeCommand(new Mode3Command(addr, Mode3Command.Command.CurrentScene));
            _output.WriteLine($"Addr {adr} Current Scene reply: {currentScene}");

            Thread.Sleep(200);

            // Actual Level
            var actualLevel = client.SendModeCommand(new Mode3Command(addr, Mode3Command.Command.QueryActualLevel));
            _output.WriteLine($"Addr {adr} Actual Level reply: {actualLevel}");

            // Reply-Answer should contain the value
            if (actualLevel.Answer == TpiReply.AnswerType.ReplyAnswer)
            {
                byte level = actualLevel.AnswerByte ?? 0xFF;
                _output.WriteLine($"  Level: {(level == 255 ? "MIXED" : level.ToString())}");
            }
        }
    }

    // ═══════════════════════════════════════════════════════════════
    // Performance comparison
    // ═══════════════════════════════════════════════════════════════

    [Fact]
    public void KeepOpen_PerformanceTest()
    {
        const int iterations = 5;
        var addr = TpiAddress.GetAddress(DeviceAddresses[0]);

        // With keepOpen
        var sw = Stopwatch.StartNew();
        using (var client = new Client(ControllerIpAddress, ControllerPort, keepOpen: true))
        {
            for (int i = 0; i < iterations; i++)
            {
                client.SendModeCommand(new Mode0Command(addr, arcLevel: 128));
            }
        }
        sw.Stop();
        _output.WriteLine($"keepOpen=true:  {iterations} commands in {sw.ElapsedMilliseconds}ms " +
                          $"({sw.ElapsedMilliseconds / (double)iterations:F1}ms each)");

        // Without keepOpen
        sw.Restart();
        for (int i = 0; i < iterations; i++)
        {
            using var client = new Client(ControllerIpAddress, ControllerPort, keepOpen: false);
            client.SendModeCommand(new Mode0Command(addr, arcLevel: 128));
        }
        sw.Stop();
        _output.WriteLine($"keepOpen=false: {iterations} commands in {sw.ElapsedMilliseconds}ms " +
                          $"({sw.ElapsedMilliseconds / (double)iterations:F1}ms each)");
    }
}
