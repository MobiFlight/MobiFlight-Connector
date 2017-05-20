// MFLCDDisplay.h
//
/// \mainpage MFLCDDisplay module for MobiFlight Framework
/// \par Revision History
/// \version 1.0 Initial release
/// \author  Sebastian Moebius (info@mobiflight.com) DO NOT CONTACT THE AUTHOR DIRECTLY: USE THE LISTS
// Copyright (C) 2013-2014 Sebastian Moebius

#ifndef MFLCDDisplay_h
#define MFLCDDisplay_h

#if ARDUINO >= 100
#include <Arduino.h>
#else
#include <WProgram.h>
#include <wiring.h>
#endif

//#include "../NewliquidCrystal/LiquidCrystal_I2C.h"
#include "../LiquidCrystal-I2C/LiquidCrystal_I2C.h"


/////////////////////////////////////////////////////////////////////
/// \class MFLCDDisplay MFLCDDisplay.h <MFLCDDisplay.h>
class MFLCDDisplay
{
public:
    MFLCDDisplay();
    void display(char *string);
    void attach(byte address, byte cols, byte lines);
    void detach();
    void test();
    void powerSavingMode(bool state);
    
private:
    LiquidCrystal_I2C  *_lcdDisplay;
    bool        _initialized;
    byte        _address;
	byte		_cols;
	byte		_lines;
};
#endif 