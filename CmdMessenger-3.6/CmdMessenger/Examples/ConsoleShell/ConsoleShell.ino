// *** ConsoleShell ***

// This example shows how to use CmdMessenger as a shell, and communicate with it using the Serial Console
// This example is different from all others:
// - there is no PC counterpart 
// - it will only receive commands, instead of sending commands it will use Serial.Pring
//
// Below is an example of interacting with the sample:
// 
//   Available commands
//   0;                  - This command list
//   1,<led state>;      - Set led. 0 = off, 1 = on
//   2,<led brightness>; - Set led brighness. 0 - 1000
//   3;                  - Show led state
//  
// Command> 3;
//  
//  Led status: on
//  Led brightness: 500
//  
// Command> 2,1000;
//  
//   Led status: on
//   Led brightness: 1000
//  
// Command> 1,0;
//  
//   Led status: off
//   Led brightness: 1000


#include <CmdMessenger.h>  // CmdMessenger

// PWM timing variables
unsigned long intervalOn        = 0;
unsigned long prevBlinkTime     = 0;
const unsigned long PWMinterval = 1000;

// Blinking led variables 
bool ledState                   = 1;                 // On/Off state of Led
int  ledBrightness              = prevBlinkTime /2 ; // 50 % Brightness 
const int kBlinkLed             = 13;                // Pin of internal Led

// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial);

// This is the list of recognized commands.  
// In order to receive, attach a callback function to these events
enum
{
  kCommandList         , // Command to request list of available commands
  kSetLed              , // Command to request led to be set in specific state  
  kSetLedBrightness    , // Command to request led to be set in to specific brightness  
  kStatus              , // Command to request led status
};

// Callbacks define on which received commands we take action
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kCommandList, OnCommandList);
  cmdMessenger.attach(kSetLed, OnSetLed);
  cmdMessenger.attach(kSetLedBrightness, OnSetLedBrightness);
  cmdMessenger.attach(kStatus, OnStatus);
}

// Called when a received command has no attached function
void OnUnknownCommand()
{
  Serial.println("This command is unknown!");
  ShowCommands();
}

// Callback function that shows a list of commands
void OnCommandList()
{
  ShowCommands();
}

// Callback function that sets led on or off
void OnSetLed()
{
  // Read led state argument, expects 0 or 1 and interprets as false or true 
  ledState = cmdMessenger.readBoolArg(); 
  ShowLedState();  
}

// Callback function that sets led on or off
void OnSetLedBrightness()
{
  // Read led brightness argument, expects value between 0 to 255
  ledBrightness = cmdMessenger.readInt16Arg();  
  // Set led brightness
  SetBrightness();  
  // Show Led state
  ShowLedState();  
}

// Callback function that shows led status
void OnStatus()
{
  // Send back status that describes the led state
  ShowLedState();  
}

// Show available commands
void ShowCommands() 
{
  Serial.println("Available commands");
  Serial.println(" 0;                 - This command list");
  Serial.println(" 1,<led state>;     - Set led. 0 = off, 1 = on");
  Serial.print  (" 2,<led brightness>; - Set led brighness. 0 - "); 
  Serial.println(PWMinterval);
  Serial.println(" 3;                  - Show led state");
}

// Show led state
void ShowLedState() 
{
  Serial.print("Led status: ");
  Serial.println(ledState?"on":"off");
  Serial.print("Led brightness: ");
  Serial.println(ledBrightness);
}

// Set led state
void SetLedState()
{
  if (ledState)  {
    // If led is turned on, go to correct brightness using analog write
    analogWrite(kBlinkLed, ledBrightness);
  } else {
    // If led is turned off, use digital write to disable PWM
    digitalWrite(kBlinkLed, LOW);
  }
}

// Set led brightness
void SetBrightness() 
{
  // clamp value intervalOn on 0 and PWMinterval
  intervalOn  =  max(min(ledBrightness,PWMinterval),0);
}

// Pulse Width Modulation to vary Led intensity
// turn on until intervalOn, then turn off until PWMinterval
bool blinkLed() {
  if (  micros() - prevBlinkTime > PWMinterval ) {
    // Turn led on at end of interval (if led state is on)
    prevBlinkTime = micros();
    digitalWrite(kBlinkLed, ledState?HIGH:LOW);    
  } else if (  micros() - prevBlinkTime > intervalOn ) {
    // Turn led off at  halfway interval    
    digitalWrite(kBlinkLed, LOW);
  } 
}

// Setup function
void setup() 
{
  // Listen on serial connection for messages from the PC
  Serial.begin(115200); 
  
  // Adds newline to every command
  cmdMessenger.printLfCr();   

  // Attach my application's user-defined callback methods
  attachCommandCallbacks();

  // set pin for blink LED
  pinMode(kBlinkLed, OUTPUT);
  
  // Show command list
  ShowCommands();
}

// Loop function
void loop() 
{
  // Process incoming serial data, and perform callbacks
  cmdMessenger.feedinSerialData();
  blinkLed();
}
