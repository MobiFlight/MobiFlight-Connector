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
#include "MFButton.h"

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
    MFStepper(uint8_t pin1 = 1, uint8_t pin2 = 2, uint8_t pin3 = 3, uint8_t pin4 = 4, uint8_t btnPin1 = 5);
    void    update();
    void    reset();
    void    moveTo(long absolute);
    void    setMaxSpeed(float speed);
    void    setAcceleration(float acceleration);

private:
    bool          _initialized;
    bool          _resetting;
    AccelStepper  _stepper;
    MFButton      _button;
    long          _targetPos;
    void          handlerOnRelease(byte eventId, uint8_t pin, String name);
};
#endif 