/**
 * Includes Core Arduino functionality 
 **/
char foo; 
#if ARDUINO < 100
  #include <WProgram.h>
#else
  #include <Arduino.h>
#endif

// 1.0.1 : Nicer firmware update, more outputs (20)
// 1.1.0 : Encoder support, more outputs (30)
// 1.2.0 : More outputs (40), more inputs (40), more led segments (4), more encoders (20), steppers (10), servos (10)
// 1.3.0 : Generate New Serial
// 1.4.0 : Servo + Stepper support
// 1.4.1 : Reduce velocity
// 1.5.0 : Improve servo behaviour
// 1.6.0 : Set name
// 1.6.1 : Reduce servo noise
// 1.7.0 : New Arduino IDE, new AVR, Uno Support
// 1.7.1 : More UNO stability
// 1.7.2 : "???"
// 1.7.3 : Servo behaviour improved, fixed stepper bug #178, increased number of buttons per module (MEGA)
// 1.8.0 : added support for LCDs
// 1.9.0 : Support for rotary encoders with different detent configurations
// 1.9.1 : Set "lastCommand" for LCD output command, 
//         Fixed problems with long button and encoder names
//         Memory optimization
// 1.9.2 : Auto reset stepper, more characters for 7 segments
// 1.9.3 : Increased number of pins for MEGA, reduced speed for stepper for more torque
// 1.9.4 : Increased MAX_PINS for MEGA.
// 1.9.5 : Increased MAX_BUTTONS for MEGA and Micro Pro.
// 1.9.6 : Fixed the MAXCALLBACKS for UNO, optimized Build settings for Micro to save memory.
// 1.9.7 : Increased EEPROM area for storing config
// 1.9.8 : Decreased EEPROM area again, changed order during reset/load
// 1.9.9 : Changed MODULE_MAX_PINS and MAX_BUTTONS to 68 (69 is internally needed but it is confusing)
const char version[8] = "1.9.9";

//#define DEBUG 1
#define MTYPE_MEGA 1
#define MTYPE_MICRO 2
#define MTYPE_UNO 3
#define MF_SEGMENT_SUPPORT  1
#define MF_LCD_SUPPORT      1
#define MF_STEPPER_SUPPORT  1
#define MF_SERVO_SUPPORT    1
// ALL 24780
// No Segments 23040 (1740)
// No Steppers 20208 (4572)
// NO Servos   23302 (1478)
// No LCDs     22850 (1930)
// 

#if defined(ARDUINO_AVR_MEGA2560)
#define MODULETYPE MTYPE_MEGA
#elif defined(ARDUINO_AVR_PROMICRO)
#define MODULETYPE MTYPE_MICRO
#elif defined(ARDUINO_AVR_UNO)
#define MODULETYPE MTYPE_UNO
#else
  #error
#endif

#if MODULETYPE == MTYPE_MEGA
#define MODULE_MAX_PINS 68
#endif

#if MODULETYPE == MTYPE_UNO
#define MODULE_MAX_PINS 13
#endif

#if MODULETYPE == MTYPE_MICRO
#define MODULE_MAX_PINS 21
#endif

#define STEPS 64
#define STEPPER_SPEED 400 // 300 already worked, 467, too?
#define STEPPER_ACCEL 800

#if MODULETYPE == MTYPE_MICRO
#define MAX_OUTPUTS     10
#define MAX_BUTTONS     16
#define MAX_LEDSEGMENTS 1
#define MAX_ENCODERS    4
#define MAX_STEPPERS    2
#define MAX_MFSERVOS    2
#define MAX_MFLCD_I2C   2
#endif

#if MODULETYPE == MTYPE_UNO
#define MAX_OUTPUTS     8
#define MAX_BUTTONS     8
#define MAX_LEDSEGMENTS 1
#define MAX_ENCODERS    2
#define MAX_STEPPERS    2
#define MAX_MFSERVOS    2
#define MAX_MFLCD_I2C   2
#endif

#if MODULETYPE == MTYPE_MEGA
#define MAX_OUTPUTS     40
#define MAX_BUTTONS     68
#define MAX_LEDSEGMENTS 4
#define MAX_ENCODERS    20
#define MAX_STEPPERS    10
#define MAX_MFSERVOS    10
#define MAX_MFLCD_I2C   2
#endif

#include <EEPROMex.h>
#include <CmdMessenger.h>
#include <LedControl.h>
#include <Button.h>
#include <TicksPerSecond.h>
#include <RotaryEncoder.h>
#include <Wire.h>

