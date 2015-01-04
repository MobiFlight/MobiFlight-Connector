// MFServo.h
//
/// \mainpage MF Servo module for MobiFlight Framework
/// \par Revision History
/// \version 1.0 Initial release
/// \author  Sebastian Moebius (mobiflight@moebiuz.de) DO NOT CONTACT THE AUTHOR DIRECTLY: USE THE LISTS
// Copyright (C) 2013-2014 Sebastian Moebius

#ifndef MFServo_h
#define MFServo_h

#include <stdlib.h>
#include "../Servo/Servo.h"

#if ARDUINO >= 100
#include <Arduino.h>
#else
#include <WProgram.h>
#endif

/////////////////////////////////////////////////////////////////////
/// \class MFServo MFServo.h <MFServo.h>
class MFServo
{
public:
		MFServo();		
    MFServo(uint8_t pin, bool enable = true);		
		void    attach(uint8_t pin = 1, bool enable = true);
    void    detach();
		void		setExternalRange(int min, int max);
		void    setInternalRange(int min, int max);
    void    moveTo(int absolute);

private:
		uint8_t _pin;
		int			_mapRange[4];
    bool    _initialized;
    Servo   _servo;
    long    _targetPos;
};
#endif 
