using System;
using System.Collections.Generic;
using Xunit;
using Xunit.Abstractions;
using zencontrol.Sdk.Tpi;

namespace src.test;

public class Tests
{
    private readonly ITestOutputHelper _output;

    public Tests(ITestOutputHelper output)
    {
        _output = output;
    }

    // ═══════════════════════════════════════════════════════════════
    // Unit tests – no hardware required
    // ═══════════════════════════════════════════════════════════════

    #region TpiAddress

    [Fact]
    public void TpiAddress_GetAddress_ReturnsCorrectByte()
    {
        Assert.Equal(0x00, TpiAddress.GetAddress(0).AddressNumber);
        Assert.Equal(0x05, TpiAddress.GetAddress(5).AddressNumber);
        Assert.Equal(0x3F, TpiAddress.GetAddress(63).AddressNumber);
    }

    [Fact]
    public void TpiAddress_GetAddress_Throws_WhenOutOfRange()
    {
        Assert.Throws<TpiException>(() => TpiAddress.GetAddress(64));
    }

    [Fact]
    public void TpiAddress_GetGroup_ReturnsCorrectByte()
    {
        Assert.Equal(0x40, TpiAddress.GetGroup(0).AddressNumber);
        Assert.Equal(0x44, TpiAddress.GetGroup(4).AddressNumber);
        Assert.Equal(0x4F, TpiAddress.GetGroup(15).AddressNumber);
    }

    [Fact]
    public void TpiAddress_GetGroup_Throws_WhenOutOfRange()
    {
        Assert.Throws<TpiException>(() => TpiAddress.GetGroup(16));
    }

    [Fact]
    public void TpiAddress_GetBroadcast_ReturnsCorrectByte()
    {
        Assert.Equal(0x7F, TpiAddress.GetBroadcast().AddressNumber);
    }

    [Fact]
    public void TpiAddress_GetAddressFromEnum_Matches()
    {
        var fromEnum = TpiAddress.GetAddressFromEnum(TpiAddress.Address.Address42);
        Assert.Equal(0x2A, fromEnum.AddressNumber);
    }

    #endregion

    #region Mode0Command - byte output

    [Fact]
    public void Mode0_DirectArcLevel_DeviceAddress_EncodesCorrectly()
    {
        var cmd = new Mode0Command(TpiAddress.GetAddress(5), arcLevel: 240);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(7, bytes.Length);
        Assert.Equal(0x00, bytes[0]); // control byte (Mode 0)
        Assert.Equal(0x00, bytes[1]); // data HI
        Assert.Equal(0x00, bytes[2]); // data MID
        Assert.Equal(0x00, bytes[3]); // data LO
        Assert.Equal(0x0A, bytes[4]); // address: (5 << 1) = 10 = 0x0A, bit 0 = 0 (direct)
        Assert.Equal(240,  bytes[5]); // arc level
        Assert.Equal(0x00 ^ 0x00 ^ 0x00 ^ 0x00 ^ 0x0A ^ 240, bytes[6]); // checksum
    }

