/**********************************************************************************************
 *
 * This Code is licensed under a Creative Commons Attribution-ShareAlike 3.0 License.
 **********************************************************************************************/

#include <DoEvery.h>

DoEvery::DoEvery(long _period) {
	period=_period;
	lastTime=0;
}

void DoEvery::reset() {
	lastTime=millis();
}

bool DoEvery::check() {
	if (millis()-lastTime > period) {
		lastTime+=period;
		return true;
	} else {
		return false;
	}
}

bool DoEvery::before(double threshTime) {
	if (threshTime>=period) return true;
	if (millis()-lastTime < threshTime) {
		return true;
	} else {
		return false;
	}
}
