// *** TemperatureControl ***

// This example expands the previous ArduinoController example. The PC will now send a start command to the controller,
// the controller will now start measuring the temperature and controlling the heater. The controller will also start 
// sending temperature and heater steer data which the PC will plot in a chart. With a slider we can set the goal 
// temperature, which will make the PID library on the controller adjust the setting of the heater.
 
#include <Adafruit_MAX31855.h>
#include <CmdMessenger.h>  
#include <PID_v1.h>
#include <DoEvery.h>   

// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger = CmdMessenger(Serial);

const int heaterPwmInterval        = 3000; // PWM cycle duration 
const int measureInterval          = 100;  // Interval between measurements

DoEvery tempTimer(measureInterval);
DoEvery pidTimer(heaterPwmInterval);

// PID settings
double pidP                        = 1500;
double pidI                        = 25;
double pidD                        = 0;

// Thermocouple pins
int thermoDO                       = 3;
int thermoCS                       = 4;
int thermoCLK                      = 5;

// Solid State switch pin
const int switchPin                = 4;

bool acquireData                   = false; // Logging start/stop flag
bool controlHeater 	           = false; // Heater start/stop flag

long startAcqMillis                = 0;

double CurrentTemperature          = 20;    // Measured temperature
double goalTemperature             = 20;    // Goal temperature

bool switchState                   = false; // Initial binary heater state 
double heaterSteerValue            = 0;     // Initial normalized heater value

// Initialize thermocouple library
Adafruit_MAX31855 thermocouple(thermoCLK, thermoCS, thermoDO);

// Initialize PID library
PID pid(&CurrentTemperature, &heaterSteerValue, &goalTemperature,pidP,pidI,pidD,DIRECT);

// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
enum
{
  // Commands
  kAcknowledge         , // Command to acknowledge a received command
  kError               , // Command to message that an error has occurred
  kStartLogging        , // Command to request logging start              
  kStopLogging         , // Command to request logging stop               
  kPlotDataPoint       , // Command to request datapoint plotting  
  kSetGoalTemperature  , // Command to set the goal temperature 
};

// Commands we send from the PC and want to receive on the Arduino.
// We must define a callback function in our Arduino program for each entry in the list below.
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kStartLogging, OnStartLogging);
  cmdMessenger.attach(kStopLogging, OnStopLogging);
  cmdMessenger.attach(kSetGoalTemperature, OnSetGoalTemperature);
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

// Start data acquisition
void OnStartLogging()
{
  // Start data acquisition
  acquireData = true;
  cmdMessenger.sendCmd(kAcknowledge,"Start Logging");
}

// Stop data acquisition
void OnStopLogging()
{
  acquireData    = false;
  cmdMessenger.sendCmd(kAcknowledge,"Stop Logging");
}

// Callback function that sets leds blinking frequency
void OnSetGoalTemperature()
{
  // Read led state argument, interpret string as float
  float newTemperature = cmdMessenger.readFloatArg(); 
  
  // Make sure that the argument is valid before we change
  // the goal temperature
  if (cmdMessenger.isArgOk()) {
    goalTemperature = newTemperature;
    
    // Enable heater control (was disabled at intialization)
    controlHeater = true;  
  
    // Send acknowledgement back to PC
    cmdMessenger.sendCmdStart(kAcknowledge); 
    cmdMessenger.sendCmdArg(goalTemperature,5); 
    cmdMessenger.sendCmdEnd();
  } else {
    // Error in received goal temperature! Send error back to PC
    cmdMessenger.sendCmd(kError,"Error in received new goal temperature");
  }
}

// ------------------ M A I N  ----------------------

// Setup function
void setup() 
{
  // Listen on serial connection for messages from the pc
  Serial.begin(115200); 
  
  //initialize  timers
  tempTimer.reset();
  pidTimer.reset();

  //initialize the PID variables
  pid.SetOutputLimits(0,heaterPwmInterval);

  CurrentTemperature= thermocouple.readCelsius();

  //prepare PID port for writing
  pinMode(switchPin, OUTPUT);  

  //turn the PID on and set to automatic
  pid.SetMode(AUTOMATIC);

  // Set pid sample time to the measure interval
  pid.SetSampleTime(measureInterval);

  // Adds newline to every command
  cmdMessenger.printLfCr();   

  // Attach my application's user-defined callback methods
  attachCommandCallbacks();

  // Send the status to the PC that says the Arduino has booted
  cmdMessenger.sendCmd(kAcknowledge,"Arduino has started!");
}

// Loop function
void loop() 
{
  // Process incoming serial data, and perform callbacks
  cmdMessenger.feedinSerialData();
 
  // Every 100 ms, update the temperature
  if(tempTimer.check()) measure();
 
  // Update the PID timer. 
  pidTimer.check();
  
  // Check if are controlling the heater 
  if (controlHeater) {
      //compute new PID parameters  
      pid.Compute();
      
      //Control the heater using Pulse Width Modulation
      heaterPWM();
  }
}

// simple readout of two Analog pins. 
void measure() {
  if (acquireData) {
     // Calculate time
     float seconds     = (float) (millis()-startAcqMillis) /1000.0 ;
     
     // Measure temperature using thermocouple
     CurrentTemperature = thermocouple.readCelsius(); 
         
     // Send data to PC    
     cmdMessenger.sendCmdStart(kPlotDataPoint);  
     cmdMessenger.sendCmdArg(seconds,4);                           // Time    
     cmdMessenger.sendCmdArg(CurrentTemperature,5);                // Measured temperature
     cmdMessenger.sendCmdArg(goalTemperature,5);                   // Goal temperature
     cmdMessenger.sendCmdArg((double)((double)heaterSteerValue/(double)heaterPwmInterval)); // normalized heater steer value
     cmdMessenger.sendCmdArg((bool)switchState);                   // On / off state during PWM cycle
     cmdMessenger.sendCmdEnd();    
  }  
} 

// Set binary heater state
void heaterPWM()
{
  // Switch heater on or off, based on moment in the PWM cycle
  if(pidTimer.before(heaterSteerValue)) {
    switchState = HIGH;
  } else { 
    switchState = LOW;
  }
  
  // Output on pin switchPin 
  digitalWrite(switchPin,switchState);
}

