# CmdMessenger

A messaging library for the Arduino and .NET/Mono platform

C# Build status: [![C# Build status](https://ci.appveyor.com/api/projects/status/07nelgrs4wu8nh5r?svg=true)](https://ci.appveyor.com/project/ThijsElenbaas/arduino-cmdmessenger)

VB# Build status: [![VB Build status](https://ci.appveyor.com/api/projects/status/3qhrsm2nw7cqc4st?svg=true)](https://ci.appveyor.com/project/ThijsElenbaas/arduino-cmdmessenger-yaykh)

Arduino Build status: [![Build Status](https://api.travis-ci.org/thijse/Arduino-CmdMessenger.svg?branch=master&dfff)](https://travis-ci.org/thijse/Arduino-CmdMessenger) 


## Introduction

CmdMessenger is a messaging library for the Arduino Platform (and .NET/Mono platform). It supports multiple transport layers: serial port over USB, Bluetooth, TCP/IP (under development) 

The message format is:
```
Cmd Id, param 1, [...] , param N;
```

Although the field separator ',' and command separator ';' can be changed

The library can
* both send and receive of commands 
* Both write and read multiple arguments
* Both write and read all primary data types
* Attach callback functions any received command

The library supports any primary data types, and zero to many multiple arguments. Arguments can either be sent in plain text (to be human readable) or in binary form (to be efficient). 

With version 3.x also comes a full implementation of the toolkit in C#, which runs both in Mono (http://monodevelop.com/Download) and Visual Studio (http://www.microsoft.com/visualstudio/eng#downloads)
This allows for full 2-way communication between the arduino controller and the PC.

If you are looking for a Python client to communicate with, please have a look at [PyCmdMessenger](https://github.com/harmsm/PyCmdMessenger)

## Requirements

* [Arduino IDE Version 1.0.5 or later](http://www.arduino.cc/en/Main/Software)* 

\* Earlier versions of the Arduino IDE will probably work but have not been tested.

## Downloading

This package can be downloaded in different manners 


- The Arduino Library Manager: [see here how to use it](http://www.arduino.cc/en/guide/libraries#toc3).
- The PlatformIO Library Manager: [see here how to use it](http://docs.platformio.org/en/latest/ide/arduino.html).
- By directly loading fetching the Archive from GitHub: 
 1. Go to [https://github.com/thijse/Arduino-CmdMessenger](https://github.com/thijse/Arduino-CmdMessenger)
 2. Click the DOWNLOAD ZIP button in the panel on the
 3. Rename the uncompressed folder **Arduino-CmdMessenger-master** to **CmdMessenger**.
 4. You may need to create the libraries subfolder if its your first library.  
 5. Place the **CmdMessenger** library folder in your **arduinosketchfolder/libraries/** folder. 
 5. Restart the IDE.
 6. For more information, [read this extended manual](http://thijs.elenbaas.net/2012/07/installing-an-arduino-library/)
- If you want to have a package that includes all referenced libraries, use the pre-packaged library
 1. Download the package as a zipfile [here](https://github.com/thijse/Zipballs/blob/master/CmdMessenger/CmdMessenger.zip?raw=true) or as a tarball [here ](https://github.com/thijse/Zipballs/blob/master/CmdMessenger/CmdMessenger.tar.gz?raw=true).
 2. Copy the folders inside the **libraries** folder  to you your **arduinosketchfolder/libraries/** folder.
 3. Restart the IDE.
 3. For more information, [read this extended manual](http://thijs.elenbaas.net/2012/07/installing-an-arduino-library/)

## Getting Started

Get to know the library, by trying the examples,from simple to complex:
### Receive 
  The 1st example will make the PC toggle the integrated led on the Arduino board. 

  * On the Arduino side, it demonstrates how to:
	  - Define commands
	  - Set up a serial connection
	  - Receive a command with a parameter from the PC
  * On the PC side, it demonstrates how to:
	  - Define commands
	  - Set up a serial connection
	  - Send a command with a parameter to the Arduino

### SendandReceive 
  This example expands the previous Receive example. The Arduino will now send back a status. 
  On the Arduino side, 
  * it demonstrates how to:
	  - Handle received commands that do not have a function attache
	  - Send a command with a parameter to the PC
  * On the PC side, it demonstrates how to:
	  - Handle received commands that do not have a function attached
	  - Receive a command with a parameter from the Arduino

### SendandReceiveArguments
  This example expands the previous SendandReceive example. The Arduino will now receive multiple 
  and sent multiple float values. 
  * On the arduino side, it demonstrates how to:
	  - Return multiple types status 
	  - Receive multiple parameters,
	  - Send multiple parameters
      - Call a function periodically
  * On the PC side, it demonstrates how to:
	  - Send multiple parameters, and wait for response 
	  - Receive multiple parameters
	  - Add logging events on data that has been sent or received
  
### SendandReceiveBinaryArguments

  This example expands the previous SendandReceiveArguments example. The Arduino will receive and send multiple 
  Binary values, demonstrating that this is more efficient way of communication. 

  * On the Arduino side, it demonstrates how to:
	  - Send binary parameters
	  - Receive binary parameters
  * On the PC side, it demonstrates how to:
	  - Receive multiple binary parameters,
      - Send multiple binary parameters
      - How callback events can be handled while the main program waits
	  - How to calculate milliseconds, similar to Arduino function Millis()

### DataLogging

This example expands the previous SendandReceiveArguments example. The PC will now send a start command to the Arduino, and wait for a response from the Arduino. The Arduino will start sending analog data which the PC will plot in a chart

This example shows how to :

- Use CmdMessenger in combination with GUI applications
- Use CmdMessenger in combination with ZedGraph
- Use the StaleGeneralStrategy
 
### ArduinoController

This example expands the SendandReceiveArguments example. The PC will now sends commands to the Arduino when the trackbar is pulled. Every TrackBarChanged events will queue a message to the Arduino to set the blink speed of the internal / pin 13 LED

This example shows how to :

- use CmdMessenger in combination with GUI applications
- use CmdMessenger in combination with ZedGraph
- Send queued commandssds
- use the CollapseCommandStrategy

### SimpleWatchdog
This example shows the usage of the watchdog for communication over virtual serial port:
- Use auto scanning and connecting
- Use watchdog 


### SimpleBluetooth
This example shows the usage of the watchdog for communication over Bluetooth, tested with the well known JY-MCU HC-05 and HC-06
On Arduino side, this uses the  SimpleWatchdog.ino script as the previous example
- Use bluetooth connection
- Use auto scanning and connecting
- Use watchdog 

### TemperatureControl

This example expands the previous ArduinoController example. The PC will now send a start command to the Arduino, and wait for a response from the Arduino. The Arduino will start sending temperature data and the heater steering value data which the PC will plot in a chart. With a slider we can set the goal temperature, which will make the PID software on the controller adjust the setting of the heater.
 
This example shows how to design a responsive performance UI that sends and receives commands
- Send queued commands
- Add queue strategies

### ConsoleShell

This example shows how to use CmdMessenger as a shell, and communicate with it using the Serial Console
This example is different from the others:


- there is no PC counterpart 
- it will only receive commands, instead of sending 
- commands it will use Serial.Print

 Below is an example of interacting with the sample:
 
```
   Available commands
   0;                  - This command list
   1,<led state>;      - Set led. 0 = off, 1 = on
   2,<led brightness>; - Set led brighness. 0 - 1000
   3;                  - Show led state
  
 Command> 3;
  
  Led status: on
  Led brightness: 500
  
 Command> 2,1000;
  
   Led status: on
   Led brightness: 1000
  
 Command> 1,0;
  
   Led status: off
   Led brightness: 1000
```

All samples are heavily documented and should be self explanatory. 
 
1. Open the Example sketch in the Arduino IDE and compile and upload it to your board.
2. Open de CmdMessenger.sln solution in Visual Studio or Mono Develop/Xamarin Studio
3. Set example project with same name as the Arduino sketch as start-up project, and run
4. Enjoy!

## Trouble shooting
* If the PC and Arduino are not able to connect, chances are that either the selected port on the PC side is not correct or that the Arduino and PC are not at the same baud rate. Try it out by typing commands into the Arduino Serial Monitor, using the ConsoleShell
* Some boards (e.g. Sparkfun Pro Micro) need DtrEnable set to be true.
* If the port and baud rate are correct but callbacks are not being invoked, try looking at logging of sent and received data. See the SendandReceiveArguments project for an example.
* If you have a problem that is hard to pinpoint, use the CommandMessengerTests testsuite. This project runs unit tests on several parts on the mayor parts of the CmdMessenger library. Note that the primary function is not to serve as an example, so the code may be less documented  and clean as the example projects. 


## Notes
An example for use with Max5 / MaxMSP was included up until version 2. (it can still be found here https://github.com/dreamcat4/CmdMessenger).
Since we have not been able to check it wil Max/MaxMSP, the example was removed.

## Changelog 

### CmdMessenger v4.0.0
* [Arduino] Additional autoConnect sample
* [.Net/.Mono] Full Threading redesign.
* [.Net/.Mono] AutoConnect & watchdog functionality. 
* [.Net/.Mono] Tested Linux compatibility
* [.Net/.Mono] Visual Basic samples. 
* [.Net/.Mono] Native Bluetooth support (windows only)

### CmdMessenger v3.6
* [Arduino] Bugfix: approx 1 in 1000 commands failed, when multiple binary parameters are sent over
* [Arduino] Bugfix: Binary sending of non-number would give compile time error 
* [Arduino] feature: Posibility to send command without argument
* [Arduino] feature: Posibility to send floats with scientific notation, to get full float range
* [.Net/.Mono] Added Unit tests 
* [.Net/.Mono] Consistent variables on .NET and Arduino side. 
* [.Net/.Mono] Major performance improvement (for boards like Teensy 3), by combining queued commands

### CmdMessenger v3.5
* [Arduino] Added console shell sample 
* [Arduino] Minor performance improvement
* [.Net/.Mono] Minor performance improvement

### CmdMessenger v3.4
* [Arduino] Bug-fix in receiving binary values
* [.Net/.Mono] Bug-fix that makes communication 100x  (!) faster, while lowering system load
* [.Net/.Mono] Added function to run on single core

### CmdMessenger v3.3
* [Arduino] Speed improvements for Teensy

### CmdMessenger v3.2
* [All] Clean transport layer interface makes it easy to implement other transport modes 
  (Bluetooth, ZigBee, Web), even if they do not implement a virtual serial port
* [.Net/.Mono] Adaptive throttling to work with transport layers of any speed
* [.Net/.Mono] Smart queuing for smooth running applications and no hanging UI
* [Arduino] Small fixes and sending long argument support

### CmdMessenger v3.1
* Adds 2 GUI examples 

### CmdMessenger v3.0

* Wait for acknowlegde commands
* Sending of common type arguments (float, int, char)
* Multi-argument commands
* Escaping of special characters in strings
* Sending of binary data of any type (uses escaping, no need for Base-64 Encoding) 
* Bugfixes 
* Added code documentation
* Added multiple samples

### CmdMessenger v2 

* Updated to work with Arduino IDE 022
* Enable / disable newline (print and ignore)
* New generic example (works with all Arduinos)
* More reliable process() loop.
* User can set their own cmd and field seperator
 (defaults to ';' and ',')
* Base-64 encoded data to avoid collisions with ^^
* Works with Arduino Serial Monitor for easy debugging

## Credit

* Initial Messenger Library - Thomas Ouellet Fredericks.
* CmdMessenger Version 1    - Neil Dudman.
* CmdMessenger Version 2    - Dreamcat4.
* CmdMessenger Version 3    - Thijs Elenbaas
* CmdMessenger Version 4    - Thijs Elenbaas & Valeriy kucherenko

## On using and modifying libraries

- [http://www.arduino.cc/en/Main/Libraries](http://www.arduino.cc/en/Main/Libraries)
- [http://www.arduino.cc/en/Reference/Libraries](http://www.arduino.cc/en/Reference/Libraries) 

## Copyright

CmdMessenger is provided Copyright © 2013,2014,2015,2016 under MIT License.

