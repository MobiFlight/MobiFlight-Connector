// *** SendandReceiveBinaryArguments ***

// This example expands the previous SendandReceiveArguments example. The Arduino will 
//  receive and send multiple Binary values, demonstrating that this is more compact and faster. 
// Since the output is not human readable any more, the logging is disabled and the NewLines 
// are removed
//
// It adds a demonstration of how to:
// - Send binary parameters
// - Receive binary parameters,


#include <CmdMessenger.h>  // CmdMessenger

// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial);

// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
enum
{
    kRequestPlainTextFloatSeries , // Command Request to send series in plain text
    kReceivePlainTextFloatSeries , // Command to send an item in plain text
    kRequestBinaryFloatSeries    , // Command Request to send series in binary form
    kReceiveBinaryFloatSeries    , // Command to send an item in binary form
};

// Callbacks define on which received commands we take action
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kRequestPlainTextFloatSeries, OnRequestPlainTextFloatSeries);
  cmdMessenger.attach(kRequestBinaryFloatSeries,    OnRequestBinaryFloatSeries);
}

// ------------------  C A L L B A C K S -----------------------

// Called when a received command has no attached function
void OnUnknownCommand()
{
  cmdMessenger.sendCmd(0,"Command without attached callback");
}

// Callback function calculates the sum of the two received float values
void OnRequestPlainTextFloatSeries()
{
  // Get series length from 1st parameter
  int16_t seriesLength = cmdMessenger.readInt16Arg();
  float seriesBase     = cmdMessenger.readFloatArg();
 
  // Send back series of floats
  for(int i=0;i< seriesLength;i++) {
     cmdMessenger.sendCmdStart (kReceivePlainTextFloatSeries);
     cmdMessenger.sendCmdArg<uint16_t>((uint16_t)i);
     cmdMessenger.sendCmdArg<float>(((float)i*(float)seriesBase),6);
     cmdMessenger.sendCmdEnd ();
  }
}

// Callback function calculates the sum of the two received float values
void OnRequestBinaryFloatSeries()
{
  // Get series length from 1st parameter
  int16_t seriesLength = cmdMessenger.readBinArg<uint16_t>();
  float seriesBase     = cmdMessenger.readBinArg<float>(); 

  // Disable new lines, this saves another 2 chars per command
  cmdMessenger.printLfCr(false); 
  // Send back series of floats
  for(int i=0;i< seriesLength;i++) {
     cmdMessenger.sendCmdStart (kReceiveBinaryFloatSeries);
     cmdMessenger.sendCmdBinArg<uint16_t>((uint16_t)i);
     cmdMessenger.sendCmdBinArg<float>(((float)i*(float)seriesBase));
     cmdMessenger.sendCmdEnd ();
  }
  // Re-enable new lines, for human readability
  cmdMessenger.printLfCr(true); 
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
} 
