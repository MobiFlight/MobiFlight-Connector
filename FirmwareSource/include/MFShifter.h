// MFSegments.h
//
/// \mainpage MF ShiftArray module for MobiFlight Framework
/// \par Revision History
/// \version 1.0 Initial release
/// \author  Manfred Berry (manfred@nystedberry.info) DO NOT CONTACT THE AUTHOR DIRECTLY: USE THE LISTS
// Copyright (C) 2021 Manfred Berry

#ifndef MFShifter_h
#define MFShifter_h

#if ARDUINO >= 100
#include <Arduino.h>
#else
#include <WProgram.h>
#include <wiring.h>
#endif

/////////////////////////////////////////////////////////////////////
/// \class MFShifter MFShifter.h <MFShifter.h>
class MFShifter
{
public:
    MFShifter();
    void setPin(uint8_t pin, int value);
    void setPins(char* pins, int value);
    void attach(uint8_t latchPin, uint8_t clockPin, uint8_t dataPin, uint8_t moduleCount);
    void detach();
    void clear();
    void test();
    void updateShiftRegister();
    
private:
    uint8_t _latchPin;	    // Latch pin
    uint8_t _clockPin;	    // Clock pin
    uint8_t _dataPin;	    // Data/SI pin
    uint8_t _moduleCount;   // Number of 8 bit modules in series. For a shift register with 16 bit one needs to select 2 modules a 8......
    unsigned long _output;  // Max 4 modules for now = 32 bit
    bool _initialized = false;
};
#endif