#if MF_SEGMENT_SUPPORT == 1
#include <MFSegments.h>
#endif

#include <MFButton.h>
#include <MFEncoder.h>

#if MF_STEPPER_SUPPORT == 1
#include <AccelStepper.h>
#include <MFStepper.h>
#endif

#if MF_SERVO_SUPPORT == 1
#include <Servo.h>
#include <MFServo.h>
#endif

#include <MFOutput.h>

#if MF_LCD_SUPPORT == 1
#include <LiquidCrystal_I2C.h>
#include <MFLCDDisplay.h>
#endif

const uint8_t MEM_OFFSET_NAME   = 0;
const uint8_t MEM_LEN_NAME      = 48;
const uint8_t MEM_OFFSET_SERIAL = MEM_OFFSET_NAME + MEM_LEN_NAME;
const uint8_t MEM_LEN_SERIAL    = 11;
const uint8_t MEM_OFFSET_CONFIG = MEM_OFFSET_NAME + MEM_LEN_NAME + MEM_LEN_SERIAL;

#if MODULETYPE == MTYPE_MEGA
char type[20]                = "MobiFlight Mega";
char serial[MEM_LEN_SERIAL]  = "1234567890";
char name[MEM_LEN_NAME]      = "MobiFlight Mega";
int eepromSize               = EEPROMSizeMega;
const int  MEM_LEN_CONFIG    = 1024;
#endif

#if MODULETYPE == MTYPE_MICRO
char type[20]                = "MobiFlight Micro";
char serial[MEM_LEN_SERIAL]  = "0987654321";
char name[MEM_LEN_NAME]      = "MobiFlight Micro";
int eepromSize               = EEPROMSizeMicro;
const int  MEM_LEN_CONFIG    = 256;
#endif

#if MODULETYPE == MTYPE_UNO
char type[20]                = "MobiFlight Uno";
char serial[MEM_LEN_SERIAL]  = "0987654321";
char name[MEM_LEN_NAME]      = "MobiFlight Uno";
int eepromSize               = EEPROMSizeUno;
const int  MEM_LEN_CONFIG    = 256;
#endif

char configBuffer[MEM_LEN_CONFIG] = "";

int configLength = 0;
boolean configActivated = false;

bool powerSavingMode = false;
uint8_t pinsRegistered[MODULE_MAX_PINS+1];
const unsigned long POWER_SAVING_TIME = 60*15; // in seconds

CmdMessenger cmdMessenger = CmdMessenger(Serial);
unsigned long lastCommand;

MFOutput outputs[MAX_OUTPUTS];
uint8_t outputsRegistered = 0;

MFButton buttons[MAX_BUTTONS];
uint8_t buttonsRegistered = 0;

#if MF_SEGMENT_SUPPORT == 1
MFSegments ledSegments[MAX_LEDSEGMENTS];
uint8_t ledSegmentsRegistered = 0;
#endif 

MFEncoder encoders[MAX_ENCODERS];
uint8_t encodersRegistered = 0;

#if MF_STEPPER_SUPPORT == 1
MFStepper *steppers[MAX_STEPPERS]; //
uint8_t steppersRegistered = 0;
#endif

#if MF_SERVO_SUPPORT == 1
MFServo servos[MAX_MFSERVOS];
uint8_t servosRegistered = 0;
#endif

#if MF_LCD_SUPPORT == 1
MFLCDDisplay lcd_I2C[MAX_MFLCD_I2C];
uint8_t lcd_12cRegistered = 0;
#endif 

enum
{
  kTypeNotSet,              // 0 
  kTypeButton,              // 1
  kTypeEncoderSingleDetent, // 2 (retained for backwards compatibility, use kTypeEncoder for new configs)
  kTypeOutput,              // 3
  kTypeLedSegment,          // 4
  kTypeStepperDeprecated,   // 5 (keep for backwards compatibility, doesn't support autohome)
  kTypeServo,               // 6
  kTypeLcdDisplayI2C,       // 7
  kTypeEncoder,             // 8
  kTypeStepper              // 9 (new stepper type with auto zero support if btnPin is > 0)
};  

// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
//
// If you increase this list, make sure to check that the MAXCALLBACKS value 
// in CmdMessenger.h is set apropriately
enum
{
  kInitModule,         // 0
  kSetModule,          // 1
  kSetPin,             // 2
  kSetStepper,         // 3
  kSetServo,           // 4
  kStatus,             // 5, Command to report status
  kEncoderChange,      // 6  
  kButtonChange,       // 7
  kStepperChange,      // 8
  kGetInfo,            // 9
  kInfo,               // 10
  kSetConfig,          // 11
  kGetConfig,          // 12
  kResetConfig,        // 13
  kSaveConfig,         // 14
  kConfigSaved,        // 15
  kActivateConfig,     // 16
  kConfigActivated,    // 17
  kSetPowerSavingMode, // 18  
  kSetName,            // 19
  kGenNewSerial,       // 20
  kResetStepper,       // 21
  kSetZeroStepper,     // 22
  kTrigger,            // 23
  kResetBoard,         // 24
  kSetLcdDisplayI2C,   // 25
  kSetModuleBrightness // 26
};

// Callbacks define on which received commands we take action
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);

#if MF_SEGMENT_SUPPORT == 1
  cmdMessenger.attach(kInitModule, OnInitModule);
  cmdMessenger.attach(kSetModule, OnSetModule);
  cmdMessenger.attach(kSetModuleBrightness, OnSetModuleBrightness);
#endif 

  cmdMessenger.attach(kSetPin, OnSetPin);

#if MF_STEPPER_SUPPORT == 1
  cmdMessenger.attach(kSetStepper, OnSetStepper);
#endif

#if MF_SERVO_SUPPORT == 1
  cmdMessenger.attach(kSetServo, OnSetServo);
#endif

  cmdMessenger.attach(kGetInfo, OnGetInfo);
  cmdMessenger.attach(kGetConfig, OnGetConfig);
  cmdMessenger.attach(kSetConfig, OnSetConfig);
  cmdMessenger.attach(kResetConfig, OnResetConfig);
  cmdMessenger.attach(kSaveConfig, OnSaveConfig);
  cmdMessenger.attach(kActivateConfig, OnActivateConfig);  
  cmdMessenger.attach(kSetName, OnSetName);  
  cmdMessenger.attach(kGenNewSerial, OnGenNewSerial);

#if MF_STEPPER_SUPPORT == 1
  cmdMessenger.attach(kResetStepper, OnResetStepper);
  cmdMessenger.attach(kSetZeroStepper, OnSetZeroStepper);
#endif 

  cmdMessenger.attach(kTrigger, OnTrigger);
  cmdMessenger.attach(kResetBoard, OnResetBoard);

#if MF_LCD_SUPPORT == 1
  cmdMessenger.attach(kSetLcdDisplayI2C, OnSetLcdDisplayI2C);  
#endif 

#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus,F("Attached callbacks"));
#endif  

}

void OnResetBoard() {
  EEPROM.setMaxAllowedWrites(1000);
  EEPROM.setMemPool(0, eepromSize);
  
  configBuffer[0]='\0';  
  //readBuffer[0]='\0'; 
  generateSerial(false); 
  clearRegisteredPins();
  lastCommand = millis();  
  loadConfig();
  _restoreName();
}

// Setup function
void setup() 
{
  Serial.begin(115200);
  attachCommandCallbacks();
  cmdMessenger.printLfCr();
  OnResetBoard();
}

void generateSerial(bool force) 
{
  EEPROM.readBlock<char>(MEM_OFFSET_SERIAL, serial, MEM_LEN_SERIAL); 
  if (!force&&serial[0]=='S'&&serial[1]=='N') return;
  randomSeed(analogRead(0));
  sprintf(serial,"SN-%03x-", (unsigned int) random(4095));
  sprintf(&serial[7],"%03x", (unsigned int) random(4095));
  EEPROM.writeBlock<char>(MEM_OFFSET_SERIAL, serial, MEM_LEN_SERIAL);
}

void loadConfig()
{
  resetConfig();
  EEPROM.readBlock<char>(MEM_OFFSET_CONFIG, configBuffer, MEM_LEN_CONFIG);  
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus, F("Restored config"));
  cmdMessenger.sendCmd(kStatus, configBuffer);  
#endif
  for(configLength=0;configLength!=MEM_LEN_CONFIG;configLength++) {
    if (configBuffer[configLength]!='\0') continue;
    break;
  }
  readConfig(configBuffer);
  _activateConfig();
}

void _storeConfig() 
{
  EEPROM.writeBlock<char>(MEM_OFFSET_CONFIG, configBuffer, MEM_LEN_CONFIG);
}

