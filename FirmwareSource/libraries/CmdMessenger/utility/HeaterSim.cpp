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

/******************************************************************************
 * Includes
 ******************************************************************************/
#include  "HeaterSim.h"
#include <math.h> 

/******************************************************************************
 * Definitions
 ******************************************************************************/

#define _HEATERSIM_VERSION 0.1.0 // software version of this library
#define LN2 0.693147f
  
 /******************************************************************************
 * Constructors
 ******************************************************************************/

HeaterSim::HeaterSim(float ambientTemp) 
{
  _ambientTemp       = ambientTemp;
  _boilerTemp        = _ambientTemp; 
  _delayedBoilerTemp = _ambientTemp;
  _halfTimeCooling   = 30.0f; // time in sec needed to cool to half of temp difference with surroundings
  _heaterSpeed       = 2.0f; // time in sec to heat the boiler by 1 degree
  _responseTime      = 5.0f; // Delay in response of temperature detector

  _lastTime          = millis(); 
}

void HeaterSim::SetAmbientTemp(float ambientTemp) {
  CalcTemperature();
  _ambientTemp         = ambientTemp;
}
 
void HeaterSim::SetBoilerTemp(float boilerTemp) {
	_boilerTemp        = boilerTemp;
	_delayedBoilerTemp = boilerTemp;
}

void HeaterSim::SetHeaterState(bool boilerOn) {
	CalcTemperature();
	_boilerOn = boilerOn;
}

float HeaterSim::GetTemp() {
	CalcTemperature();
	return _delayedBoilerTemp;
}

void HeaterSim::CalcTemperature()
{
	float rate                  = LN2/_halfTimeCooling;	 
	unsigned long currentTime   = millis();
	if (currentTime >= _lastTime) _deltams = max(currentTime - _lastTime,1);
	_deltaTime                  = _deltams/ 1000.0f;
	float deltaTemp             = _boilerTemp - _ambientTemp;
	float deltaTempAfterCooling = deltaTemp*exp(-rate*_deltaTime);
	float inflowTemp            = _deltaTime * (_boilerOn?_heaterSpeed:0.0f);	 
	float currentTemp           = _ambientTemp + deltaTempAfterCooling + inflowTemp;
	currentTemp                 = min(max(currentTemp,0.0f),100.0f);

	_lastTime                   = currentTime;
	_boilerTemp                 = currentTemp;
	CalcDelayedTemperature();	  
}

void HeaterSim::CalcDelayedTemperature()
{
	float fract = min(max(_deltaTime/_responseTime,0.0f),1.0f);
	_delayedBoilerTemp = fract* _boilerTemp + (1.0f - fract)* _delayedBoilerTemp;
}