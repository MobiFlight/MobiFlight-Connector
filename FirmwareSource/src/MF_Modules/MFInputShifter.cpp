// MFSegments.cpp
//
// Copyright (C) 2021

#include "MFInputShifter.h"

void printBinary(unsigned long val)
{
  for (int i = 0; i < 8; i++)
  {
    bool b = val & 0x80;
    Serial.print(b);
    val = val << 1;
  }
}

MFInputShifter::MFInputShifter(const char *name)
{
  _initialized = false;
  _lastState = 0;
  _last = millis();
  _name = name;
}

// Registers a new input shifter and configures the clock, data and latch pins as well
// as the number of modules to read from.
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

// Reads the values from the attached modules, compares them to the previously
// read values, and calls the registered event handler for any inputs that
// changed from the previously read state.
void MFInputShifter::update()
{
  // Don't take any chances within the last 10 milliseconds to cover basic switch debouncing
  uint32_t now = millis();
  if (now - _last <= 10)
    return;

  uint8_t currentState;

  digitalWrite(_clockPin, HIGH);                         // preset clock to retrieve first bit
  digitalWrite(_latchPin, HIGH);                         // disable input latching and enable shifting
  currentState = shiftIn(_dataPin, _clockPin, MSBFIRST); // capture input values
  digitalWrite(_latchPin, LOW);                          // disable shifting and enable input latching

  if (currentState != _lastState)
  {
    detectChanges(_lastState, currentState);
    _lastState = currentState;
  }

  _last = now;
}

// Detects changes between the current state and the previously saved state
// of the shifter inputs.
void MFInputShifter::detectChanges(uint8_t lastState, uint8_t currentState)
{
  // TODO: Expand this so it handles the number of attached shifters
  for (int i = 0; i < 8; i++)
  {
    // If last and current input state for the bit are different
    // then the input changed and the handler for the bit needs to fire
    if ((lastState & 1) ^ (currentState & 1))
    {
      trigger(i, currentState & 1);
    }

    lastState = lastState >> 1;
    currentState = currentState >> 1;
  }
}

// Triggers the event handler for the associated input shift register pin,
// if a handler is registered.
void MFInputShifter::trigger(uint8_t pin, bool state)
{
  if (state == LOW && _handlerList[shifterOnPress] != NULL)
  {
    (*_handlerList[shifterOnPress])(shifterOnPress, pin, _name);
  }
  else if (_handlerList[shifterOnRelease] != NULL)
  {
    (*_handlerList[shifterOnRelease])(shifterOnRelease, pin, _name);
  }
}

void MFInputShifter::attachHandler(byte eventId, inputShifterEvent newHandler)
{
  _handlerList[eventId] = newHandler;
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