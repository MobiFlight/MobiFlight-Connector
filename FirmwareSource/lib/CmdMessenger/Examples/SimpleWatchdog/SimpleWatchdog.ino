// *** SimpleWatchdog ***

// This example shows how to autoconnect between the PC and Aduino.
//
// It demonstrates how to 
// - Respond to a connection request from the PC
// - Use a identifier to handshake

#include <CmdMessenger.h>  // CmdMessenger

// Internal led 
const int ledPin = 13;

// Listen on serial connection for messages from the pc
CmdMessenger messenger(Serial);

// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
enum
{
    kIdentify, // This command will be used both to identify exact device and for watchdog if multiple devices is connected to PC with serial communication
    kTurnLedOn, // This command will be used to turn the internal led on after connection has been established
};

void attachCommandCallbacks()
{
  // Attach callback methods
  messenger.attach(onUnknownCommand);
  messenger.attach(kIdentify  , onIdentifyRequest);
  messenger.attach(kTurnLedOn , onTurnLedOn);
}

// ------------------  C A L L B A C K S -----------------------

// Called when a received command has no attached function
void onUnknownCommand()
{
}

// Callback function to respond to indentify request. This is part of the 
// Auto connection handshake. 
void onIdentifyRequest()
{
  // Here we will send back our communication identifier. Make sure it 
  // corresponds the Id in the C# code. Use F() macro to store ID in PROGMEM
    
  // You can make a unique identifier per device
  messenger.sendCmd(kIdentify, F("BFAF4176-766E-436A-ADF2-96133C02B03C"));
    
  // You could also check for the first device that has the correct application+version running
  //messenger.sendCmd(kIdentify, F("SimpleWatchdog__1_0_1"));
}

// Callback to perform some action
void onTurnLedOn()
{
  // turn led on (this happens after connection)
  digitalWrite(ledPin, HIGH);
}

// ------------------ M A I N  ----------------------

// Setup function
void setup()
{
  // Listen on serial connection for messages from the pc
    
  // 115200 is typically the maximum speed for serial over USB
  //Serial.begin(115200);
  
  // Many bluetooth breakout boards (such as the HC-05/HC-06)
  // run on 9600 at default speed
  // The Serial setting below should match this
  Serial.begin(9600);    
  
  // Attach my application's user-defined callback methods
  attachCommandCallbacks();
  
  // initialize the digital pin as an output.
  pinMode(ledPin, OUTPUT);
  
  // Make sure led is turned off after start or reset
  digitalWrite(ledPin, LOW);
}

// Loop function
void loop()
{
    // Process incoming serial data, and perform callbacks
    messenger.feedinSerialData();
}

