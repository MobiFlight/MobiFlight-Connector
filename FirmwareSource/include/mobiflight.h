#ifndef _mobiflight_h
#define _mobiflight_h

#include <MFEncoder.h>
#include <MFAnalog.h>

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
  kTypeStepper,             // 9 (new stepper type with auto zero support if btnPin is > 0)
  kShiftRegister,           // 10 Shift register support (example: 74HC595, TLC592X)
  kTypeAnalogInput,         // 11 Analog Device with 1 pin
  kInputShifter             // 12 Input shift register support (example: 74HC165)
};

// This is the list of recognized commands. These can be commands that can either be sent or received.
// In order to receive, attach a callback function to these events
//
// If you increase this list, make sure to check that the MAXCALLBACKS value
// in CmdMessenger.h is set apropriately
enum
{
  kInitModule,           // 0
  kSetModule,            // 1
  kSetPin,               // 2
  kSetStepper,           // 3
  kSetServo,             // 4
  kStatus,               // 5, Command to report status
  kEncoderChange,        // 6
  kButtonChange,         // 7
  kStepperChange,        // 8
  kGetInfo,              // 9
  kInfo,                 // 10
  kSetConfig,            // 11
  kGetConfig,            // 12
  kResetConfig,          // 13
  kSaveConfig,           // 14
  kConfigSaved,          // 15
  kActivateConfig,       // 16
  kConfigActivated,      // 17
  kSetPowerSavingMode,   // 18
  kSetName,              // 19
  kGenNewSerial,         // 20
  kResetStepper,         // 21
  kSetZeroStepper,       // 22
  kTrigger,              // 23
  kResetBoard,           // 24
  kSetLcdDisplayI2C,     // 25
  kSetModuleBrightness,  // 26
  kSetShiftRegisterPins, // 27
  kAnalogChange          // 28
};

void attachCommandCallbacks();
void OnResetBoard();
void generateSerial(bool force);
void loadConfig();
void _storeConfig();
void SetPowerSavingMode(bool state);
void updatePowerSaving();
bool isPinRegistered(uint8_t pin);
bool isPinRegisteredForType(uint8_t pin, uint8_t type);
void registerPin(uint8_t pin, uint8_t type);
void clearRegisteredPins(uint8_t type);
void clearRegisteredPins();
void AddOutput(uint8_t pin, char const *name);
void ClearOutputs();
void AddButton(uint8_t pin, char const *name, bool repeat);
void ClearButtons();
void AddEncoder(uint8_t pin1, uint8_t pin2, uint8_t encoder_type, char const *name);
void ClearEncoders();
void AddLedSegment(int dataPin, int csPin, int clkPin, int numDevices, int brightness);
void ClearLedSegments();
void PowerSaveLedSegment(bool state);
void AddStepper(int pin1, int pin2, int pin3, int pin4, int btnPin1);
void ClearSteppers();
void AddServo(int pin);
void ClearServos();
void AddLcdDisplay(uint8_t address, uint8_t cols, uint8_t lines, char const *name);
void ClearLcdDisplays();
void handlerOnRelease(uint8_t eventId, uint8_t pin, const char *name);
void handlerOnEncoder(uint8_t eventId, uint8_t pin, const char *name);
void OnSetConfig();
void resetConfig();
void OnResetConfig();
void OnSaveConfig();
void OnActivateConfig();
void _activateConfig();
void readConfig(String cfg);
void OnUnknownCommand();
void OnGetInfo();
void OnGetConfig();
void OnSetPin();
void OnInitModule();
void OnSetModule();
void OnSetModuleBrightness();
void OnSetStepper();
void OnResetStepper();
void OnSetZeroStepper();
void updateSteppers();
void OnSetServo();
void updateServos();
void OnSetLcdDisplayI2C();
void readButtons();
void readEncoder();
void OnGenNewSerial();
void OnSetName();
void _storeName();
void _restoreName();
void OnTrigger();
void readAnalog();
void AddAnalog(uint8_t pin, char const *name, uint8_t sensitivity);
void ClearAnalog();
void handlerOnAnalogChange(int value, uint8_t pin, const char *name);
void OnInitShiftRegister();
void OnSetShiftRegisterPins();
void AddShifter(uint8_t latchPin, uint8_t clockPin, uint8_t dataPin, uint8_t modules, char const *name);
void AddInputShifter(uint8_t latchPin, uint8_t clockPin, uint8_t dataPin, uint8_t modules, char const *name);

// RK added
extern uint8_t encodersRegistered;
extern MFEncoder encoders[];
void readConfig_reduced(char buffer[]);

#endif