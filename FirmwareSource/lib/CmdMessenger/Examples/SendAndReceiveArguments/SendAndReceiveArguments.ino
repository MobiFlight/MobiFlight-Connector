// *** SendandReceiveArguments ***

// This example expands the previous SendandReceive example. The Arduino will now receive multiple 
// and sent multiple float values. 
// It adds a demonstration of how to:
// - Return multiple types status; It can return an Acknowlegde and Error command
// - Receive multiple parameters,
// - Send multiple parameters
// - Call a function periodically

#include <CmdMessenger.h>  // CmdMessenger

// Blinking led variables 
unsigned long previousToggleLed = 0;   // Last time the led was toggled
bool ledState                   = 0;   // Current state of Led
const int kBlinkLed             = 13;  // Pin of internal Led

// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial);

// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
enum
{
  // Commands
  kAcknowledge         , // Command to acknowledge that cmd was received
  kError               , // Command to report errors
  kFloatAddition       , // Command to request add two floats
  kFloatAdditionResult , // Command to report addition result
};

// Commands we send from the PC and want to receive on the Arduino.
// We must define a callback function in our Arduino program for each entry in the list below.

void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kFloatAddition, OnFloatAddition);
}

// ------------------  C A L L B A C K S -----------------------

// Called when a received command has no attached function
void OnUnknownCommand()
{
  cmdMessenger.sendCmd(kError,"Command without attached callback");
}

// Callback function that responds that Arduino is ready (has booted up)
void OnArduinoReady()
{
  cmdMessenger.sendCmd(kAcknowledge,"Arduino ready");
}

// Callback function calculates the sum of the two received float values
void OnFloatAddition()
{
  // Retreive first parameter as float
  float a = cmdMessenger.readFloatArg();
  
  // Retreive second parameter as float
  float b = cmdMessenger.readFloatArg();
  
  // Send back the result of the addition
  //cmdMessenger.sendCmd(kFloatAdditionResult,a + b);
  cmdMessenger.sendCmdStart(kFloatAdditionResult);
  cmdMessenger.sendCmdArg(a+b);
  cmdMessenger.sendCmdArg(a-b);
  cmdMessenger.sendCmdEnd();
}

// ------------------ M A I N  ----------------------

// Setup function
void setup() 
{
  // Listen on serial connection for messages from the pc
  Serial.begin(115200); 

  // Adds newline to every command
  cmdMessenger.printLfCr();   

  // Attach my application's user-defined callback methods
  attachCommandCallbacks();

  // Send the status to the PC that says the Arduino has booted
  cmdMessenger.sendCmd(kAcknowledge,"Arduino has started!");

  // set pin for blink LED
  pinMode(kBlinkLed, OUTPUT);
}

// Returns if it has been more than interval (in ms) ago. Used for periodic actions
bool hasExpired(unsigned long &prevTime, unsigned long interval) {
  if (  millis() - prevTime > interval ) {
    prevTime = millis();
    return true;
  } else     
    return false;
}

// Loop function
void loop() 
{
  // Process incoming serial data, and perform callbacks
  cmdMessenger.feedinSerialData();

  // Toggle LED periodically. If the LED does not toggle every 2000 ms, 
  // this means that cmdMessenger are taking a longer time than this  
  if (hasExpired(previousToggleLed,2000)) // Toggle every 2 secs
  {
    toggleLed();  
  } 
}

// Toggle led state 
void toggleLed()
{  
  ledState = !ledState;
  digitalWrite(kBlinkLed, ledState?HIGH:LOW);
}  