void SetPowerSavingMode(bool state) 
{
  // disable the lights ;)
  powerSavingMode = state;

#if MF_SEGMENT_SUPPORT == 1
  PowerSaveLedSegment(state);
#endif

#ifdef DEBUG  
  if (state)
    cmdMessenger.sendCmd(kStatus, F("On"));
  else
    cmdMessenger.sendCmd(kStatus, F("Off"));    
#endif
  //PowerSaveOutputs(state);
}

void updatePowerSaving() {
  if (!powerSavingMode && ((millis() - lastCommand) > (POWER_SAVING_TIME * 1000))) {
    // enable power saving
    SetPowerSavingMode(true);
  } else if (powerSavingMode && ((millis() - lastCommand) < (POWER_SAVING_TIME * 1000))) {
    // disable power saving
    SetPowerSavingMode(false);
  }
}

// Loop function
void loop() 
{ 
  // Process incoming serial data, and perform callbacks   
  cmdMessenger.feedinSerialData();
  updatePowerSaving();
  
  // if config has been reset
  // and still is not activated
  // do not perform updates
  // to prevent mangling input for config (shared buffers)
  if (!configActivated) return;
  
  readButtons();
  readEncoder();

  // segments do not need update
#if MF_STEPPER_SUPPORT == 1
  updateSteppers();  
#endif

#if MF_SERVO_SUPPORT == 1
  updateServos();  
#endif
}

bool isPinRegistered(uint8_t pin) {
  return pinsRegistered[pin] != kTypeNotSet;
}

bool isPinRegisteredForType(uint8_t pin, uint8_t type) {
  return pinsRegistered[pin] == type;
}

void registerPin(uint8_t pin, uint8_t type) {
  pinsRegistered[pin] = type;
}

void clearRegisteredPins(uint8_t type) {
  for(int i=0; i!=MODULE_MAX_PINS+1;++i)
    if (pinsRegistered[i] == type)
      pinsRegistered[i] = kTypeNotSet;
}

void clearRegisteredPins() {
  for(int i=0; i!=MODULE_MAX_PINS+1;++i)
    pinsRegistered[i] = kTypeNotSet;
}

//// OUTPUT /////
void AddOutput(uint8_t pin = 1, char const * name = "Output")
{
  if (outputsRegistered == MAX_OUTPUTS) return;
  if (isPinRegistered(pin)) return;
  
  outputs[outputsRegistered] = MFOutput(pin);
  registerPin(pin, kTypeOutput);
  outputsRegistered++;
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus, F("Added output"));
#endif  
}

void ClearOutputs() 
{
  clearRegisteredPins(kTypeOutput);
  outputsRegistered = 0;
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus,F("Cleared outputs"));
#endif  
}

//// BUTTONS /////
void AddButton(uint8_t pin = 1, char const * name = "Button")
{  
  if (buttonsRegistered == MAX_BUTTONS) return;
  
  if (isPinRegistered(pin)) return;
  
  buttons[buttonsRegistered] = MFButton(pin, name);
  buttons[buttonsRegistered].attachHandler(btnOnRelease, handlerOnRelease);
  buttons[buttonsRegistered].attachHandler(btnOnPress, handlerOnRelease);

  registerPin(pin, kTypeButton);
  buttonsRegistered++;
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus, F("Added button ")/* + name */);
#endif
}

void ClearButtons() 
{
  clearRegisteredPins(kTypeButton);
  buttonsRegistered = 0;
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus,F("Cleared buttons"));
#endif  
}

//// ENCODERS /////
void AddEncoder(uint8_t pin1 = 1, uint8_t pin2 = 2, uint8_t encoder_type = 0, char const * name = "Encoder")
{  
  if (encodersRegistered == MAX_ENCODERS) return;
  if (isPinRegistered(pin1) || isPinRegistered(pin2)) return;
  
  encoders[encodersRegistered] = MFEncoder();
  encoders[encodersRegistered].attach(pin1, pin2, encoder_type, name);
  encoders[encodersRegistered].attachHandler(encLeft, handlerOnEncoder);
  encoders[encodersRegistered].attachHandler(encLeftFast, handlerOnEncoder);
  encoders[encodersRegistered].attachHandler(encRight, handlerOnEncoder);
  encoders[encodersRegistered].attachHandler(encRightFast, handlerOnEncoder);

  registerPin(pin1, kTypeEncoder); registerPin(pin2, kTypeEncoder);    
  encodersRegistered++;
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus,F("Added encoder"));
#endif
}

