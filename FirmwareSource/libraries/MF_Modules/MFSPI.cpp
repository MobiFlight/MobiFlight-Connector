// MFSPI.cpp
//
// Copyright (C) 2021-2022

#include "MFSPI.h"
#include "SPI.h"

void MFSPI::moveTo(byte address, int absolute)
{
	int newValue = map(absolute, _mapRange[0], _mapRange[1], _mapRange[2], _mapRange[3]);
    if (_targetPos != newValue)
    {
			_targetPos = newValue;
			if (!_initialized) {
			  _spi.attach(_pin);
				_initialized = true;
			}
			// take the SS pin low to select the chip:
			digitalWrite(_pin, LOW);
			//  send in the address and value via SPI:
			SPI.transfer(address);
			SPI.transfer(newValue);
			// take the SS pin high to de-select the chip:
			digitalWrite(_pin, HIGH);
    }
}

void MFSPI::update() {
	    if (_currentPos == _targetPos) { 
		// detach(); 
		return; 
	}
	
    //if ((millis()-_lastUpdate) < 5) return;
    
    if (_currentPos > _targetPos) _currentPos--;
    else _currentPos++;
    
    _lastUpdate = millis();
    _spi.write(_currentPos);
}

void MFSPI::detach() { 
  if (_initialized) {
    _spi.detach(); 
    _initialized = false; 
  }
}

void MFSPI::attach(uint8_t pin, bool enable)
{
	_initialized = false;
	_targetPos = 0;
	_currentPos = 0;
	setExternalRange(0,255);
	setInternalRange(0,255);
	_pin = pin;		
	_lastUpdate = millis();
}

MFSPI::MFSPI() : _spi() {}

MFSPI::MFSPI(uint8_t pin, bool enable) : _spi()
{				
	attach(pin, enable);
}

void MFSPI::setExternalRange(int min, int max)
{
	_mapRange[0] = min;
	_mapRange[1] = max;
}

void MFSPI::setInternalRange(int min, int max)
{
	_mapRange[2] = min;
	_mapRange[3] = max;
}
