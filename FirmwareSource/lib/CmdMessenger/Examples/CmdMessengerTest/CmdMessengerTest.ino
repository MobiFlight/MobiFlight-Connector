// *** CommandMessengerTest ***
// This project runs unit tests on several parts on the mayor parts of the CmdMessenger library
// Note that the primary function is not to serve as an example, so the code may be less documented 
// and clean than the example projects. 

#include <CmdMessenger.h>  // CmdMessenger

char field_separator   = ',';
char command_separator = ';';
char escape_separator  = '/';

// Blinking led variables 
unsigned long previousToggleLed = 0;   // Last time the led was toggled
bool ledState                   = 0;   // Current state of Led
const int kBlinkLed             = 13;  // Pin of internal Led

int seriesLength;
int seriesLengthCount;

// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial, field_separator, command_separator);


// ------------------ C M D  L I S T I N G ( T X / R X ) ---------------------

// We can define up to a default of 50 cmds total, including both directions (send + receive)
// and including also the first 4 default command codes for the generic error handling.
// If you run out of message slots, then just increase the value of MAXCALLBACKS in CmdMessenger.h

 // This is the list of recognized commands. These can be commands that can either be sent or received. 
 // In order to receive, attach a callback function to these events
 // 
 // Note that commands work both directions:
 // - All commands can be sent
 // - Commands that have callbacks attached can be received
 // 
 // This means that both sides should have an identical command list:
 // both sides can either send it or receive it (or even both)    
enum
{
  // General commands
  kCommError                , // Command reports serial port comm error (only works for some comm errors)
  kComment                  , // Command to sent comment in argument
  
  // Setup connection test
  kAcknowledge              , // Command to acknowledge that cmd was received
  kAreYouReady              , // Command to ask if other side is ready
  kError                    , // Command to report errors
  
  // Acknowledge test
  kAskUsIfReady             , // Command to ask other side to ask if ready 
  kYouAreReady              , // Command to acknowledge that other is ready
  
  // Clear & Binary text data test
  kValuePing                , // Command to send value to other side
  kValuePong                , // Command to return value received with pong
  
  // Multiple Arguments test
  kMultiValuePing           , // Command to send value to other side
  kMultiValuePong           , // Command to return value received with pong
  
  // Benchmarks
  kRequestReset             , // Command Request reset
  kRequestResetAcknowledge  , // Command to acknowledge reset

  kRequestSeries            , // Command Request to send series in plain text
  kReceiveSeries            , // Command to send an item in plain text
  kDoneReceiveSeries,
        
  kPrepareSendSeries        , // Command to tell other side to prepare for receiving a series of text float commands
  kSendSeries               , // Command to send a series of text float commands
  kAckSendSeries            , // Command to acknowledge the send series of text float commands
};

// Needed for ping-pong function
enum
{
  kBool,
  kInt16,
  kInt32,
  kFloat,
  kFloatSci,
  kDouble,
  kDoubleSci,
  kChar,
  kString,
  kBBool,
  kBInt16,
  kBInt32,
  kBFloat,
  kBDouble,
  kBChar,
  kEscString,
};


void attachCommandCallbacks()
{
  // Attach callback methods
  //cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kAreYouReady, OnArduinoReady);
  cmdMessenger.attach(kAskUsIfReady, OnAskUsIfReady);

  // Clear & Binary text data test
  cmdMessenger.attach(kValuePing, OnValuePing);
  cmdMessenger.attach(kMultiValuePing, OnMultiValuePing);
  
  // Benchmarks
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kRequestReset,      OnRequestReset);
  cmdMessenger.attach(kRequestSeries,     OnRequestSeries);

  cmdMessenger.attach(kPrepareSendSeries, OnPrepareSendSeries);
  cmdMessenger.attach(kSendSeries,        OnSendSeries);
}

// ------------------  C A L L B A C K S -----------------------

void OnArduinoReady()
{
  // In response to ping. We just send a throw-away Acknowledgment to say "i'm ready"
  cmdMessenger.sendCmd(kAcknowledge,"Arduino ready");
}

void OnUnknownCommand()
{
  // Default response for unknown commands and corrupt messages
  cmdMessenger.sendCmd(kError,"Unknown command");
  cmdMessenger.sendCmdStart(kYouAreReady);  
  cmdMessenger.sendCmdArg("Command without attached callback");    
  cmdMessenger.sendCmdArg(cmdMessenger.commandID());    
  cmdMessenger.sendCmdEnd();
}

void OnAskUsIfReady()
{
  // The other side asks us to send kAreYouReady command, wait for
  //acknowledge
   int isAck = cmdMessenger.sendCmd(kAreYouReady, "Asking PC if ready", true, kAcknowledge,1000 );
  // Now we send back whether or not we got an acknowledgments  
  cmdMessenger.sendCmd(kYouAreReady,isAck?1:0);
}

