// MFSegments.h
//
/// \mainpage MF MFAnalog module for MobiFlight Framework
/// \par Revision History
/// \version 1.0 Initial release
/// \author  Manfred Berry (manfred@nystedberry.info) DO NOT CONTACT THE AUTHOR DIRECTLY: USE THE LISTS
// Copyright (C) 2021 Manfred Berry

#ifndef MFAnalog_h
#define MFAnalog_h

#if ARDUINO >= 100
#include <Arduino.h>
#else
#include <WProgram.h>
#endif

extern "C"
{
  // callback functions
  typedef void (*analogEvent) (int, uint8_t, const char *);
};


/////////////////////////////////////////////////////////////////////
/// \class MFAnalog MFAnalog.h <MFAnalog.h>
class MFAnalog
{
public:
    MFAnalog(uint8_t pin = 1, analogEvent callback = NULL, const char * name = "Analog Input", uint8_t sensitivity = 2);
    void update();    
    const char *  _name;
    uint8_t       _pin;
    
private:
    int          _lastValue;
    uint32_t      _last;
    analogEvent   _handler; 
    uint8_t       _sensitivity;
};
#endif 