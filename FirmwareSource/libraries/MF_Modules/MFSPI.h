// MFSPI.h
//
/// \mainpage MF SPI module for MobiFlight Framework
/// \version 1.0 Initial release
/// \author  J�rgen Baginski (github: JueBag)
// Copyright (C) 2021-2022 J�rgen Baginski

#ifndef MFSPI_h
#define MFSPI_h

#include <stdlib.h>
#include <Servo.h>

#if ARDUINO >= 100
#include <Arduino.h>
#else
#include <WProgram.h>
#endif

/////////////////////////////////////////////////////////////////////
/// \class MFSPI MFSPI.h <MFSPI.h>
class MFSPI
{
public:
		MFSPI();		
    MFSPI(uint8_t pin, bool enable = true);		
		void    attach(uint8_t pin = 1, bool enable = true);
    void    detach();
		void		setExternalRange(int min, int max);
		void    setInternalRange(int min, int max);
    void    moveTo(byte address, int absolute);
    void    update();

private:
		uint8_t _pin;
		int			_mapRange[4];
    bool    _initialized;
    Servo   _spi;
    long    _targetPos;
    long    _currentPos;
    unsigned long _lastUpdate;
    int     speed;
};
#endif 