    [Fact]
    public void Mode0_DirectArcLevel_GroupAddress_MatchesSpecExample()
    {
        // Spec Example 2: Set group 15 to arc level 240
        // Expected: \x00\x00\x00\x00\x9E\xF0\x6E
        var cmd = new Mode0Command(TpiAddress.GetGroup(15), arcLevel: 240);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x00, bytes[0]);
        Assert.Equal(0x00, bytes[1]);
        Assert.Equal(0x00, bytes[2]);
        Assert.Equal(0x00, bytes[3]);
        Assert.Equal(0x9E, bytes[4]); // group 15: (0x4F << 1) = 0x9E, bit 0 = 0
        Assert.Equal(0xF0, bytes[5]);
        Assert.Equal(0x6E, bytes[6]);
    }

    [Fact]
    public void Mode0_IndirectMax_GroupAddress_MatchesSpecExample()
    {
        // Spec Example 1: Set group 4 to MAX
        // Expected: \x00\x00\x00\x00\x89\x05\x8C
        var cmd = new Mode0Command(TpiAddress.GetGroup(4), Mode0Command.IndirectCommands.RecallMax);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x00, bytes[0]);
        Assert.Equal(0x00, bytes[1]);
        Assert.Equal(0x00, bytes[2]);
        Assert.Equal(0x00, bytes[3]);
        Assert.Equal(0x89, bytes[4]); // group 4 indirect: (0x44 << 1) + 1 = 0x89
        Assert.Equal(0x05, bytes[5]); // MAX
        Assert.Equal(0x8C, bytes[6]);
    }

    [Fact]
    public void Mode0_IndirectCommands_HaveCorrectValues()
    {
        Assert.Equal(0x00, (byte)Mode0Command.IndirectCommands.Off);
        Assert.Equal(0x01, (byte)Mode0Command.IndirectCommands.Up);
        Assert.Equal(0x02, (byte)Mode0Command.IndirectCommands.Down);
        Assert.Equal(0x03, (byte)Mode0Command.IndirectCommands.StepUp);
        Assert.Equal(0x04, (byte)Mode0Command.IndirectCommands.StepDown);
        Assert.Equal(0x05, (byte)Mode0Command.IndirectCommands.RecallMax);
        Assert.Equal(0x06, (byte)Mode0Command.IndirectCommands.RecallMin);
        Assert.Equal(0x10, (byte)Mode0Command.IndirectCommands.Scene0);
        Assert.Equal(0x1F, (byte)Mode0Command.IndirectCommands.Scene15);
    }

    [Fact]
    public void Mode0_ArcLevel255_IsNoChange()
    {
        var cmd = new Mode0Command(TpiAddress.GetAddress(0), arcLevel: 255);
        byte[] bytes = cmd.ToByteArray();
        Assert.Equal(255, bytes[5]); // 255 = no change per spec
    }

    #endregion

    #region Mode1Command - byte output

    [Fact]
    public void Mode1_Inhibit_AddressEncoding_MatchesSpecExample()
    {
        // Spec Example 3: Inhibit address 42 for 8 hours (28800 sec = 0x007080)
        // Expected: \x01\x00\x70\x80\x54\x00\xA5
        var cmd = Mode1Command.CreateInhibit(TpiAddress.GetAddress(42), durationSeconds: 28800);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x01, bytes[0]); // control byte (Mode 1)
        Assert.Equal(0x00, bytes[1]); // data HI
        Assert.Equal(0x70, bytes[2]); // data MID (28800 >> 8)
        Assert.Equal(0x80, bytes[3]); // data LO (28800 & 0xFF)
        Assert.Equal(0x54, bytes[4]); // address: (42 << 1) = 84 = 0x54
        Assert.Equal(0x00, bytes[5]); // INHIBIT command
        Assert.Equal(0xA5, bytes[6]); // checksum
    }

    [Fact]
    public void Mode1_CreateInhibit_SetsCorrectData()
    {
        var cmd = Mode1Command.CreateInhibit(TpiAddress.GetAddress(1), durationSeconds: 0);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x01, bytes[0]);
        Assert.Equal(0x00, bytes[1]);
        Assert.Equal(0x00, bytes[2]);
        Assert.Equal(0x00, bytes[3]);
        Assert.Equal(0x02, bytes[4]); // address 1: (1 << 1) = 2
        Assert.Equal(0x00, bytes[5]); // Inhibit
    }

    [Fact]
    public void Mode1_CreateInhibit_MaxDuration()
    {
        var cmd = Mode1Command.CreateInhibit(TpiAddress.GetAddress(0), durationSeconds: 65535);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x00, bytes[1]);
        Assert.Equal(0xFF, bytes[2]);
        Assert.Equal(0xFF, bytes[3]);
        Assert.Equal(0x00, bytes[5]); // Inhibit
    }

    [Fact]
    public void Mode1_CreateProfile_SetsCorrectBytes()
    {
        var cmd = Mode1Command.CreateProfile(profileNumber: 42);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x01, bytes[0]); // control byte (Mode 1)
        Assert.Equal(0x00, bytes[1]); // data HI
        Assert.Equal(0x00, bytes[2]); // data MID (42 >> 8 = 0)
        Assert.Equal(0x2A, bytes[3]); // data LO (42 & 0xFF)
        Assert.Equal(0x00, bytes[4]); // address always 0 for profile
        Assert.Equal(0x01, bytes[5]); // PROFILE command
    }

    [Fact]
    public void Mode1_CreateProfile_ZeroReturnsToScheduledProfile()
    {
        var cmd = Mode1Command.CreateProfile(profileNumber: 0);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x01, bytes[0]);
        Assert.Equal(0x00, bytes[1]);
        Assert.Equal(0x00, bytes[2]);
        Assert.Equal(0x00, bytes[3]); // profile 0 = return to scheduled
        Assert.Equal(0x00, bytes[4]);
        Assert.Equal(0x01, bytes[5]); // PROFILE
    }

    [Fact]
    public void Mode1_InhibitGroup_EncodesCorrectly()
    {
        var cmd = Mode1Command.CreateInhibit(TpiAddress.GetGroup(3), durationSeconds: 60);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x01, bytes[0]);
        Assert.Equal((byte)(0x43 << 1), bytes[4]); // group 3: (0x43 << 1) = 0x86
        Assert.Equal(0x00, bytes[5]); // Inhibit
    }

    #endregion

    #region Mode2Command – byte output

    [Fact]
    public void Mode2_ShortPress_VirtualInstance10_MatchesSpecExample()
    {
        // Spec Example 4: Short press on instance 10 (virtual instance 0) on Room Controller
        // Expected: \x02\x00\x00\x00\x0A\x01\x09
        var cmd = new Mode2Command(instanceAddress: 0x0A, Mode2Command.Command.PushButtonShortPress);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x02, bytes[0]);
        Assert.Equal(0x00, bytes[1]);
        Assert.Equal(0x00, bytes[2]);
        Assert.Equal(0x00, bytes[3]);
        Assert.Equal(0x0A, bytes[4]); // instance 10
        Assert.Equal(0x01, bytes[5]); // short press
        Assert.Equal(0x09, bytes[6]);
    }

    [Fact]
    public void Mode2_LongPress_EncodesCorrectly()
    {
        var cmd = new Mode2Command(instanceAddress: 0x0B, Mode2Command.Command.PushButtonLongPressAction);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x02, bytes[0]);
        Assert.Equal(0x0B, bytes[4]); // instance 11
        Assert.Equal(0x02, bytes[5]); // long press
    }

    [Fact]
    public void Mode2_AbsoluteInput_EncodesCorrectly()
    {
        var onCmd = new Mode2Command(instanceAddress: 0x05, Mode2Command.Command.AbsoluteInputOn);
        var offCmd = new Mode2Command(instanceAddress: 0x05, Mode2Command.Command.AbsoluteInputOff);

        Assert.Equal(0x02, onCmd.ToByteArray()[5]);
        Assert.Equal(0x01, offCmd.ToByteArray()[5]);
    }

    [Fact]
    public void Mode2_OccupancySensor_EncodesCorrectly()
    {
        var occupied = new Mode2Command(instanceAddress: 0x03, Mode2Command.Command.OccupancyOccupied);
        var unoccupied = new Mode2Command(instanceAddress: 0x03, Mode2Command.Command.OccupancyUnoccupied);

        Assert.Equal(0x02, occupied.ToByteArray()[5]);
        Assert.Equal(0x01, unoccupied.ToByteArray()[5]);
    }

    #endregion

    #region Mode3Command – byte output

    [Fact]
    public void Mode3_LastHeardScene_EncodesCorrectly()
    {
        var cmd = new Mode3Command(TpiAddress.GetAddress(10), Mode3Command.Command.LastHeardScene);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x03, bytes[0]); // control byte (Mode 3)
        Assert.Equal(0x14, bytes[4]); // address: (10 << 1) = 20 = 0x14
        Assert.Equal(0x10, bytes[5]); // Last Heard Scene
    }

    [Fact]
    public void Mode3_CurrentScene_EncodesCorrectly()
    {
        var cmd = new Mode3Command(TpiAddress.GetAddress(0), Mode3Command.Command.CurrentScene);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x03, bytes[0]);
        Assert.Equal(0x00, bytes[4]); // address 0: (0 << 1) = 0
        Assert.Equal(0x11, bytes[5]); // Current Scene
    }

    [Fact]
    public void Mode3_ActualLevel_EncodesCorrectly()
    {
        var cmd = new Mode3Command(TpiAddress.GetAddress(63), Mode3Command.Command.QueryActualLevel);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x03, bytes[0]);
        Assert.Equal(0x7E, bytes[4]); // address 63: (63 << 1) = 126 = 0x7E
        Assert.Equal(0xA0, bytes[5]); // Query Actual Level
    }

    [Fact]
    public void Mode3_GroupAddress_EncodesCorrectly()
    {
        var cmd = new Mode3Command(TpiAddress.GetGroup(0), Mode3Command.Command.CurrentScene);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x03, bytes[0]);
        Assert.Equal(0x80, bytes[4]); // group 0: (0x40 << 1) = 0x80
        Assert.Equal(0x11, bytes[5]);
    }

    [Fact]
    public void Mode3_Broadcast_EncodesCorrectly()
    {
        var cmd = new Mode3Command(TpiAddress.GetBroadcast(), Mode3Command.Command.LastHeardScene);
        byte[] bytes = cmd.ToByteArray();

        Assert.Equal(0x03, bytes[0]);
        Assert.Equal(0xFE, bytes[4]); // broadcast: (0x7F << 1) = 0xFE
        Assert.Equal(0x10, bytes[5]);
    }

    #endregion

    #region Checksum

    [Fact]
    public void Checksum_CalculatedCorrectly()
    {
        // XOR of all 6 preceding bytes per spec
        byte[] packet = { 0x00, 0x00, 0x00, 0x00, 0x89, 0x05 };
        byte checksum = 0x00 ^ 0x00 ^ 0x00 ^ 0x00 ^ 0x89 ^ 0x05;
        Assert.Equal(0x8C, checksum);
    }

    [Fact]
    public void Checksum_AllCommands_HaveCorrectLength()
    {
        // Every command must produce a 7-byte array
        var commands = new IModeCommand[]
        {
            new Mode0Command(TpiAddress.GetAddress(0), arcLevel: 0),
            new Mode0Command(TpiAddress.GetAddress(0), Mode0Command.IndirectCommands.Off),
            Mode1Command.CreateInhibit(TpiAddress.GetAddress(0), 0),
            Mode1Command.CreateProfile(0),
            new Mode2Command(0x00, Mode2Command.Command.PushButtonShortPress),
            new Mode3Command(TpiAddress.GetAddress(0), Mode3Command.Command.LastHeardScene),
        };

        foreach (var cmd in commands)
        {
            byte[] bytes = cmd.ToByteArray();
            Assert.Equal(7, bytes.Length);

            // Verify checksum: XOR of first 6 bytes == 7th byte
            byte computed = 0;
            for (int i = 0; i < 6; i++)
                computed ^= bytes[i];
            Assert.Equal(computed, bytes[6]);
        }
    }

    #endregion

    #region TpiReply

    [Fact]
    public void TpiReply_ParsesReplyOk()
    {
        var reply = new TpiReply(new byte[] { 0x50, 0x00, 0x50 });
        Assert.Equal(TpiReply.AnswerType.ReplyOk, reply.Answer);
        Assert.Equal((byte)0x00, reply.AnswerByte!.Value);
        Assert.Equal((byte)0x50, reply.CheckSum!.Value); // 0x50 ^ 0x00 = 0x50
    }

    [Fact]
    public void TpiReply_ParsesReplyAnswer()
    {
        var reply = new TpiReply(new byte[] { 0x51, 0x01, 0x50 });
        Assert.Equal(TpiReply.AnswerType.ReplyAnswer, reply.Answer);
        Assert.Equal((byte)0x01, reply.AnswerByte!.Value);
    }

    [Fact]
    public void TpiReply_ParsesReplyNoAnswer()
    {
        var reply = new TpiReply(new byte[] { 0x52, 0x00, 0x52 });
        Assert.Equal(TpiReply.AnswerType.ReplyNoAnswer, reply.Answer);
    }

    [Fact]
    public void TpiReply_ParsesReplyError_InvalidCommand()
    {
        var reply = new TpiReply(new byte[] { 0x53, 0x01, 0x52 });
        Assert.Equal(TpiReply.AnswerType.ReplyError, reply.Answer);
        Assert.Equal((byte)0x01, reply.AnswerByte!.Value); // Error 1 = Invalid Command
    }

    [Fact]
    public void TpiReply_ParsesReplyError_ShortCircuit()
    {
        var reply = new TpiReply(new byte[] { 0x53, 0x02, 0x51 });
        Assert.Equal(TpiReply.AnswerType.ReplyError, reply.Answer);
        Assert.Equal((byte)0x02, reply.AnswerByte!.Value); // Error 2 = Short Circuit
    }

    [Fact]
    public void TpiReply_ThrowsOnBadChecksum()
    {
        Assert.Throws<TpiException>(() => new TpiReply(new byte[] { 0x50, 0x00, 0x00 }));
    }

    [Fact]
    public void TpiReply_ThrowsOnInvalidLength()
    {
        Assert.Throws<TpiException>(() => new TpiReply(new byte[] { 0x50, 0x00 }));
        Assert.Throws<TpiException>(() => new TpiReply(new byte[] { 0x50, 0x00, 0x50, 0x00 }));
    }

    [Fact]
    public void TpiReply_ThrowsOnInvalidAnswerType()
    {
        Assert.Throws<TpiException>(() => new TpiReply(new byte[] { 0x54, 0x00, 0x54 }));
    }

    [Fact]
    public void TpiReply_Null_AllPropertiesNull()
    {
        var reply = new TpiReply(isNull: true);
        Assert.Null(reply.Answer);
        Assert.Null(reply.AnswerByte);
        Assert.Null(reply.CheckSum);
        Assert.Empty(reply.ToByteArray());
    }

    [Fact]
    public void TpiReply_ToByteArray_Roundtrips()
    {
        byte[] original = { 0x51, 0x2A, 0x7B };
        var reply = new TpiReply(original);
        byte[] roundtripped = reply.ToByteArray();
        Assert.Equal(original, roundtripped);
    }

    #endregion

    #region BaseModeCommand.ToString

    [Fact]
    public void BaseModeCommand_ToString_ReturnsExpectedFormat()
    {
        var cmd = new Mode0Command(TpiAddress.GetAddress(0), arcLevel: 0);
        string str = cmd.ToString();

        Assert.StartsWith("0x", str);
        Assert.Equal(16, str.Length); // "0x" + 14 hex chars (7 bytes)
    }

    #endregion

    #region Client unit tests (with custom transport)

    [Fact]
    public void Client_SendModeCommand_ThrowsOnNull()
    {
        var transport = new FakeTransport();
        using var client = new Client(transport);
        Assert.Throws<ArgumentNullException>(() => client.SendModeCommand(null!));
    }

    [Fact]
    public void Client_SendModeCommand_SendsAndReceives()
    {
        var transport = new FakeTransport();
        transport.ResponseToReturn = new byte[] { 0x50, 0x00, 0x50 };
        using var client = new Client(transport);

        var reply = client.SendModeCommand(new Mode0Command(TpiAddress.GetAddress(1), arcLevel: 128));

        Assert.NotNull(reply);
        Assert.Equal(TpiReply.AnswerType.ReplyOk, reply.Answer);
        Assert.Single(transport.SentPackets);
    }

    [Fact]
    public void Client_SendModeCommand_ReturnsNullOnWrongLength()
    {
        var transport = new FakeTransport();
        transport.ResponseToReturn = new byte[] { 0x50, 0x00 }; // only 2 bytes
        using var client = new Client(transport);

        var reply = client.SendModeCommand(new Mode0Command(TpiAddress.GetAddress(0), arcLevel: 0));

        Assert.Null(reply.Answer); // null TpiReply
    }

    [Fact]
    public void Client_KeepOpen_OpensOnceAndReuses()
    {
        var transport = new FakeTransport();
        transport.ResponseToReturn = new byte[] { 0x50, 0x00, 0x50 };
        using var client = new Client(transport, keepOpen: true);

        client.SendModeCommand(new Mode0Command(TpiAddress.GetAddress(0), arcLevel: 100));
        client.SendModeCommand(new Mode0Command(TpiAddress.GetAddress(0), arcLevel: 200));

        Assert.Equal(1, transport.OpenCount);
        Assert.Equal(0, transport.CloseCount);
        Assert.Equal(2, transport.SentPackets.Count);
    }

    [Fact]
    public void Client_KeepOpen_ClosesOnDispose()
    {
        var transport = new FakeTransport();
        transport.ResponseToReturn = new byte[] { 0x50, 0x00, 0x50 };
        var client = new Client(transport, keepOpen: true);

        client.SendModeCommand(new Mode0Command(TpiAddress.GetAddress(0), arcLevel: 0));
        client.Dispose();

        Assert.Equal(1, transport.CloseCount);
    }

    [Fact]
    public void Client_NotKeepOpen_OpensAndClosesPerCommand()
    {
        var transport = new FakeTransport();
        transport.ResponseToReturn = new byte[] { 0x50, 0x00, 0x50 };
        using var client = new Client(transport, keepOpen: false);

        client.SendModeCommand(new Mode0Command(TpiAddress.GetAddress(0), arcLevel: 0));
        client.SendModeCommand(new Mode0Command(TpiAddress.GetAddress(0), arcLevel: 0));

        Assert.Equal(2, transport.OpenCount);
        Assert.Equal(2, transport.CloseCount);
    }

    #endregion

    // ═══════════════════════════════════════════════════════════════
    // Fake transport for unit testing Client
    // ═══════════════════════════════════════════════════════════════
    private class FakeTransport : ITransport
    {
        public List<byte[]> SentPackets { get; } = new();
        public byte[] ResponseToReturn { get; set; }
        public int OpenCount { get; private set; }
        public int CloseCount { get; private set; }
        public bool IsDisposed { get; private set; }

        public void Open() => OpenCount++;
        public void Close() => CloseCount++;

        public void WriteBytes(byte[] bytes) => SentPackets.Add(bytes);

        public byte[] ReadBytes()
        {
            if (ResponseToReturn == null)
                return Array.Empty<byte>();
            var copy = new byte[ResponseToReturn.Length];
            Array.Copy(ResponseToReturn, copy, ResponseToReturn.Length);
            return copy;
        }

        public void Dispose() => IsDisposed = true;
    }
}
