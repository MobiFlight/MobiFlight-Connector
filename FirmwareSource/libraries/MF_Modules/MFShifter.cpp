// MFSegments.cpp
//
// Copyright (C) 2021

#include "MFShifter.h"

MFShifter::MFShifter()
{
  _initialized = false;
}

void MFShifter::setPin(uint8_t pin, int value)
{
  if (!_initialized) return;

  if (value > 0) {
    bitSet(_output, pin);  
  } else {
    bitClear(_output, pin);
  }  
  updateShiftRegister();
}

void MFShifter::setPins(char* pins, int value)
{
  if (!_initialized) return;

  char* pinTokens = strtok(pins, "|");
  while (pinTokens != 0) {
    int registerNum = atoi(pinTokens);

    if (value > 0) {
      bitSet(_output, registerNum);  
    } else {
      bitClear(_output, registerNum);
    }    
    pinTokens = strtok(0, "|");
  }
  updateShiftRegister();
}

void MFShifter::attach(uint8_t latchPin, uint8_t clockPin, uint8_t dataPin, uint8_t moduleCount)
{
  _initialized = true;
  _latchPin = latchPin;
  _clockPin = clockPin;
  _dataPin = dataPin;
  _moduleCount = moduleCount;

  pinMode(_latchPin, OUTPUT);
  pinMode(_clockPin, OUTPUT);  
  pinMode(_dataPin, OUTPUT);

  clear();
}

void MFShifter::detach()
{
  if (!_initialized) return;
  _initialized = false;
}

void MFShifter::clear() 
{
  // Set everything to 0
  _output = 0;
  updateShiftRegister();  
}

void MFShifter::test() 
{
  _output = 0;
  updateShiftRegister();

  for (unsigned long i = 0; i < _moduleCount * 8; i++)
  {   
    bitSet(_output, i);
    updateShiftRegister();
    delay(50);
  }
  
  for (unsigned long i = 0; i < _moduleCount * 8; i++)
  {
    bitClear(_output, i);
    updateShiftRegister();
    delay(50);
  }
}

void MFShifter::updateShiftRegister()
{
   digitalWrite(_latchPin, LOW);
   for (int i = _moduleCount; i >=0 ; i--) {
      shiftOut(_dataPin, _clockPin, MSBFIRST, (_output >> i*8)); //LSBFIRST, MSBFIRST,
   }    
   digitalWrite(_latchPin, HIGH);
}
