// MFStepper.cpp
//
// Copyright (C) 2013-2014

#include "MFStepper.h"
#include "MFButton.h"
/*
uint8_t MFStepper_stepperCount = 0;
MFStepper* MFStepper_steppers[10];

void addStepper(MFStepper *stepper) {
  MFStepper_steppers[MFStepper_stepperCount] = stepper;
  MFStepper_stepperCount ++;
}

void HandlerOnRelease(byte eventId, uint8_t pin, String name) {
  if ( eventId != btnOnPress ) return;
  for (int i=0; i < MFStepper_stepperCount; i++) {
      if (MFStepper_steppers[i]->getButton()->_pin == pin) {
        MFStepper_steppers[i]->setZeroInReset();
        break;
      }
  }
    //if (_resetting) {
    //  _resetting = false;
    //  _stepper.setCurrentPosition(0);
    //}
}
*/

MFStepper::MFStepper(uint8_t pin1, uint8_t pin2, uint8_t pin3, uint8_t pin4 /*, uint8_t btnPin5*/) : _stepper(AccelStepper::FULL4WIRE, pin4, pin2, pin1, pin3) /*, _button(btnPin5, "0") */
{
    _resetting = false;
    /* addStepper(this);
    _button.attachHandler(btnOnPress, HandlerOnRelease);
    _button.attachHandler(btnOnRelease, HandlerOnRelease);
    */
}

void MFStepper::moveTo(long absolute)
{
    _resetting = false;
    if (_targetPos != absolute)
    {
      _targetPos = absolute;
      _stepper.moveTo(absolute);
    }
}

void MFStepper::setZero()
{
    _stepper.setCurrentPosition(0);
}

void MFStepper::setZeroInReset()
{
    if (_resetting) {
      _stepper.setCurrentPosition(0);
      _resetting = false;
    }
}

void MFStepper::update()
{
    _stepper.run();
    /* _button.update(); */
}

void MFStepper::reset()
{
    // if we are already resetting ignore next reset command
    if (_resetting) return;
    
    // flag that we are resetting
    _resetting = true;
    
    // tell stepper to move counter clockwise for a long while
    _stepper.moveTo(-100000);
}

void MFStepper::setMaxSpeed(float speed) {
    _stepper.setMaxSpeed(speed);
}

void MFStepper::setAcceleration(float acceleration){
    _stepper.setAcceleration(acceleration);
}
/*
MFButton * MFStepper::getButton() {
    return &_button;
}
*/