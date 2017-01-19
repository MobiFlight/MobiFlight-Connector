// MFButton.h
//
/// \mainpage MF Button module for MobiFlight Framework
/// \par Revision History
/// \version 1.0 Initial release
/// \author  Sebastian Moebius (mobiflight@moebiuz.de) DO NOT CONTACT THE AUTHOR DIRECTLY: USE THE LISTS
// Copyright (C) 2013-2014 Sebastian Moebius

#ifndef MFOutput_h
#define MFOutput_h

#include <stdlib.h>
#if ARDUINO >= 100
#include <Arduino.h>
#else
#include <WProgram.h>
#include <wiring.h>
#endif

/////////////////////////////////////////////////////////////////////
/// \class MFOutput MFOutput.h <MFOutput.h>
class MFOutput
{
public:
    MFOutput(uint8_t pin = 1);
    void set(bool state);
    void powerSavingMode(bool state);
    
private:
    uint8_t       _pin;
    bool          _state;
};
#endif 