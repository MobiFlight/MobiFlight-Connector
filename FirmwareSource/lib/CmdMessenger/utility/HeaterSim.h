/*
  HeaterSim.cpp - Simple heater simulation library
  Copyright (c) 2014 Thijs Elenbaas.  All right reserved.

  This library is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public
  License as published by the Free Software Foundation; either
  version 2.1 of the License, or (at your option) any later version.

  This library is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
  Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General Public
  License along with this library; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

#ifndef HEATERSIM_h
#define HEATERSIM_h


#if ARDUINO >= 100
#include <Arduino.h> 
#else
#include <WProgram.h> 
#endif
#include <inttypes.h>


class HeaterSim 
{	 
  private:
	float _boilerOn;
	float _boilerTemp;
	float _delayedBoilerTemp;
	float _ambientTemp;
	float _deltaTime;
	unsigned long _lastTime;
	unsigned long _deltams;
	float _halfTimeCooling;
	float _responseTime;
	float _heaterSpeed;
	void CalcTemperature();
	void CalcDelayedTemperature();
	
  public:
	HeaterSim(float ambientTemp = 20);
	void SetAmbientTemp(float ambientTemp);
	void SetBoilerTemp(float boilerTemp);
	float GetTemp();	
	void SetHeaterState(bool boilerOn);	
};

extern HeaterSim Boiler;

#endif