void ClearEncoders() 
{
  clearRegisteredPins(kTypeEncoder);
  encodersRegistered = 0;
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus,F("Cleared encoders"));
#endif  
}

//// OUTPUTS /////

#if MF_SEGMENT_SUPPORT == 1
//// SEGMENTS /////
void AddLedSegment(int dataPin, int csPin, int clkPin, int numDevices, int brightness)
{
  if (ledSegmentsRegistered == MAX_LEDSEGMENTS) return;
  
  if (isPinRegistered(dataPin) || isPinRegistered(clkPin) || isPinRegistered(csPin)) return;
  
  ledSegments[ledSegmentsRegistered].attach(dataPin,csPin,clkPin,numDevices,brightness); // lc is our object
  
  registerPin(dataPin, kTypeLedSegment);
  registerPin(csPin, kTypeLedSegment);  
  registerPin(clkPin, kTypeLedSegment);
  ledSegmentsRegistered++;
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus,F("Added Led Segment"));
#endif  
}

void ClearLedSegments()
{
  clearRegisteredPins(kTypeLedSegment);
  for (int i=0; i!=ledSegmentsRegistered; i++) {
    ledSegments[ledSegmentsRegistered].detach();
  }
  ledSegmentsRegistered = 0;
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus, F("Cleared segments"));
#endif  
}

void PowerSaveLedSegment(bool state)
{
  for (int i=0; i!= ledSegmentsRegistered; ++i) {
    ledSegments[i].powerSavingMode(state);
  }
  
  for (int i=0; i!= outputsRegistered; ++i) {
    outputs[i].powerSavingMode(state);
  }
}
#endif

#if MF_STEPPER_SUPPORT == 1
//// STEPPER ////
void AddStepper(int pin1, int pin2, int pin3, int pin4, int btnPin1)
{
  if (steppersRegistered == MAX_STEPPERS) return;
  if (isPinRegistered(pin1) || isPinRegistered(pin2) || isPinRegistered(pin3) || isPinRegistered(pin4) 
  || (btnPin1 > 0 && isPinRegistered(btnPin1))) {
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus,F("Conflict with stepper"));
#endif 
    return;
  }

  steppers[steppersRegistered] = new MFStepper(pin1, pin2, pin3, pin4, btnPin1); // is our object 
  steppers[steppersRegistered]->setMaxSpeed(STEPPER_SPEED);
  steppers[steppersRegistered]->setAcceleration(STEPPER_ACCEL);

  registerPin(pin1, kTypeStepper); registerPin(pin2, kTypeStepper); registerPin(pin3, kTypeStepper); registerPin(pin4, kTypeStepper); 
  // autoreset is not released yet
  if (btnPin1>0) {
    registerPin(btnPin1, kTypeStepper);
    // this triggers the auto reset if we need to reset
    steppers[steppersRegistered]->reset();
  }

  // all set
  steppersRegistered++;
  
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus,F("Added stepper"));
#endif 
}

void ClearSteppers()
{
  for (int i=0; i!=steppersRegistered; i++) 
  {
    delete steppers[steppersRegistered];
  }  
  clearRegisteredPins(kTypeStepper);
  steppersRegistered = 0;
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus,F("Cleared steppers"));
#endif 
}
#endif

#if MF_SERVO_SUPPORT == 1
//// SERVOS /////
void AddServo(int pin)
{
  if (servosRegistered == MAX_MFSERVOS) return;
  if (isPinRegistered(pin)) return;
  
  servos[servosRegistered].attach(pin, true);
  registerPin(pin, kTypeServo);
  servosRegistered++;
}

void ClearServos()
{
  for (int i=0; i!=servosRegistered; i++) 
  {
    servos[servosRegistered].detach();
  }  
  clearRegisteredPins(kTypeServo);
  servosRegistered = 0;
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus,F("Cleared servos"));
#endif 
}
#endif

#if MF_LCD_SUPPORT == 1
//// LCD Display /////
void AddLcdDisplay (uint8_t address = 0x24, uint8_t cols = 16, uint8_t lines = 2, char const * name = "LCD")
{  
  if (lcd_12cRegistered == MAX_MFLCD_I2C) return;
  lcd_I2C[lcd_12cRegistered].attach(address, cols, lines);
  lcd_12cRegistered++;
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus,F("Added lcdDisplay"));
#endif
}

