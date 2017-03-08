// -----
// RotaryEncoder.cpp - Library for using rotary encoders.
// This class is implemented for use with the Arduino environment.
// Copyright (c) by Matthias Hertel, http://www.mathertel.de
// This work is licensed under a BSD style license. See http://www.mathertel.de/License.aspx
// More information on: http://www.mathertel.de/Arduino
// -----
// 18.01.2014 created by Matthias Hertel
// -----

#include "Arduino.h"
#include "RotaryEncoder.h"
#include <TicksPerSecond.h>


// The array holds the values –1 for the entries where a position was decremented,
// a 1 for the entries where the position was incremented
// and 0 in all the other (no change or not valid) cases.

const int8_t KNOBDIR[] = {
  0, -1,  1,  0,
  1,  0,  0, -1,
  -1,  0,  0,  1,
0,  1, -1,  0  };

// ----- Initialization and Default Values -----

void RotaryEncoder::initialize(int pin1, int pin2) {
  
  // Remember Hardware Setup
  _pin1.initialize(pin1, 5);
  _pin2.initialize(pin2, 5);
  /*
  // Setup the input pins
  pinMode(pin1, INPUT);
  digitalWrite(pin1, HIGH);   // turn on pullup resistor

  pinMode(pin2, INPUT);
  digitalWrite(pin2, HIGH);   // turn on pullup resistor
*/
  // when not started in motion, the current state of the encoder should be 3
  _oldState = 3;

  // start with position 0;
  _position = 0;
  _positionExt = 0;
  
  _minValue = 0;
  _maxValue = 1000;
  
  tps.initialize();
} // RotaryEncoder()


void RotaryEncoder::setMinMax(int min, int max) {
	_minValue = min;
	_maxValue = max;
}


int  RotaryEncoder::getPosition() {
  return _positionExt;
}

void RotaryEncoder::setPosition(int newPosition) {
  // only adjust the external part of the position.
  
  _position = ((newPosition<<2) | (_position & 0x03));
  _positionExt = newPosition;

}

void RotaryEncoder::tick(void)
{
	_pin1.update(); // toggle
	_pin2.update(); // direction
  int sig1 = _pin1.isDown();
  int sig2 = _pin2.isDown();
  int8_t thisState = sig1 | (sig2 << 1);
  tps.update(false);
  
  if (_oldState != thisState) {
	int _speed = calcSpeed();
	_position += KNOBDIR[thisState | (_oldState<<2)] * _speed;
	
    if (thisState == LATCHSTATE) {
	  tps.update(true);
	  
      _positionExt = (_position >> 2);
	}
    
    _oldState = thisState;
  }// if
} // tick()

long RotaryEncoder::calcSpeed(void)
{
	int speed = constrain(tps.getIntTPS_unsafe(), MIN_TPS, MAX_TPS) - MIN_TPS;
	long delta = max(1, (_maxValue - _minValue) / TICKS_AT_MAX_SPEED_FOR_FULL_SPAN);

	// Exponential acceleration - cubic (most comfortable)
	long step = 1 + delta * speed * speed * speed /
			((MAX_TPS - MIN_TPS) * (MAX_TPS - MIN_TPS) * (MAX_TPS - MIN_TPS));

	return step;
}

// End