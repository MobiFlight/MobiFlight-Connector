// MFSegments.cpp
//
// Copyright (C) 2021

#include "MFInputShifter.h"

MFInputShifter::MFInputShifter()
{
  _initialized = false;
  _lastState = 0;
  _last = 0;
}

void MFInputShifter::attach(uint8_t latchPin, uint8_t clockPin, uint8_t dataPin, uint8_t moduleCount)
{
  _initialized = true;
  _latchPin = latchPin;
  _clockPin = clockPin;
  _dataPin = dataPin;
  _moduleCount = moduleCount;

  pinMode(_latchPin, OUTPUT);
  pinMode(_clockPin, OUTPUT);
  pinMode(_dataPin, INPUT);
}

void MFInputShifter::update()
{
  uint32_t now = millis();
  if (now - _last <= 10)
    return;

  unsigned long currentState;
  unsigned long changedBits;

  digitalWrite(_clockPin, HIGH); // preset clock to retrieve first bit
  digitalWrite(_latchPin, HIGH); // disable input latching and enable shifting
  for (int i = 0; i < _moduleCount; i++)
  {
    currentState = shiftIn(_dataPin, _clockPin, MSBFIRST); // capture input values
  }
  digitalWrite(_latchPin, LOW); // disable shifting and enable input latching

  if (currentState != _lastState)
  {
    changedBits = currentState ^ _lastState; // Figure out which inputs changed
    Serial.print("Changed bits: 0b");
    Serial.println(changedBits, BIN);
    _lastState = currentState;
  }

  _last = now;
}

void MFInputShifter::detach()
{
  if (!_initialized)
    return;
  _initialized = false;
}

void MFInputShifter::clear()
{
  _lastState = 0;
  _last = 0;
}

void MFInputShifter::test()
{
}