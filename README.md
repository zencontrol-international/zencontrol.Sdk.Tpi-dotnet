# Introduction

The zencontrol platform has been designed for modern buildings with integrated system from HVAC, BMS, Security to Fans, Shades and Audio Visual systems. One of the ways in which zencontrol encourages connected buildings is the provision of a UDP based network lighting protocol for control of connected DALI luminaries from 3rd party systems. The UDP system allows integration to existing systems and networks infrastructure. The following describes the specification and usage of the network based lighting interface.

# Prerequisites 

* Requires a controller connnected to the local network
* The controller requires TPI or TPI advanced to be enabled.


# Wiring

* Requires a controller connected to the local network
* The controller requires TPI or TPI advanced to be enabled.
* The controller will require virtual buttons enabled as well as TPI for button press events
* The controller will will have 1 or more ECG (Electronic control gear) connected to the DALI bus.

# Example Use

## Send a Mode 0 Command to the controller.

* 使用Udp时，udp网址的端口是5108
* When using Udp, the port for the udp URL is 5108

Direct or indirect commands to the controller's bus.

```csharp
var udp = new Udp("192.168.2.82",5108,GetLogger());
udp.TimeoutMilliSeconds = new System.TimeSpan(0,0,0,0,500);
udp.Open();
var client = new Client(udp,GetLogger());

var packet = new Mode0Command(TPIAddress.Address.Group6, Mode0Command.IndirectCommands.Scene0);

var reply = client.SendModeCommand(packet);
```

## Send a Mode 1 command to the controller.

```csharp
// Example missing.
```


## Send a Mode 2 command to the controller. 

Simulate sensors, push buttons or Absolute commands to the controller as a virtual device. 
This then can run sequences or other commands that can be determined on the cloud.

```csharp
var udp = new Udp("192.168.2.82",5108,GetLogger());
udp.TimeoutMilliSeconds = new System.TimeSpan(0,0,0,0,500);
udp.Open();
var client = new Client(udp,GetLogger());

var packet = new Mode2Command(0x00, Mode2Command.Command.PushButtonShortPress);
var reply = client.SendModeCommand(packet);
```


## Send a Mode 3 command to the controller. 

Device specific queries.

```csharp
var udp = new Udp("192.168.2.82",5108,GetLogger());
udp.TimeoutMilliSeconds = new System.TimeSpan(0,0,0,0,500);
udp.Open();
var client = new Client(udp,GetLogger());

var address = TPIAddress.Address.Address0;
var command = Mode3Command.Command.QueryActualLevel;

var packet = new Mode3Command(address, command);
var reply = client.SendModeCommand(packet);
```