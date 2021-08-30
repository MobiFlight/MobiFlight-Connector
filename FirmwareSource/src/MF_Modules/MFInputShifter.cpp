// MFSegments.cpp
//
// Copyright (C) 2021

#include "MFInputShifter.h"

MFInputShifter::MFInputShifter()
{
  _initialized = false;
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

void MFInputShifter::detach()
{
  if (!_initialized)
    return;
  _initialized = false;
}

void MFInputShifter::test()
{
}