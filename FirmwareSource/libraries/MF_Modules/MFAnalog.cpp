// MFSegments.cpp
//
// Copyright (C) 2021

#include "MFAnalog.h"

MFAnalog::MFAnalog(uint8_t pin, analogEvent callback, const char * name, uint8_t sensitivity)
{   
  _sensitivity = sensitivity;  
  _pin  = pin;
  _name = name;
  _lastValue = 0;
  _last = millis();
  _handler = callback; 
  pinMode(_pin, INPUT_PULLUP);     // set pin to input. Could use OUTPUT for analog, but shows the intention :-)
  analogRead(_pin); // turn on pullup resistors
}

void MFAnalog::update()
{    
    uint32_t now = millis();
    if (now-_last <= 50) return; // Analog is too spammy on the protocol to MF otherwise.

    int newValue = (int) analogRead(_pin);
    _last = now;
    if (abs(newValue - _lastValue) >= _sensitivity) {
      _lastValue = newValue;
       if (_handler!= NULL) {
        (*_handler)(_lastValue, _pin, _name);
      }      
    }
}