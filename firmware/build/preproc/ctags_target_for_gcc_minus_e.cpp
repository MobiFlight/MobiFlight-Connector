# 1 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
# 1 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
/**

 * Includes Core Arduino functionality 

 **/
# 4 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
char foo;



# 9 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2


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
const char version[8] = "1.9.3";

//#define DEBUG 1
# 95 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
# 96 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 97 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 98 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 99 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 100 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 101 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 102 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 103 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 104 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 105 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 106 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 107 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 108 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 109 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 110 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 111 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2
# 112 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 2

const uint8_t MEM_OFFSET_NAME = 0;
const uint8_t MEM_LEN_NAME = 48;
const uint8_t MEM_OFFSET_SERIAL = MEM_OFFSET_NAME + MEM_LEN_NAME;
const uint8_t MEM_LEN_SERIAL = 11;
const uint8_t MEM_OFFSET_CONFIG = MEM_OFFSET_NAME + MEM_LEN_NAME + MEM_LEN_SERIAL;


char type[20] = "MobiFlight Mega";
char serial[MEM_LEN_SERIAL] = "1234567890";
char name[MEM_LEN_NAME] = "MobiFlight Mega";
int eepromSize = 4096;
const int MEM_LEN_CONFIG = 1024;
# 143 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
char configBuffer[MEM_LEN_CONFIG] = "";

int configLength = 0;
boolean configActivated = false;

bool powerSavingMode = false;
uint8_t pinsRegistered[68];
const unsigned long POWER_SAVING_TIME = 60*15; // in seconds

CmdMessenger cmdMessenger = CmdMessenger(Serial);
unsigned long lastCommand;

MFOutput outputs[40];
uint8_t outputsRegistered = 0;

MFButton buttons[50];
uint8_t buttonsRegistered = 0;

MFSegments ledSegments[4];
uint8_t ledSegmentsRegistered = 0;

MFEncoder encoders[20];
uint8_t encodersRegistered = 0;

MFStepper *steppers[10]; //
uint8_t steppersRegistered = 0;

MFServo servos[10];
uint8_t servosRegistered = 0;

MFLCDDisplay lcd_I2C[2];
uint8_t lcd_12cRegistered = 0;

enum
{
  kTypeNotSet, // 0 
  kTypeButton, // 1
  kTypeEncoderSingleDetent, // 2 (retained for backwards compatibility, use kTypeEncoder for new configs)
  kTypeOutput, // 3
  kTypeLedSegment, // 4
  kTypeStepperDeprecated, // 5 (keep for backwards compatibility, doesn't support autohome)
  kTypeServo, // 6
  kTypeLcdDisplayI2C, // 7
  kTypeEncoder, // 8
  kTypeStepper // 9 (new stepper type with auto zero support if btnPin is > 0)
};

// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
enum
{
  kInitModule, // 0
  kSetModule, // 1
  kSetPin, // 2
  kSetStepper, // 3
  kSetServo, // 4
  kStatus, // 5, Command to report status
  kEncoderChange, // 6  
  kButtonChange, // 7
  kStepperChange, // 8
  kGetInfo, // 9
  kInfo, // 10
  kSetConfig, // 11
  kGetConfig, // 12
  kResetConfig, // 13
  kSaveConfig, // 14
  kConfigSaved, // 15
  kActivateConfig, // 16
  kConfigActivated, // 17
  kSetPowerSavingMode, // 18  
  kSetName, // 19
  kGenNewSerial, // 20
  kResetStepper, // 21
  kSetZeroStepper, // 22
  kTrigger, // 23
  kResetBoard, // 24
  kSetLcdDisplayI2C, // 25
};

