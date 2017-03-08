// MFEncoder.cpp
//
// Copyright (C) 2013-2014

#include "MFEncoder.h"

MFEncoder::MFEncoder() : _encoder() {
  _initialized = false;
}

void MFEncoder::attach(uint8_t pin1, uint8_t pin2, String name)
{
  _pos   = 0;  
  _name  = name;
  _pin1  = pin1;
  _pin2  = pin2;
  
  _encoder.initialize(pin1,pin2);
  _encoder.setMinMax(MF_ENC_MIN,MF_ENC_MAX);
  _encoder.setPosition(_pos);
  
  _initialized = true;
}

void MFEncoder::update()
{
  if (!_initialized) return;
  
  //_encoder.update();
  _encoder.tick();
  long pos = _encoder.getPosition();
  
  if (pos == _pos) {
    // nothing happened 
    return;
  }
  
  long delta = pos - _pos;
  long dir = 1;
  if (delta<0) dir = -1;  
  
  long absDelta = abs(delta);
  if (absDelta < 2) {
    // slow turn detected
    if (dir==1 && _handlerList[encLeft]!= NULL) {
        (*_handlerList[encLeft])(encLeft, _pin1, _name);
    } else if(_handlerList[encRight]!= NULL) {
        (*_handlerList[encRight])(encRight, _pin2, _name);
    }
  } else {
    // fast turn detected
    if (dir==1 && _handlerList[encLeftFast]!= NULL) {
        (*_handlerList[encLeftFast])(encLeftFast,  _pin1, _name);
    } else if(_handlerList[encRightFast]!= NULL) {
        (*_handlerList[encRightFast])(encRightFast, _pin2, _name);
    }
  }
  
  // clamp values
  if ( (dir > 0 && (pos + delta*2) > MF_ENC_MAX) || (dir < 0 && (pos - delta*2) < MF_ENC_MIN)) 
  { 
    _encoder.setPosition(0); 
    pos = 0; 
  }
  _pos = pos;
}

void MFEncoder::attachHandler(byte eventId, encoderEvent newHandler)
{
  _handlerList[eventId] = newHandler;
}