#ifndef MFCore_H
#define MFCore_H

#include <stdlib.h>
#if ARDUINO >= 100
#include <Arduino.h>
#else
#include <WProgram.h>
#include <wiring.h>
#endif

enum
{
  kTypeNotSet,      // 0 
  kTypeButton,      // 1
  kTypeEncoder,     // 2
  kTypeOutput,      // 3
  kTypeLedSegment,  // 4
  kTypeStepper,     // 5
  kTypeServo,       // 6
  kTypeLcdDisplayI2C,       // 7
  kTypeEncoder,             // 8
  kTypeStepper,             // 9 (new stepper type with auto zero support if btnPin is > 0)
  kShiftRegister,           // 10 Shift register support (example: 74HC595, TLC592X)
  kTypeAnalogInput,         // 11 Analog Device with 1 pin
  kTypeSPIOutput            // 12 SPI Output Device, like a digital Potentiometer
};

// This is the list of recognized commands. These can be commands that can either be sent or received. 
// In order to receive, attach a callback function to these events
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
  kTrigger,             // 23
  kResetBoard,              // 24
  kSetLcdDisplayI2C,        // 25
  kSetModuleBrightness,     // 26
  kSetShiftRegisterPins,    // 27
  kAnalogChange,             // 28
  kSetSPIOutput             // 29
};



class MFCore
{
public:
    MFCore();
    void test();
    static void CallbackFunction();
    
private:
    bool          _state;
};
#endif