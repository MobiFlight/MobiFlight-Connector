// MFOutput.cpp
//
// Copyright (C) 2013-2014

#include "MFOutput.h"

MFOutput::MFOutput(uint8_t pin)
{   
  _pin  = pin;
  _state = false;
  pinMode(_pin, OUTPUT);    // set pin to input
  set(_state);
}

void MFOutput::set(bool state)
{
  _state = state;
  digitalWrite(_pin, (_state) ? HIGH : LOW);
}

void MFOutput::powerSavingMode(bool state) 
{
  if (state) digitalWrite(_pin, LOW);
  else set(_state);
}