void ClearLcdDisplays()
{
  for (int i=0; i!=lcd_12cRegistered; i++) 
  {
    lcd_I2C[lcd_12cRegistered].detach();
  }  
  
  lcd_12cRegistered = 0;
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus,F("Cleared lcdDisplays"));
#endif 
}
#endif
  

//// EVENT HANDLER /////
void handlerOnRelease(uint8_t eventId, uint8_t pin, const char * name)
{
  cmdMessenger.sendCmdStart(kButtonChange);
  cmdMessenger.sendCmdArg(name);
  cmdMessenger.sendCmdArg(eventId);
  cmdMessenger.sendCmdEnd();
};

//// EVENT HANDLER /////
void handlerOnEncoder(uint8_t eventId, uint8_t pin, const char * name)
{
  cmdMessenger.sendCmdStart(kEncoderChange);
  cmdMessenger.sendCmdArg(name);
  cmdMessenger.sendCmdArg(eventId);
  cmdMessenger.sendCmdEnd();
};

/**
 ** config stuff
 **/
void OnSetConfig() 
{
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus,F("Setting config start"));
#endif    

  lastCommand = millis();
  String cfg = cmdMessenger.readStringArg();
  int cfgLen = cfg.length();
  int bufferSize = MEM_LEN_CONFIG - (configLength+cfgLen);
  
  if (bufferSize>1) {    
    cfg.toCharArray(&configBuffer[configLength], bufferSize);
    configLength += cfgLen;
    cmdMessenger.sendCmd(kStatus,configLength);
  } else
    cmdMessenger.sendCmd(kStatus,-1);  
#ifdef DEBUG  
  cmdMessenger.sendCmd(kStatus,F("Setting config end"));
#endif
}

void resetConfig()
{
  ClearButtons();
  ClearEncoders();
  ClearOutputs();

#if MF_SEGMENT_SUPPORT == 1
  ClearLedSegments();
#endif

#if MF_SERVO_SUPPORT == 1
  ClearServos();
#endif 

#if MF_STEPPER_SUPPORT == 1
  ClearSteppers();
#endif 

#if MF_LCD_SUPPORT == 1
  ClearLcdDisplays();
#endif

  configLength = 0;
  configActivated = false;
}

void OnResetConfig()
{
  resetConfig();
  cmdMessenger.sendCmd(kStatus, F("OK") );
}

void OnSaveConfig()
{
  _storeConfig();
  cmdMessenger.sendCmd(kConfigSaved, F("OK"));
}

void OnActivateConfig() 
{
  readConfig(configBuffer);
  _activateConfig();
  //cmdMessenger.sendCmd(kConfigActivated, F("OK"));
}

void _activateConfig() {
  configActivated = true;
  cmdMessenger.sendCmd(kConfigActivated, F("OK"));
}

