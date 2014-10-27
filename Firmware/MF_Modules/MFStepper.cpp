// MFStepper.cpp
//
// Copyright (C) 2013-2014

#include "MFStepper.h"

void MFStepper::moveTo(long absolute)
{		
    if (_targetPos != absolute)
    {
			_targetPos = absolute;
			_stepper.moveTo(absolute);
    }
}

MFStepper::MFStepper(uint8_t pin1, uint8_t pin2, uint8_t pin3, uint8_t pin4) : _stepper(AccelStepper::FULL4WIRE, pin4, pin2, pin1, pin3)
{		
		_targetPos = 0;
}

void MFStepper::update()
{
	_stepper.run();
}