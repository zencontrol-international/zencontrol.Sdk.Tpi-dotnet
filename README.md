# Introduction

The zencontrol platform has been designed for modern buildings with integrated systems from HVAC, BMS, Security to Fans, Shades and Audio Visual systems. One of the ways in which zencontrol encourages connected buildings is the provision of a UDP based network lighting protocol for control of connected DALI luminaires from 3rd party systems. The UDP system allows integration to existing systems and networks infrastructure. The following describes the specification and usage of the network based lighting interface.

# Prerequisites

* Requires a controller connected to the local network
* The controller requires TPI or TPI Advanced to be enabled

# Wiring

* Requires a controller connected to the local network
* The controller requires TPI or TPI Advanced to be enabled
* The controller will require virtual buttons enabled as well as TPI for button press events
* The controller will have 1 or more ECG (Electronic Control Gear) connected to the DALI bus

# Protocol Overview

The TPI protocol uses a 7-byte command structure over UDP port 5108:

| Byte 1 | Bytes 2–4 | Byte 5 | Byte 6 | Byte 7 |
|--------|-----------|--------|--------|--------|
| Control | Data (HI/MID/LO) | Address | Command | Checksum |

Responses are 3 bytes: Answer Type | Answer | Checksum

Four modes of operation:
- **Mode 0** — DALI lighting commands (direct arc levels and indirect relative commands)
- **Mode 1** — Controller configuration (Inhibit, Profile changes)
- **Mode 2** — Virtual instance triggers (push buttons, absolute inputs, occupancy sensors)
- **Mode 3** — Quick queries (last heard scene, current scene, actual level)

For the full protocol specification, see `Documentation/extract.txt`.

# Example Usage

## Send a Mode 0 command (DALI lighting)

Direct or indirect commands to the controller's DALI bus.

```csharp
using var client = new Client("192.168.2.82", 5108);

// Set device address 5 to arc level 200 (direct command)
var directCmd = new Mode0Command(TpiAddress.GetAddress(5), arcLevel: 200);
var reply = client.SendModeCommand(directCmd);

// Recall scene 0 on group 6 (indirect command)
var indirectCmd = new Mode0Command(
    TpiAddress.GetGroup(6), Mode0Command.IndirectCommands.Scene0);
reply = client.SendModeCommand(indirectCmd);

// Turn off all devices (broadcast)
var broadcastCmd = new Mode0Command(
    TpiAddress.GetBroadcast(), Mode0Command.IndirectCommands.Off);
reply = client.SendModeCommand(broadcastCmd);
```

## Send a Mode 1 command (controller configuration)

```csharp
using var client = new Client("192.168.2.82", 5108);

// Inhibit address 42 for 30 seconds
var inhibitCmd = Mode1Command.CreateInhibit(
    TpiAddress.GetAddress(42), durationSeconds: 30);
var reply = client.SendModeCommand(inhibitCmd);

// Release inhibit (duration = 0)
var releaseCmd = Mode1Command.CreateInhibit(
    TpiAddress.GetAddress(42), durationSeconds: 0);
reply = client.SendModeCommand(releaseCmd);

// Request a profile change
var profileCmd = Mode1Command.CreateProfile(profileNumber: 5);
reply = client.SendModeCommand(profileCmd);

// Return to scheduled profile
var scheduledCmd = Mode1Command.CreateProfile(profileNumber: 0);
reply = client.SendModeCommand(scheduledCmd);
```

## Send a Mode 2 command (virtual instances)

Simulate sensors, push buttons or absolute inputs as a virtual device.
This can trigger sequences or other commands configured on the controller.