void readConfig(String cfg) {
  char readBuffer[MEM_LEN_CONFIG+1] = "";
  char *p = NULL;
  cfg.toCharArray(readBuffer, MEM_LEN_CONFIG);

  char *command = strtok_r(readBuffer, ".", &p);
  char *params[6];
  if (*command == 0) return;

  do {    
    switch (atoi(command)) {
      case kTypeButton:
        params[0] = strtok_r(NULL, ".", &p); // pin
        params[1] = strtok_r(NULL, ":", &p); // name
        AddButton(atoi(params[0]), params[1]);
        break; 

      case kTypeOutput:
        params[0] = strtok_r(NULL, ".", &p); // pin
        params[1] = strtok_r(NULL, ":", &p); // Name
        AddOutput(atoi(params[0]), params[1]);
        break;

      case kTypeLedSegment:      
        params[0] = strtok_r(NULL, ".", &p); // pin Data
        params[1] = strtok_r(NULL, ".", &p); // pin Cs
        params[2] = strtok_r(NULL, ".", &p); // pin Clk
        params[3] = strtok_r(NULL, ".", &p); // brightness
        params[4] = strtok_r(NULL, ".", &p); // numModules
        params[5] = strtok_r(NULL, ":", &p); // Name
        // int dataPin, int clkPin, int csPin, int numDevices, int brightness
#if MF_SEGMENT_SUPPORT == 1
        AddLedSegment(atoi(params[0]), atoi(params[1]), atoi(params[2]), atoi(params[4]), atoi(params[3]));
#endif
      break;
      
      case kTypeStepperDeprecated:
        // this is for backwards compatibility
        params[0] = strtok_r(NULL, ".", &p); // pin1
        params[1] = strtok_r(NULL, ".", &p); // pin2
        params[2] = strtok_r(NULL, ".", &p); // pin3
        params[3] = strtok_r(NULL, ".", &p); // pin4
        params[4] = strtok_r(NULL, ".", &p); // btnPin1
        params[5] = strtok_r(NULL, ":", &p); // Name
#if MF_STEPPER_SUPPORT == 1
        AddStepper(atoi(params[0]), atoi(params[1]), atoi(params[2]), atoi(params[3]), 0);
#endif
      break;

      case kTypeStepper:
        // AddStepper(int pin1, int pin2, int pin3, int pin4)
        params[0] = strtok_r(NULL, ".", &p); // pin1
        params[1] = strtok_r(NULL, ".", &p); // pin2
        params[2] = strtok_r(NULL, ".", &p); // pin3
        params[3] = strtok_r(NULL, ".", &p); // pin4
        params[4] = strtok_r(NULL, ".", &p); // btnPin1
        params[5] = strtok_r(NULL, ":", &p); // Name
#if MF_STEPPER_SUPPORT == 1
        AddStepper(atoi(params[0]), atoi(params[1]), atoi(params[2]), atoi(params[3]), atoi(params[4]));
#endif
      break;
      
      case kTypeServo:
        // AddServo(int pin)
        params[0] = strtok_r(NULL, ".", &p); // pin1
        params[1] = strtok_r(NULL, ":", &p); // Name
#if MF_SERVO_SUPPORT == 1
        AddServo(atoi(params[0]));
#endif
      break;
      
	  case kTypeEncoderSingleDetent:
		  // AddEncoder(uint8_t pin1 = 1, uint8_t pin2 = 2, uint8_t encoder_type = 0, String name = "Encoder")
		  params[0] = strtok_r(NULL, ".", &p); // pin1
		  params[1] = strtok_r(NULL, ".", &p); // pin2
		  params[2] = strtok_r(NULL, ":", &p); // Name
		  AddEncoder(atoi(params[0]), atoi(params[1]), 0, params[2]);
		  break;

	  case kTypeEncoder:
		  // AddEncoder(uint8_t pin1 = 1, uint8_t pin2 = 2, uint8_t encoder_type = 0, String name = "Encoder")
		  params[0] = strtok_r(NULL, ".", &p); // pin1
		  params[1] = strtok_r(NULL, ".", &p); // pin2
		  params[2] = strtok_r(NULL, ".", &p); // encoder_type
		  params[3] = strtok_r(NULL, ":", &p); // Name
		  AddEncoder(atoi(params[0]), atoi(params[1]), atoi(params[2]), params[3]);
		  break;

      case kTypeLcdDisplayI2C:
        // AddEncoder(uint8_t address = 0x24, uint8_t cols = 16, lines = 2, String name = "Lcd")
        params[0] = strtok_r(NULL, ".", &p); // address
        params[1] = strtok_r(NULL, ".", &p); // cols
        params[2] = strtok_r(NULL, ".", &p); // lines
        params[3] = strtok_r(NULL, ":", &p); // Name
#if MF_LCD_SUPPORT == 1
        AddLcdDisplay(atoi(params[0]), atoi(params[1]), atoi(params[2]), params[3]);
#endif
      break;
        
      default:
        // read to the end of the current command which is
        // apparently not understood
        params[0] = strtok_r(NULL, ":", &p); // read to end of unknown command
    }    
    command = strtok_r(NULL, ".", &p);  
  } while (command!=NULL);
}

// Called when a received command has no attached function
void OnUnknownCommand()
{
  lastCommand = millis();
  cmdMessenger.sendCmd(kStatus,F("n/a"));
}

void OnGetInfo() {
  lastCommand = millis();
  cmdMessenger.sendCmdStart(kInfo);
  cmdMessenger.sendCmdArg(type);
  cmdMessenger.sendCmdArg(name);
  cmdMessenger.sendCmdArg(serial);
  cmdMessenger.sendCmdArg(version);
  cmdMessenger.sendCmdEnd();
}

void OnGetConfig() 
{
  lastCommand = millis();
  cmdMessenger.sendCmdStart(kInfo);
  cmdMessenger.sendCmdArg(configBuffer);
  cmdMessenger.sendCmdEnd();
}

