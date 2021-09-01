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
  clearLastState();
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

  digitalWrite(_clockPin, HIGH); // preset clock to retrieve first bit
  digitalWrite(_latchPin, HIGH); // disable input latching and enable shifting

  // Multiple chained modules are handled individually, one at a time.
  // As shiftIn() keeps getting called it will pull in the data from each
  // chained module.
  for (int i = 0; i < _moduleCount; i++)
  {
    uint8_t currentState;

    currentState = shiftIn(_dataPin, _clockPin, MSBFIRST); // capture input values

    if (currentState != _lastState[i])
    {
      detectChanges(_lastState[i], currentState, i);
      _lastState[i] = currentState;
    }
  }
  digitalWrite(_latchPin, LOW); // disable shifting and enable input latching

  _last = now;
}

// Detects changes between the current state and the previously saved state
// of the shifter inputs.
void MFInputShifter::detectChanges(uint8_t lastState, uint8_t currentState, uint8_t module)
{
  for (int i = 0; i < 8; i++)
  {
    // If last and current input state for the bit are different
    // then the input changed and the handler for the bit needs to fire
    if ((lastState & 1) ^ (currentState & 1))
    {
      // When triggering the pin is the actual pin on the chip plus the module it's on.
      // This means pin 7 on chip 2 is really pin 15 as far as MobiFlight is concerned.
      trigger(i + (module * 8), currentState & 1);
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
  clearLastState();
  _last = 0;
}

void MFInputShifter::clearLastState()
{
  for (int i = 0; i < MAX_CHAINED_INPUT_SHIFTERS; i++)
  {
    _lastState[i] = 0;
  }
}