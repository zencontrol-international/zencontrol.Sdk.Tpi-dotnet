using zencontrol.Sdk.Tpi;

// Constants for the program, please change this area to match the requirements for the demonstration.
const string controllerIpAddress = "192.168.8.116";
const int maxNumberOfStepDownCommands = 60;
const int virtualInstanceAddress = 0x00;
const int millisecondsTimeout = 1000;
TpiAddress deviceAddress = TpiAddress.GetAddress(34);


// Program starts here.
var controller = new Client(controllerIpAddress);

// In Mode select 0, DALI commands are separated in to Direct Lighting commands, and Indirect Lighting 
// commands based on the indirect/direct selector bit in the Address Byte. Please ensure this bit is set 
// correctly for the right command set. No Mode Select 0 commands use the Data bytes, which should be 
// left unset (all zero).
// Direct commands – used to specify a particular light level.
// Indirect commands – provide relative instructions such as step up or step down.

Thread.Sleep(millisecondsTimeout);
Console.WriteLine("Example is using Mode 0 to send a direct Arc command to the a device.");
var mode0Packet = new Mode0Command(deviceAddress, arcLevel: 210);
Console.WriteLine($"Sending Packet: {mode0Packet}");
var mode0Reply = controller.SendModeCommand(mode0Packet);
Console.WriteLine($"Reply: {mode0Reply}");

Console.WriteLine($"\n\nExample is using Mode 0 to send a command to the a device. {maxNumberOfStepDownCommands} times.");
mode0Packet = new Mode0Command(deviceAddress, Mode0Command.IndirectCommands.StepDown);
Console.WriteLine($"Sending Packet: {mode0Packet}");
for (var i = 0; i < maxNumberOfStepDownCommands; i++)
{
    mode0Reply = controller.SendModeCommand(mode0Packet);
}
Console.WriteLine($"Reply: {mode0Reply}");


// In Mode select 1, Control commands can be sent to modify how the controller sends commands on the 
// bus.



// In Mode select 2, Commands can be sent to emulate triggers on virtual instances. zencontrol room 
// controllers can be configured to have up to two such virtual instances. Zencontrol application controllers 
// can have up to 10. Both uses the instance addresses described in the Address Byte section. These 
// instances can be of the type Push Button, Absolute Input or Occupancy Sensor.

Thread.Sleep(millisecondsTimeout);
Console.WriteLine("\n\nExample is using Mode 2 to emulate triggers on the controller");
var mode2Packet = new Mode2Command(virtualInstanceAddress, Mode2Command.Command.PushButtonShortPress);
Console.WriteLine($"Sending Packet: {mode2Packet}");
var mode2Reply = controller.SendModeCommand(mode2Packet);
Console.WriteLine($"Reply: {mode2Reply}");


// In Mode select 3, Commands can be sent to perform Quick Queries on the controller.
Thread.Sleep(millisecondsTimeout);
Console.WriteLine($"\n\nExample is using Mode 3 to perform Quick Queries on the controller on IP Address {controllerIpAddress}");
var mode3Packet = new Mode3Command(deviceAddress, Mode3Command.Command.QueryActualLevel);
Console.WriteLine($"Sending Packet: {mode3Packet}");
var mode3Reply = controller.SendModeCommand(mode3Packet);
Console.WriteLine($"Reply: {mode3Reply}");


// Finally to tie the items together.
// Start with using mode 0 to send direct arc commands to a device and then query the actual level
// this can be paired with many queries to plot the results. or store them into a database for customers? 
Console.WriteLine($"\n\nExample is using all the modes together. Starting with changing the device {deviceAddress} to arc Level 0");
mode0Packet = new Mode0Command(deviceAddress, arcLevel: 0);
Thread.Sleep(millisecondsTimeout);
Console.WriteLine($"Sending Packet: {mode0Packet}");
mode0Reply = controller.SendModeCommand(mode0Packet);
Console.WriteLine($"Reply: {mode0Reply}");
Thread.Sleep(millisecondsTimeout);
Console.WriteLine($"Sending Packet: {mode3Packet}");
mode3Reply = controller.SendModeCommand(mode3Packet);
Console.WriteLine($"Reply: {mode3Reply}");

Console.WriteLine($"Change the Arc of the device on Address {deviceAddress}");
mode0Packet = new Mode0Command(deviceAddress, arcLevel: 210);
Thread.Sleep(millisecondsTimeout);
Console.WriteLine($"Sending Packet: {mode0Packet}");
mode0Reply = controller.SendModeCommand(mode0Packet);
Console.WriteLine($"Reply: {mode0Reply}");
Thread.Sleep(millisecondsTimeout);
Console.WriteLine($"Sending Packet: {mode3Packet}");
mode3Reply = controller.SendModeCommand(mode3Packet);
Console.WriteLine($"Reply: {mode3Reply}");


// Showing button presses with Mode 2 (virtual button presses)
Thread.Sleep(millisecondsTimeout);
Console.WriteLine($"\n\nExample is using Mode 2 to show the virtual button presses on instance {virtualInstanceAddress}");
Console.WriteLine($"Sending Packet {mode2Packet}");
mode2Reply = controller.SendModeCommand(mode2Packet);
Thread.Sleep(millisecondsTimeout);
Console.WriteLine($"Sending Packet: {mode3Packet}");
mode3Reply = controller.SendModeCommand(mode3Packet);
Console.WriteLine($"Reply: {mode3Reply}");
Thread.Sleep(millisecondsTimeout);
Console.WriteLine($"Sending Packet: {mode2Packet}");
mode0Reply = controller.SendModeCommand(mode2Packet);
Thread.Sleep(millisecondsTimeout);
Console.WriteLine($"Sending Packet: {mode3Packet}");
mode3Reply = controller.SendModeCommand(mode3Packet);
Console.WriteLine($"Reply: {mode3Reply}");
Thread.Sleep(millisecondsTimeout);



Console.WriteLine($"Sending Packet {mode2Packet}");
mode2Reply = controller.SendModeCommand(mode2Packet);
Thread.Sleep(millisecondsTimeout);
Console.WriteLine($"Sending Packet: {mode3Packet}");
mode3Reply = controller.SendModeCommand(mode3Packet);
Console.WriteLine($"Reply: {mode3Reply}");
Thread.Sleep(millisecondsTimeout);
Console.WriteLine($"Sending Packet: {mode2Packet}");
mode2Reply = controller.SendModeCommand(mode2Packet);
Thread.Sleep(millisecondsTimeout);
Console.WriteLine($"Sending Packet: {mode3Packet}");
mode3Reply = controller.SendModeCommand(mode3Packet);
Console.WriteLine($"Reply: {mode3Reply}");

Console.WriteLine("Press Enter to end the demo");
Console.ReadLine();
