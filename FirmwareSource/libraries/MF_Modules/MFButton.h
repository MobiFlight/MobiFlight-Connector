// MFButton.h
//
/// \mainpage MF Button module for MobiFlight Framework
/// \par Revision History
/// \version 1.0 Initial release
/// \author  Sebastian Moebius (mobiflight@moebiuz.de) DO NOT CONTACT THE AUTHOR DIRECTLY: USE THE LISTS
// Copyright (C) 2013-2014 Sebastian Moebius

#ifndef MFButton_h
#define MFButton_h

#include <stdlib.h>
#if ARDUINO >= 100
#include <Arduino.h>
#else
#include <WProgram.h>
#include <wiring.h>
#endif

extern "C"
{
  // callback functions always follow the signature: void cmd(void);
  typedef void (*buttonEvent) (byte, uint8_t, const char *);
};

enum
{
  btnOnPress,
  btnOnRelease,
};

/////////////////////////////////////////////////////////////////////
/// \class MFButton MFButton.h <MFButton.h>
class MFButton
{
public:
    MFButton(uint8_t pin = 1, const char * name = "Button");
    void update();
    void trigger();
    void attachHandler(byte eventId, buttonEvent newHandler);    
    const char *  _name;
    uint8_t       _pin;
    
private:
    bool          _state;
    long          _last;
    buttonEvent   _handlerList[2];    
};
#endif 