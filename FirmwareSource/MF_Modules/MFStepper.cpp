// MFStepper.cpp
//
// Copyright (C) 2013-2014

#include "MFStepper.h"
#include "MFButton.h"

uint8_t MFStepper_stepperCount = 0;
MFStepper* MFStepper_steppers[5];

void addStepper(MFStepper *stepper) {
  MFStepper_steppers[MFStepper_stepperCount] = stepper;
  MFStepper_stepperCount ++;
}

void HandlerOnRelease(byte eventId, uint8_t pin, String name) {
    //if (_resetting) {
    //  _resetting = false;
    //  _stepper.setCurrentPosition(0);
    //}
}

MFStepper::MFStepper(uint8_t pin1, uint8_t pin2, uint8_t pin3, uint8_t pin4, uint8_t btnPin5) : _stepper(AccelStepper::FULL4WIRE, pin4, pin2, pin1, pin3), _button(btnPin5, "0")
{
    addStepper(this);
    _button.attachHandler(btnOnPress, HandlerOnRelease);
}

void MFStepper::moveTo(long absolute)
{
    if (_targetPos != absolute)
    {
      _targetPos = absolute;
      _stepper.moveTo(absolute);
    }
}

void MFStepper::update()
{
    _stepper.run();
}

void MFStepper::reset()
{
    if (_resetting) return;
    _resetting = true;
    _stepper.moveTo(-10000);
}

void MFStepper::setMaxSpeed(float speed) {
    _stepper.setMaxSpeed(speed);
}

void MFStepper::setAcceleration(float acceleration){
    _stepper.setAcceleration(acceleration);
}