  /**
    * Includes Core Arduino functionality 
    **/
  #if ARDUINO < 100
  #include <WProgram.h>
  #else
  #include <Arduino.h>
  #endif
#include "TicksPerSecond.h"

void TicksPerSecond::initialize(int holdLastTimeoutMillis) {
	lastTime = millis();
	for (byte i = 0; i < TPS_TIMES_PER_PERIOD; i++) {
		counters[i] = 0;
		started[i] = lastTime;
	}
	curCounter = 0;
	deltaTime = holdLastTimeoutMillis / TPS_TIMES_PER_PERIOD;
}

void TicksPerSecond::update(boolean tick) {
	long now = millis();
	if (now - lastTime >= deltaTime) {
		counters[curCounter] = 0;
		lastTime = started[curCounter++] = now;
		if (curCounter >= TPS_TIMES_PER_PERIOD)
			curCounter = 0;
	}
	if (tick) {
		for (byte i = 0; i < TPS_TIMES_PER_PERIOD; i++) {
			counters[i]++;
		}
	}
	now -= started[curCounter];
	if (now <= 0)
		now = 1; // This is a division by zero protection.
	tps = (counters[curCounter] * 1000.0) / now;
}

void TicksPerSecond::smooth(int data, float *smoothedValue, int timesPerSecond) {
	float scale = getTPS() / timesPerSecond;
	*smoothedValue = (data + (*smoothedValue) * scale) / (scale + 1);
}
