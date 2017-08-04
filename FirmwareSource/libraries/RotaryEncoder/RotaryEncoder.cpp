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
   0,  1, -1,  0};

// Configuration for different types of rotary encoders.
// For more information, refer to http://svglobe.com/arduino/in_encoder.html
//
// The detents in the encoder type settings below are indexed 
// by value, not by the order in the quadrature cycle. For example, a rotary 
// encoder with detents at 00 and 11 (positions 0 and 2 in the 
// quadrature) but are indexed based on their decimal values (0 and 3).
// This allows simple lookup of detent positions by value.
const encoderType encoderTypes[]{

	// 1 detents per cycle:  00 ,  10 , [11],  01 
	{ { false, false, false, true }, 2 },

	// 1 detents per cycle: [00],  10 ,  11 ,  01 
	{ { true, false, false, false }, 2 },

	// 2 detents per cycle: [00],  10 , [11],  01 
	{ { true, false, false, true }, 1 },

	// 2 detents per cycle:  00 , [10],  11,  [01]
	{ { false, true, true, false }, 1 },

	// 4 detents per cycle: [00], [10], [11], [01]
	{ { true, true, true, true }, 0 },
};

// ----- Initialization and Default Values -----

void RotaryEncoder::initialize(int pin1, int pin2, int encoder_type) {
  
  // Remember Hardware Setup
  _pin1.initialize(pin1, 5);
  _pin2.initialize(pin2, 5);

  // start with position 0;
  _oldState = 0;
  _position = 0;
  _positionExt = 0;
  
  _minValue = 0;
  _maxValue = 1000;

  _encoderType = encoderTypes[encoder_type];
  
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
  _position = ((newPosition >> _encoderType.resolutionShift) | (_position & 0x03));
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
		
		if (_encoderType.detents[thisState]) {
			tps.update(true);

			_positionExt = _position >> _encoderType.resolutionShift;
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