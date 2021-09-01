// MFSegments.cpp
//
// Copyright (C) 2021

#include "MFInputShifter.h"

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
  _latchPin = latchPin;
  _clockPin = clockPin;
  _dataPin = dataPin;
  _moduleCount = moduleCount;

  pinMode(_latchPin, OUTPUT);
  pinMode(_clockPin, OUTPUT);
  pinMode(_dataPin, INPUT);
  _initialized = true;
}

// Reads the values from the attached modules, compares them to the previously
// read values, and calls the registered event handler for any inputs that
// changed from the previously read state.
void MFInputShifter::update()
{
  // Don't take any changes within the last 10 milliseconds to cover basic switch debouncing.
  // This technically could miss two different switches changing within 10 milliseconds
  // but that seems extremely unlikely to happen.
  uint32_t now = millis();
  if (now - _last <= 10)
    return;

  digitalWrite(_clockPin, HIGH); // Preset clock to retrieve first bit
  digitalWrite(_latchPin, HIGH); // Disable input latching and enable shifting

  // Multiple chained modules are handled one at a time. As shiftIn() keeps getting
  // called it will pull in the data from each chained module.
  for (int i = 0; i < _moduleCount; i++)
  {
    uint8_t currentState;

    currentState = shiftIn(_dataPin, _clockPin, MSBFIRST);

    // If an input changed on the current module from the last time it was read
    // then hand it off to figure out which bits specifically changed.
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
// of a byte's worth of input.
void MFInputShifter::detectChanges(uint8_t lastState, uint8_t currentState, uint8_t module)
{
  for (int i = 0; i < sizeof(uint8_t); i++)
  {
    // If last and current input state for the bit are different
    // then the input changed and the handler for the bit needs to fire
    if ((lastState & 1) ^ (currentState & 1))
    {
      // When triggering event the pin is the actual pin on the chip offset by 8 bits for each
      // module beyond the first that it's on. The state of the trigger is the bit currently
      // in position 0 of currentState.
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

// Attaches a new event handler for the specified event.
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

// Clears the internal state of the shifter, including all received bits
// and the timestamp for the last time the data was read.
void MFInputShifter::clear()
{
  _last = 0;
  clearLastState();
}

// Sets the last recorded state of every bit on every shifter to 0.
void MFInputShifter::clearLastState()
{
  for (int i = 0; i < MAX_CHAINED_INPUT_SHIFTERS; i++)
  {
    _lastState[i] = 0;
  }
}