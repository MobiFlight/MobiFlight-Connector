// MFOutput.cpp
//
// Copyright (C) 2013-2014

#include "MFOutput.h"

MFOutput::MFOutput(uint8_t pin)
{   
  _pin  = pin;
  _value = false;
  pinMode(_pin, OUTPUT);    // set pin to input
  set(_value);
}

void MFOutput::set(uint8_t value)
{
  _value = value;
  analogWrite(_pin, _value);
}

void MFOutput::powerSavingMode(bool state) 
{
  if (state) set(0);
  else set(_value);
}