// Callbacks define on which received commands we take action
void attachCommandCallbacks()
{
  // Attach callback methods
  cmdMessenger.attach(OnUnknownCommand);
  cmdMessenger.attach(kInitModule, OnInitModule);
  cmdMessenger.attach(kSetModule, OnSetModule);
  cmdMessenger.attach(kSetPin, OnSetPin);
  cmdMessenger.attach(kSetStepper, OnSetStepper);
  cmdMessenger.attach(kSetServo, OnSetServo);
  cmdMessenger.attach(kGetInfo, OnGetInfo);
  cmdMessenger.attach(kGetConfig, OnGetConfig);
  cmdMessenger.attach(kSetConfig, OnSetConfig);
  cmdMessenger.attach(kResetConfig, OnResetConfig);
  cmdMessenger.attach(kSaveConfig, OnSaveConfig);
  cmdMessenger.attach(kActivateConfig, OnActivateConfig);
  cmdMessenger.attach(kSetName, OnSetName);
  cmdMessenger.attach(kGenNewSerial, OnGenNewSerial);
  cmdMessenger.attach(kResetStepper, OnResetStepper);
  cmdMessenger.attach(kSetZeroStepper, OnSetZeroStepper);
  cmdMessenger.attach(kTrigger, OnTrigger);
  cmdMessenger.attach(kResetBoard, OnResetBoard);
  cmdMessenger.attach(kSetLcdDisplayI2C, OnSetLcdDisplayI2C);




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
  OnResetBoard();
  cmdMessenger.printLfCr();
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
  PowerSaveLedSegment(state);






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
  updateSteppers();
  updateServos();
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
  for(int i=0; i!=68;++i)
    if (pinsRegistered[i] == type)
      pinsRegistered[i] = kTypeNotSet;
}

void clearRegisteredPins() {
  for(int i=0; i!=68;++i)
    pinsRegistered[i] = kTypeNotSet;
}

//// OUTPUT /////
void AddOutput(uint8_t pin = 1, char const * name = "Output")
{
  if (outputsRegistered == 40) return;
  if (isPinRegistered(pin)) return;

  outputs[outputsRegistered] = MFOutput(pin);
  registerPin(pin, kTypeOutput);
  outputsRegistered++;



}

void ClearOutputs()
{
  clearRegisteredPins(kTypeOutput);
  outputsRegistered = 0;



}

//// BUTTONS /////
void AddButton(uint8_t pin = 1, char const * name = "Button")
{
  if (buttonsRegistered == 50) return;

  if (isPinRegistered(pin)) return;

  buttons[buttonsRegistered] = MFButton(pin, name);
  buttons[buttonsRegistered].attachHandler(btnOnRelease, handlerOnRelease);
  buttons[buttonsRegistered].attachHandler(btnOnPress, handlerOnRelease);

  registerPin(pin, kTypeButton);
  buttonsRegistered++;



}

void ClearButtons()
{
  clearRegisteredPins(kTypeButton);
  buttonsRegistered = 0;



}

//// ENCODERS /////
void AddEncoder(uint8_t pin1 = 1, uint8_t pin2 = 2, uint8_t encoder_type = 0, char const * name = "Encoder")
{
  if (encodersRegistered == 20) return;
  if (isPinRegistered(pin1) || isPinRegistered(pin2)) return;

  encoders[encodersRegistered] = MFEncoder();
  encoders[encodersRegistered].attach(pin1, pin2, encoder_type, name);
  encoders[encodersRegistered].attachHandler(encLeft, handlerOnEncoder);
  encoders[encodersRegistered].attachHandler(encLeftFast, handlerOnEncoder);
  encoders[encodersRegistered].attachHandler(encRight, handlerOnEncoder);
  encoders[encodersRegistered].attachHandler(encRightFast, handlerOnEncoder);

  registerPin(pin1, kTypeEncoder); registerPin(pin2, kTypeEncoder);
  encodersRegistered++;



}

void ClearEncoders()
{
  clearRegisteredPins(kTypeEncoder);
  encodersRegistered = 0;



}

//// OUTPUTS /////

