// MFEncoder.h
//
/// \mainpage MF Button module for MobiFlight Framework
/// \par Revision History
/// \version 1.0 Initial release
/// \author  Sebastian Moebius (mobiflight@moebiuz.de) DO NOT CONTACT THE AUTHOR DIRECTLY: USE THE LISTS
// Copyright (C) 2013-2014 Sebastian Moebius

#ifndef MFEncoder_h
#define MFEncoder_h

#include <stdlib.h>
#if ARDUINO >= 100
#include <Arduino.h>
#else
#include <WProgram.h>
#include <wiring.h>
#endif

///#include "../Button/Button.h"
///#include "../TicksPerSecond/TicksPerSecond.h"
///#include "../RotaryEncoderAcelleration/RotaryEncoderAcelleration.h"
#include <RotaryEncoder.h>

extern "C"
{
  typedef void (*encoderEvent) (uint8_t, uint8_t, const char *);
};

// this prevents the internal position overflow.
// no need to change this
#define MF_ENC_MAX 8000 

// this defines the delta value limit for triggering onFast
// this should work well for all encoder types
#define MF_ENC_FAST_LIMIT 40

enum
{
  encLeft,
  encLeftFast,
  encRight,
  encRightFast
};

/////////////////////////////////////////////////////////////////////
/// \class MFEncoder MFEncoder.h <MFEncoder.h>
class MFEncoder
{
public:
    MFEncoder();
	  void attach(uint8_t pin1, uint8_t pin2, uint8_t encoderType, const char * name = "Encoder");
    void update();
    void attachHandler(uint8_t eventId, encoderEvent newHandler);
    
private:
    uint8_t                   _pin1;              
    uint8_t                   _pin2;
    bool                      _initialized;
    RotaryEncoder             _encoder;
    const char *              _name;
    long                      _pos;
    encoderEvent              _handlerList[4];
    uint8_t                   _encoderType;
    uint16_t                  _fastLimit = MF_ENC_FAST_LIMIT;
};
#endif 