// Callback function that sets led on or off
void OnSetPin()
{
  // Read led state argument, interpret string as boolean
  int pin = cmdMessenger.readIntArg();
  int state = cmdMessenger.readIntArg();
  // Set led
  digitalWrite(pin, state > 0 ? HIGH : LOW);
  lastCommand = millis();
}

#if MF_SEGMENT_SUPPORT == 1
void OnInitModule()
{
  int module = cmdMessenger.readIntArg();
  int subModule = cmdMessenger.readIntArg();  
  int brightness = cmdMessenger.readIntArg();
  ledSegments[module].setBrightness(brightness);
  lastCommand = millis();
}

void OnSetModule()
{
  int module = cmdMessenger.readIntArg();
  int subModule = cmdMessenger.readIntArg();  
  char * value = cmdMessenger.readStringArg();
  uint8_t points = (uint8_t) cmdMessenger.readIntArg();
  uint8_t mask = (uint8_t) cmdMessenger.readIntArg();
  ledSegments[module].display(subModule, value, points, mask);
  lastCommand = millis();
}

void OnSetModuleBrightness()
{
  int module = cmdMessenger.readIntArg();
  int brightness = cmdMessenger.readIntArg();  
  ledSegments[module].setBrightness(brightness);      
  lastCommand = millis();
}
#endif

#if MF_STEPPER_SUPPORT == 1
void OnSetStepper()
{
  int stepper    = cmdMessenger.readIntArg();
  long newPos    = cmdMessenger.readLongArg();
  
  if (stepper >= steppersRegistered) return;
  steppers[stepper]->moveTo(newPos);
  lastCommand = millis();
}

void OnResetStepper()
{
  int stepper    = cmdMessenger.readIntArg();
  
  if (stepper >= steppersRegistered) return;
  steppers[stepper]->reset();
  lastCommand = millis();
}

void OnSetZeroStepper()
{
  int stepper    = cmdMessenger.readIntArg();
  
  if (stepper >= steppersRegistered) return;
  steppers[stepper]->setZero();
  lastCommand = millis();
}

void updateSteppers()
{
  for (int i=0; i!=steppersRegistered; i++) {
    steppers[i]->update();
  }
}
#endif

#if MF_SERVO_SUPPORT == 1
void OnSetServo()
{ 
  int servo    = cmdMessenger.readIntArg();
  int newValue = cmdMessenger.readIntArg();
  if (servo >= servosRegistered) return;
  servos[servo].moveTo(newValue);  
  lastCommand = millis();
}

void updateServos()
{
  for (int i=0; i!=servosRegistered; i++) {
    servos[i].update();
  }
}
#endif

#if MF_LCD_SUPPORT == 1
void OnSetLcdDisplayI2C()
{
  int address  = cmdMessenger.readIntArg();
  char *output   = cmdMessenger.readStringArg();
  lcd_I2C[address].display(output);
  cmdMessenger.sendCmd(kStatus, output);
  lastCommand = millis();
}
#endif

void readButtons()
{
  for(int i=0; i!=buttonsRegistered; i++) {
    buttons[i].update();
  }  
}

void readEncoder() 
{
  for(int i=0; i!=encodersRegistered; i++) {
    encoders[i].update();
  }
}

void OnGenNewSerial() 
{
  generateSerial(true);
  cmdMessenger.sendCmdStart(kInfo);
  cmdMessenger.sendCmdArg(serial);
  cmdMessenger.sendCmdEnd();
}

void OnSetName() {
  String cfg = cmdMessenger.readStringArg();
  cfg.toCharArray(&name[0], MEM_LEN_NAME);
  _storeName();
  cmdMessenger.sendCmdStart(kStatus);
  cmdMessenger.sendCmdArg(name);
  cmdMessenger.sendCmdEnd();
}

void _storeName() {
  char prefix[] = "#";
  EEPROM.writeBlock<char>(MEM_OFFSET_NAME, prefix, 1);
  EEPROM.writeBlock<char>(MEM_OFFSET_NAME+1, name, MEM_LEN_NAME-1);
}

void _restoreName() {
  char testHasName[1] = "";
  EEPROM.readBlock<char>(MEM_OFFSET_NAME, testHasName, 1); 
  if (testHasName[0] != '#') return;
  
  EEPROM.readBlock<char>(MEM_OFFSET_NAME+1, name, MEM_LEN_NAME-1); 
}

void OnTrigger()
{
  for(int i=0; i!=buttonsRegistered; i++) {
    buttons[i].trigger();
  }  
}
