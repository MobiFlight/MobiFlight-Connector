// MFStepper.h
//
/// \mainpage MF Stepper module for MobiFlight Framework
/// \par Revision History
/// \version 1.0 Initial release
/// \author  Sebastian Moebius (mobiflight@moebiuz.de) DO NOT CONTACT THE AUTHOR DIRECTLY: USE THE LISTS
// Copyright (C) 2013-2014 Sebastian Moebius

#ifndef MFStepper_h
#define MFStepper_h

#include <stdlib.h>
#include "../AccelStepper/AccelStepper.h"

#if ARDUINO >= 100
#include <Arduino.h>
#else
#include <WProgram.h>
#include <wiring.h>
#endif

/////////////////////////////////////////////////////////////////////
/// \class MFStepper MFStepper.h <MFServo.h>
class MFStepper
{
public:
    MFStepper(uint8_t pin1 = 1, uint8_t pin2 = 2, uint8_t pin3 = 3, uint8_t pin4 = 4);		
		void    update();
    void    moveTo(long absolute);

private:
    bool    			_initialized;
    AccelStepper  _stepper;
    long    			_targetPos;
};
#endif 