//// SEGMENTS /////
void AddLedSegment(int dataPin, int csPin, int clkPin, int numDevices, int brightness)
{
  if (ledSegmentsRegistered == 4) return;

  if (isPinRegistered(dataPin) || isPinRegistered(clkPin) || isPinRegistered(csPin)) return;

  ledSegments[ledSegmentsRegistered].attach(dataPin,csPin,clkPin,numDevices,brightness); // lc is our object

  registerPin(dataPin, kTypeLedSegment);
  registerPin(csPin, kTypeLedSegment);
  registerPin(clkPin, kTypeLedSegment);
  ledSegmentsRegistered++;



}

void ClearLedSegments()
{
  clearRegisteredPins(kTypeLedSegment);
  for (int i=0; i!=ledSegmentsRegistered; i++) {
    ledSegments[ledSegmentsRegistered].detach();
  }
  ledSegmentsRegistered = 0;



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
//// STEPPER ////
void AddStepper(int pin1, int pin2, int pin3, int pin4, int btnPin1)
{
  if (steppersRegistered == 10) return;
  if (isPinRegistered(pin1) || isPinRegistered(pin2) || isPinRegistered(pin3) || isPinRegistered(pin4)
  || (btnPin1 > 0 && isPinRegistered(btnPin1))) {



    return;
  }

  steppers[steppersRegistered] = new MFStepper(pin1, pin2, pin3, pin4, btnPin1); // is our object 
  steppers[steppersRegistered]->setMaxSpeed(400 /* 300 already worked, 467, too?*/);
  steppers[steppersRegistered]->setAcceleration(800);

  registerPin(pin1, kTypeStepper); registerPin(pin2, kTypeStepper); registerPin(pin3, kTypeStepper); registerPin(pin4, kTypeStepper);
  // autoreset is not released yet
  if (btnPin1>0) {
    registerPin(btnPin1, kTypeStepper);
    // this triggers the auto reset if we need to reset
    steppers[steppersRegistered]->reset();
  }

  // all set
  steppersRegistered++;




}

void ClearSteppers()
{
  for (int i=0; i!=steppersRegistered; i++)
  {
    delete steppers[steppersRegistered];
  }
  clearRegisteredPins(kTypeStepper);
  steppersRegistered = 0;



}

//// SERVOS /////
void AddServo(int pin)
{
  if (servosRegistered == 10) return;
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



}

//// LCD Display /////
void AddLcdDisplay (uint8_t address = 0x24, uint8_t cols = 16, uint8_t lines = 2, char const * name = "LCD")
{
  if (lcd_12cRegistered == 2) return;
  lcd_I2C[lcd_12cRegistered].attach(address, cols, lines);
  lcd_12cRegistered++;



}

void ClearLcdDisplays()
{
  for (int i=0; i!=lcd_12cRegistered; i++)
  {
    lcd_I2C[lcd_12cRegistered].detach();
  }

  lcd_12cRegistered = 0;



}


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
# 608 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
void OnSetConfig()
{




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



}

void resetConfig()
{
  ClearButtons();
  ClearEncoders();
  ClearOutputs();
  ClearLedSegments();
  ClearServos();
  ClearSteppers();
  ClearLcdDisplays();
  configLength = 0;
  configActivated = false;
}

void OnResetConfig()
{
  resetConfig();
  cmdMessenger.sendCmd(kStatus, (reinterpret_cast<const __FlashStringHelper *>(
# 646 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3
                               (__extension__({static const char __c[] __attribute__((__progmem__)) = (
# 646 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                               "OK"
# 646 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3
                               ); &__c[0];}))
# 646 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                               )) );
}

void OnSaveConfig()
{
  _storeConfig();
  cmdMessenger.sendCmd(kConfigSaved, (reinterpret_cast<const __FlashStringHelper *>(
# 652 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3
                                    (__extension__({static const char __c[] __attribute__((__progmem__)) = (
# 652 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                    "OK"
# 652 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3
                                    ); &__c[0];}))
# 652 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                    )));
}

void OnActivateConfig()
{
  readConfig(configBuffer);
  _activateConfig();
  cmdMessenger.sendCmd(kConfigActivated, (reinterpret_cast<const __FlashStringHelper *>(
# 659 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3
                                        (__extension__({static const char __c[] __attribute__((__progmem__)) = (
# 659 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                        "OK"
# 659 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3
                                        ); &__c[0];}))
# 659 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                        )));
}

void _activateConfig() {
  configActivated = true;
}

void readConfig(String cfg) {
  char readBuffer[MEM_LEN_CONFIG+1] = "";
  char *p = 
# 668 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
           __null
# 668 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
               ;
  cfg.toCharArray(readBuffer, MEM_LEN_CONFIG);

  char *command = strtok_r(readBuffer, ".", &p);
  char *params[6];
  if (*command == 0) return;

  do {
    switch (atoi(command)) {
      case kTypeButton:
        params[0] = strtok_r(
# 678 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 678 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // pin
        params[1] = strtok_r(
# 679 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 679 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ":", &p); // name
        AddButton(atoi(params[0]), params[1]);
        break;

      case kTypeOutput:
        params[0] = strtok_r(
# 684 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 684 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // pin
        params[1] = strtok_r(
# 685 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 685 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ":", &p); // Name
        AddOutput(atoi(params[0]), params[1]);
        break;

      case kTypeLedSegment:
        params[0] = strtok_r(
# 690 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 690 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // pin Data
        params[1] = strtok_r(
# 691 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 691 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // pin Cs
        params[2] = strtok_r(
# 692 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 692 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // pin Clk
        params[3] = strtok_r(
# 693 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 693 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // brightness
        params[4] = strtok_r(
# 694 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 694 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // numModules
        params[5] = strtok_r(
# 695 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 695 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ":", &p); // Name
        // int dataPin, int clkPin, int csPin, int numDevices, int brightness
        AddLedSegment(atoi(params[0]), atoi(params[1]), atoi(params[2]), atoi(params[4]), atoi(params[3]));
      break;

      case kTypeStepperDeprecated:
        // this is for backwards compatibility
        params[0] = strtok_r(
# 702 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 702 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // pin1
        params[1] = strtok_r(
# 703 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 703 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // pin2
        params[2] = strtok_r(
# 704 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 704 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // pin3
        params[3] = strtok_r(
# 705 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 705 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // pin4
        params[4] = strtok_r(
# 706 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 706 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // btnPin1
        params[5] = strtok_r(
# 707 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 707 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ":", &p); // Name
        AddStepper(atoi(params[0]), atoi(params[1]), atoi(params[2]), atoi(params[3]), 0);
      break;

      case kTypeStepper:
        // AddStepper(int pin1, int pin2, int pin3, int pin4)
        params[0] = strtok_r(
# 713 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 713 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // pin1
        params[1] = strtok_r(
# 714 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 714 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // pin2
        params[2] = strtok_r(
# 715 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 715 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // pin3
        params[3] = strtok_r(
# 716 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 716 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // pin4
        params[4] = strtok_r(
# 717 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 717 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // btnPin1
        params[5] = strtok_r(
# 718 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 718 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ":", &p); // Name
        AddStepper(atoi(params[0]), atoi(params[1]), atoi(params[2]), atoi(params[3]), atoi(params[4]));
      break;

      case kTypeServo:
        // AddServo(int pin)
        params[0] = strtok_r(
# 724 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 724 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // pin1
        params[1] = strtok_r(
# 725 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 725 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ":", &p); // Name
        AddServo(atoi(params[0]));
      break;

   case kTypeEncoderSingleDetent:
    // AddEncoder(uint8_t pin1 = 1, uint8_t pin2 = 2, uint8_t encoder_type = 0, String name = "Encoder")
    params[0] = strtok_r(
# 731 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                        __null
# 731 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                            , ".", &p); // pin1
    params[1] = strtok_r(
# 732 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                        __null
# 732 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                            , ".", &p); // pin2
    params[2] = strtok_r(
# 733 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                        __null
# 733 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                            , ":", &p); // Name
    AddEncoder(atoi(params[0]), atoi(params[1]), 0, params[2]);
    break;

   case kTypeEncoder:
    // AddEncoder(uint8_t pin1 = 1, uint8_t pin2 = 2, uint8_t encoder_type = 0, String name = "Encoder")
    params[0] = strtok_r(
# 739 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                        __null
# 739 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                            , ".", &p); // pin1
    params[1] = strtok_r(
# 740 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                        __null
# 740 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                            , ".", &p); // pin2
    params[2] = strtok_r(
# 741 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                        __null
# 741 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                            , ".", &p); // encoder_type
    params[3] = strtok_r(
# 742 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                        __null
# 742 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                            , ":", &p); // Name
    AddEncoder(atoi(params[0]), atoi(params[1]), atoi(params[2]), params[3]);
    break;

      case kTypeLcdDisplayI2C:
        // AddEncoder(uint8_t address = 0x24, uint8_t cols = 16, lines = 2, String name = "Lcd")
        params[0] = strtok_r(
# 748 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 748 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // address
        params[1] = strtok_r(
# 749 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 749 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // cols
        params[2] = strtok_r(
# 750 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 750 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ".", &p); // lines
        params[3] = strtok_r(
# 751 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 751 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ":", &p); // Name
        AddLcdDisplay(atoi(params[0]), atoi(params[1]), atoi(params[2]), params[3]);
      break;

      default:
        // read to the end of the current command which is
        // apparently not understood
        params[0] = strtok_r(
# 758 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                            __null
# 758 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                                , ":", &p); // read to end of unknown command
    }
    command = strtok_r(
# 760 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                      __null
# 760 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                          , ".", &p);
  } while (command!=
# 761 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3 4
                   __null
# 761 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                       );
}

// Called when a received command has no attached function
void OnUnknownCommand()
{
  lastCommand = millis();
  cmdMessenger.sendCmd(kStatus,(reinterpret_cast<const __FlashStringHelper *>(
# 768 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3
                              (__extension__({static const char __c[] __attribute__((__progmem__)) = (
# 768 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                              "n/a"
# 768 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino" 3
                              ); &__c[0];}))
# 768 "d:\\projects\\MobiFlight\\FirmwareSource\\mobiflight\\mobiflight.ino"
                              )));
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
  digitalWrite(pin, state > 0 ? 0x1 : 0x0);
  lastCommand = millis();
}

void OnInitModule()
{
  int module = cmdMessenger.readIntArg();
  int subModule = cmdMessenger.readIntArg();
  int brightness = cmdMessenger.readIntArg();
  ledSegments[module].setBrightness(subModule,brightness);
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

void OnSetStepper()
{
  int stepper = cmdMessenger.readIntArg();
  long newPos = cmdMessenger.readLongArg();

  if (stepper >= steppersRegistered) return;
  steppers[stepper]->moveTo(newPos);
  lastCommand = millis();
}

void OnResetStepper()
{
  int stepper = cmdMessenger.readIntArg();

  if (stepper >= steppersRegistered) return;
  steppers[stepper]->reset();
  lastCommand = millis();
}

void OnSetZeroStepper()
{
  int stepper = cmdMessenger.readIntArg();

  if (stepper >= steppersRegistered) return;
  steppers[stepper]->setZero();
  lastCommand = millis();
}

void OnSetServo()
{
  int servo = cmdMessenger.readIntArg();
  int newValue = cmdMessenger.readIntArg();
  if (servo >= servosRegistered) return;
  servos[servo].moveTo(newValue);
  lastCommand = millis();
}

void OnSetLcdDisplayI2C()
{
  int address = cmdMessenger.readIntArg();
  char *output = cmdMessenger.readStringArg();
  lcd_I2C[address].display(output);
  cmdMessenger.sendCmd(kStatus, output);
  lastCommand = millis();
}

void updateSteppers()
{
  for (int i=0; i!=steppersRegistered; i++) {
    steppers[i]->update();
  }
}

void updateServos()
{
  for (int i=0; i!=servosRegistered; i++) {
    servos[i].update();
  }
}

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
