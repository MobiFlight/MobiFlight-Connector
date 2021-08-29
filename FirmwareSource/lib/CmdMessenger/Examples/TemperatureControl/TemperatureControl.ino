
// *** TemperatureControl ***

// This example expands the previous ArduinoController example. The PC will now send a start command to the controller,
// the controller will now start measuring the temperature and controlling the heater. The controller will also start 
// sending temperature and heater steer data which the PC will plot in a chart. With a slider we can set the goal 
// temperature, which will make the PID library on the controller adjust the setting of the heater.

// To use this example without having a thermocouple or heating element, it comes with a simulated boiler
// In order to use the simulator, disable the #define REAL_HEATER

// NOTE: If you used a package manager to download CmdMessenger library, 
// make sure have also fetched these libraries:
//
// * PID 
// * Adafruit_MAX31855 (not necessary in simulated mode)
//
// A package that includes all referenced libraries can be found at:
// https://github.com/thijse/Zipballs/blob/master/CmdMessenger/CmdMessenger.zip?raw=true



#include <utility\HeaterSim.h>
 
//#define REAL_HEATER;
#ifdef REAL_HEATER
#include <Adafruit_MAX31855.h>
#else
#include <utility\HeaterSim.h>
#endif

#include <CmdMessenger.h>  
#include <PID_v1.h>
#include <utility\\DoEvery.h>   


// Attach a new CmdMessenger object to the default Serial port
CmdMessenger cmdMessenger(Serial);



const int heaterPwmInterval        = 300; // PWM cycle duration 
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
bool controlHeater 	               = false; // Heater start/stop flag

long startAcqMillis                = 0;

double CurrentTemperature          = 20;    // Measured temperature
double goalTemperature             = 20;    // Goal temperature

bool heaterOn                      = false; // Initial binary heater state 
double heaterSteerValue            = 0;     // Initial normalized heater value

// Initialize thermocouple library
#ifdef REAL_HEATER
Adafruit_MAX31855 thermocouple(thermoCLK, thermoCS, thermoDO);  
#else
HeaterSim heaterSim(20);   // Heater is placed in ambient temperature of 20 degrees Celsius
#endif

// Initialize PID library
PID pid(&CurrentTemperature, &heaterSteerValue, &goalTemperature,pidP,pidI,pidD,DIRECT);

// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
enum
{
  // Commands
  kWatchdog            , // Command to request application ID
  kAcknowledge         , // Command to acknowledge a received command
  kError               , // Command to message that an error has occurred
  kStartLogging        , // Command to request logging start              
  kStopLogging         , // Command to request logging stop               
  kPlotDataPoint       , // Command to request data-point plotting  
  kSetGoalTemperature  , // Command to set the goal temperature 
  KSetStartTime        , // Command to set the new start time for the logger   
};

// Commands we send from the PC and want to receive on the Arduino.
// We must define a callback function in our Arduino program for each entry in the list below.
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kWatchdog, OnWatchdogRequest);
  cmdMessenger.attach(kStartLogging, OnStartLogging);
  cmdMessenger.attach(kStopLogging, OnStopLogging);
  cmdMessenger.attach(kSetGoalTemperature, OnSetGoalTemperature);
  cmdMessenger.attach(KSetStartTime, OnSetStartTime);
}

// ------------------  C A L L B A C K S -----------------------

// Called when a received command has no attached function
void OnUnknownCommand()
{
  cmdMessenger.sendCmd(kError,"Command without attached callback");
}