void OnValuePing()
{
  int dataType = cmdMessenger.readInt16Arg(); 
   switch (dataType) 
   {
      // Plain text
      case kBool:
      {
        bool value = cmdMessenger.readBoolArg();
       cmdMessenger.sendCmd(kValuePong, value);
        break;
      }
      case kInt16:
      {
        int value = cmdMessenger.readInt16Arg();
        cmdMessenger.sendCmd(kValuePong, value);
        break;
      }
      case kInt32:    
      {  
        long value = cmdMessenger.readInt32Arg();
        cmdMessenger.sendCmd(kValuePong, value);
        break;
      }
      case kFloat:
      {
         float value = cmdMessenger.readFloatArg();
         cmdMessenger.sendCmd(kValuePong, value);
         break;
      }
      case kDouble:
      {
         double value = cmdMessenger.readDoubleArg();
         cmdMessenger.sendCmd(kValuePong, value);
         break;
      }
      case kChar:    
      {  
        char value = cmdMessenger.readCharArg();
        cmdMessenger.sendCmd(kValuePong, value);
        break;
      }
      case kString:   
      {   
        char * value = cmdMessenger.readStringArg();
        cmdMessenger.sendCmd(kValuePong, value);
        break;
      }
      // Binary values
      case kBBool:
      {
         bool value = cmdMessenger.readBinArg<bool>();
         cmdMessenger.sendBinCmd(kValuePong, value);
         break;
      }
      case kBInt16:
      {
         int16_t value = cmdMessenger.readBinArg<int16_t>();
         cmdMessenger.sendBinCmd(kValuePong, value);
         break;
      }
      case kBInt32:
      {
         int32_t value = cmdMessenger.readBinArg<int32_t>();
         cmdMessenger.sendBinCmd(kValuePong, value);
         break;
      }
      case kBFloat:
      {
         float value = cmdMessenger.readBinArg<float>();
         cmdMessenger.sendBinCmd(kValuePong, value);
         break;
      }
      case kFloatSci:
      {
        float value = cmdMessenger.readFloatArg();
        cmdMessenger.sendCmdStart(kValuePong);
        cmdMessenger.sendCmdSciArg(value,2);
        cmdMessenger.sendCmdEnd();
         break;
      }
      case kBDouble:
      {
         double value = cmdMessenger.readBinArg<double>();
         cmdMessenger.sendBinCmd(kValuePong, value);
         break;
      }
      case kDoubleSci:
      {
        double value = cmdMessenger.readDoubleArg();
        cmdMessenger.sendCmdStart(kValuePong);
        cmdMessenger.sendCmdSciArg(value,4);
        cmdMessenger.sendCmdEnd();
       break;
      }
      case kBChar:    
      {  
         char value = cmdMessenger.readBinArg<char>();
         cmdMessenger.sendBinCmd(kValuePong, value);
         break;
      }
      case kEscString:   
      {   
        char * value = cmdMessenger.readStringArg();
        cmdMessenger.unescape(value);
        cmdMessenger.sendCmdStart(kValuePong);
        cmdMessenger.sendCmdEscArg(value);
        cmdMessenger.sendCmdEnd();
        break;
      }
      default: 
        cmdMessenger.sendCmd(kError,"Unsupported type for valuePing!");  
        break;
   }   
}

void OnMultiValuePing()
{  
   int16_t valueInt16 = cmdMessenger.readBinArg<int16_t>();  
   int32_t valueInt32 = cmdMessenger.readBinArg<int32_t>();  
   double valueDouble = cmdMessenger.readBinArg<double>();
   
   cmdMessenger.sendCmdStart(kMultiValuePong);
   cmdMessenger.sendCmdBinArg(valueInt16);
   cmdMessenger.sendCmdBinArg(valueInt32);
   cmdMessenger.sendCmdBinArg(valueDouble);
   cmdMessenger.sendCmdEnd();
}

//--------------- Benchmarks ----------------------

void OnRequestReset()
{
    seriesLengthCount = 0;
     cmdMessenger.sendCmd(kRequestResetAcknowledge,"");
}
// Callback function calculates the sum of the two received float values
void OnRequestSeries()
{
  // Get series length from 1st parameter
  int seriesLength = cmdMessenger.readInt16Arg();
  float seriesBase = cmdMessenger.readFloatArg();
 
  // Send back series of floats
  for(int i=0;i< seriesLength;i++) {
     cmdMessenger.sendCmdStart (kReceiveSeries);
     cmdMessenger.sendCmdArg<float>(((float)i*(float)seriesBase),6);
     cmdMessenger.sendCmdEnd ();
  }
  cmdMessenger.sendCmd(kDoneReceiveSeries,"");
}

void OnPrepareSendSeries()
{
  seriesLength      = cmdMessenger.readInt16Arg();
  seriesLengthCount = 0;
}

void OnSendSeries()
{
  seriesLengthCount++;
  //float seriesBase = cmdMessenger.readFloatArg();
  if (seriesLengthCount == seriesLength) {
    cmdMessenger.sendCmd(kAckSendSeries,"");
  }
}


// ------------------ M A I N ( ) ----------------------


void setup() 
{
  // Listen on serial connection for messages from the pc
  
  // 115200 is the max speed on Arduino Uno, Mega, with AT8u2 USB
  // SERIAL_8N1 is the default config, but we want to make certain
  // that we have 8 bits to our disposal
  Serial.begin(115200); 

  // Maximum speed of some boards: Arduino Duemilanove, FTDI Serial
  //Serial.begin(57600);  

  // Many bluetooth breakout boards run on 9600 at default speed
  // The Serial setting below should match this
   //Serial.begin(9600); 

  // Makes output more readable whilst debugging in Arduino Serial Monitor, 
  // but uses more bytes 
  cmdMessenger.printLfCr();   

  // Attach my application's user-defined callback methods
  attachCommandCallbacks();

  // Set command to PC to say we're ready
  //OnArduinoReady();
  
  cmdMessenger.sendCmd(kAcknowledge,"Arduino has resetted!");

  // set pin for blink LED
  pinMode(kBlinkLed, OUTPUT);
}

bool hasExpired(unsigned long &prevTime, unsigned long interval) {
  if (  millis() - prevTime > interval ) {
    prevTime = millis();
    return true;
  } else     
    return false;
}

void loop() 
{
  // Process incoming serial data, and perform callbacks
  cmdMessenger.feedinSerialData();

  // toggle LED. If the LED does not toggle every timeoutInterval, 
  // this means the callbacks my the Messenger are taking a longer time than that  
  if (hasExpired(previousToggleLed,2000)) // Every 2 secs
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