```csharp
using var client = new Client("192.168.2.82", 5108);

// Short press on virtual instance 0 (Application Controller)
var shortPress = new Mode2Command(
    instanceAddress: 0x00, Mode2Command.Command.PushButtonShortPress);
var reply = client.SendModeCommand(shortPress);

// Long press on virtual instance 10 (Room Controller instance 0)
var longPress = new Mode2Command(
    instanceAddress: 0x0A, Mode2Command.Command.PushButtonLongPressAction);
reply = client.SendModeCommand(longPress);

// Set occupancy sensor to occupied
var occupied = new Mode2Command(
    instanceAddress: 0x02, Mode2Command.Command.OccupancyOccupied);
reply = client.SendModeCommand(occupied);

// Set absolute input to on
var inputOn = new Mode2Command(
    instanceAddress: 0x03, Mode2Command.Command.AbsoluteInputOn);
reply = client.SendModeCommand(inputOn);
```

## Send a Mode 3 command (quick queries)

Device-specific queries for status information.

```csharp
using var client = new Client("192.168.2.82", 5108);

var addr = TpiAddress.GetAddress(7);

// Query the last heard scene
var lastHeardCmd = new Mode3Command(addr, Mode3Command.Command.LastHeardScene);
var reply = client.SendModeCommand(lastHeardCmd);
Console.WriteLine($"Last Heard Scene reply: {reply}");

// Query the current scene
var currentSceneCmd = new Mode3Command(addr, Mode3Command.Command.CurrentScene);
reply = client.SendModeCommand(currentSceneCmd);
Console.WriteLine($"Current Scene reply: {reply}");

// Query the actual light level (returns level or 255 = MIXED)
var levelCmd = new Mode3Command(addr, Mode3Command.Command.QueryActualLevel);
reply = client.SendModeCommand(levelCmd);
if (reply.Answer == TpiReply.AnswerType.ReplyAnswer)
{
    byte level = reply.AnswerByte ?? 255;
    Console.WriteLine(
        $"Actual Level: {(level == 255 ? "MIXED" : level.ToString())}");
}
```

## High-performance usage (keepOpen)

For sending many commands in sequence, use `keepOpen: true` to avoid
open/close overhead on each call:

```csharp
using var client = new Client("192.168.2.82", 5108, keepOpen: true);

for (int i = 0; i < 100; i++)
{
    var cmd = new Mode0Command(
        TpiAddress.GetAddress(7), arcLevel: (byte)(i % 255));
    client.SendModeCommand(cmd);
    Thread.Sleep(50);
}
```

## Custom UDP transport

For fine-grained control over timeouts and networking:

```csharp
var udp = new Udp("192.168.2.82", 5108);
udp.ReceiveTimeout = TimeSpan.FromMilliseconds(500);

using var client = new Client(udp, keepOpen: true);
udp.Open();

var reply = client.SendModeCommand(
    new Mode0Command(TpiAddress.GetAddress(0), arcLevel: 128));

udp.Close();
```

## Understanding replies

```csharp
var reply = client.SendModeCommand(...);

switch (reply.Answer)
{
    case TpiReply.AnswerType.ReplyOk:
        Console.WriteLine("Command accepted, no data returned");
        break;
    case TpiReply.AnswerType.ReplyAnswer:
        Console.WriteLine($"Command accepted, data: {reply.AnswerByte}");
        break;
    case TpiReply.AnswerType.ReplyNoAnswer:
        Console.WriteLine("No response from target device");
        break;
    case TpiReply.AnswerType.ReplyError:
        string error = reply.AnswerByte switch
        {
            1 => "Invalid Command",
            2 => "Short Circuit",
            _ => $"Unknown error: {reply.AnswerByte}"
        };
        Console.WriteLine($"Error: {error}");
        break;
    case null:
        Console.WriteLine("No reply received (timeout or connection issue)");
        break;
}
```

# Address Helpers

```csharp
// Individual devices (0–63)
TpiAddress.GetAddress(5);        // device address 5

// Groups (0–15)
TpiAddress.GetGroup(0);          // group 0
TpiAddress.GetGroup(12);         // group 12

// Broadcast (all devices)
TpiAddress.GetBroadcast();        // address 127

// From predefined enum
TpiAddress.GetAddressFromEnum(TpiAddress.Address.Address42);
```