void OnWatchdogRequest()
{
  // Will respond with same command ID and Unique device identifier.
  cmdMessenger.sendCmd(kWatchdog, "77FAEDD5-FAC8-46BD-875E-5E9B6D44F85C");
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

// Callback function that sets the goal temperature
void OnSetGoalTemperature()
{
  // Read led state argument, interpret string as float
  float newTemperature = cmdMessenger.readBinArg<float>();
  
  // Make sure that the argument is valid before we change
  // the goal temperature
  if (cmdMessenger.isArgOk()) {
    goalTemperature = newTemperature;
    
    // Enable heater control (was disabled at initialization)
    controlHeater = true;  
  
    // Send acknowledgment back to PC
    cmdMessenger.sendBinCmd(kAcknowledge,goalTemperature); 
  } else {
    // Error in received goal temperature! Send error back to PC
    cmdMessenger.sendCmd(kError,"Error in received new goal temperature");
  }
}

// Callback function that sets the start time
void OnSetStartTime()
{
	// Read led state argument, interpret string as float
	float startTime = cmdMessenger.readBinArg<float>();
	
	// Make sure that the argument is valid before we change
	if (cmdMessenger.isArgOk()) {
		unsigned long milis =  millis();
		// translate time in seconds to time in milliseconds wrt to internal clock;
		startAcqMillis = (unsigned long)((float)startTime*1000.0f);
		if (startAcqMillis >  milis) { startAcqMillis = milis; }
		startAcqMillis = milis- startAcqMillis;
	}
}

// ------------------ M A I N  ----------------------

// Setup function
void setup() 
{
  // Listen on serial connection for messages from the pc
    
  // 115200 is typically the maximum speed for serial over USB
  Serial.begin(115200);
    
  // Many bluetooth breakout boards run on 9600 at default speed
  // The Serial setting below should match this
  //Serial.begin(9600);    
	
  // Do not print newLine at end of command, 
  // in order to reduce data being sent
  cmdMessenger.printLfCr(false);
  
  //initialize  timers
  tempTimer.reset();
  pidTimer.reset();

  //initialize the PID variables
  pid.SetOutputLimits(0,heaterPwmInterval);

  // Read the current temperature
  #ifdef REAL_HEATER
	CurrentTemperature= thermocouple.readCelsius();
  #else
	CurrentTemperature = heaterSim.GetTemp();
  #endif

  //prepare PID port for writing
  pinMode(switchPin, OUTPUT);  

  //turn the PID on and set to automatic
  pid.SetMode(AUTOMATIC);

  // Set pid sample time to the measure interval
  pid.SetSampleTime(measureInterval);

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

// Measure temperature in boiler 
void measure() {
  if (acquireData) {
     // Calculate time
     float seconds     = (float) (millis()-startAcqMillis) /1000.0 ;
     
     // Measure temperature 
     #ifdef REAL_HEATER
		CurrentTemperature= thermocouple.readCelsius(); // measure with thermocouple
     #else 
		CurrentTemperature = heaterSim.GetTemp();       // measure temperature in simulated boiler
     #endif   
         
     // Send data to PC    
     cmdMessenger.sendCmdStart(kPlotDataPoint);  
     cmdMessenger.sendCmdBinArg((float)seconds);                           // Time    
     cmdMessenger.sendCmdBinArg((float)CurrentTemperature);                // Measured temperature
     cmdMessenger.sendCmdBinArg((float)goalTemperature);                   // Goal temperature
     cmdMessenger.sendCmdBinArg((float)((double)heaterSteerValue/(double)heaterPwmInterval)); // normalized heater steer value
     cmdMessenger.sendCmdBinArg((bool)heaterOn);                        // On / off state during PWM cycle
     cmdMessenger.sendCmdEnd();    
  }  
} 

void SetHeaterState(bool heaterOn)
{
	// Turn heater, connected to relay at pin switchPin
	digitalWrite(switchPin,heaterOn?HIGH:LOW);
}

// Set binary heater state
void heaterPWM()
{
  // Switch heater on or off, based on moment in the PWM cycle
  heaterOn = pidTimer.before(heaterSteerValue);
  #ifdef REAL_HEATER
	SetHeaterState(heaterOn);			  // Turn on heater of boiler 
  #else
	heaterSim.SetHeaterState(heaterOn); // Turn on heater of simulated boiler
  #endif 
  

}