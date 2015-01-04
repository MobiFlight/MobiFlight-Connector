// MFServo.cpp
//
// Copyright (C) 2013-2014

#include "MFServo.h"

void MFServo::moveTo(int absolute)
{
		int newValue = map(absolute, _mapRange[0], _mapRange[1], _mapRange[2], _mapRange[3]);
    if (_targetPos != newValue)
    {
			_targetPos = newValue;
			if (!_initialized) {
			  _servo.attach(_pin);
				_initialized = true;
			}
			_servo.write(newValue);
      //delay(5);
    }
}

void MFServo::detach() { _servo.detach(); _initialized = false; }

void MFServo::attach(uint8_t pin, bool enable)
{
	_initialized = false;
	_targetPos = 0;
	setExternalRange(0,180);
	setInternalRange(0,180);
	_pin = pin;		
}

MFServo::MFServo() : _servo() {}

MFServo::MFServo(uint8_t pin, bool enable) : _servo()
{				
	attach(pin, enable);
}

void MFServo::setExternalRange(int min, int max)
{
	_mapRange[0] = min;
	_mapRange[1] = max;
}

void MFServo::setInternalRange(int min, int max)
{
	_mapRange[2] = min;
	_mapRange[3] = max;
}
