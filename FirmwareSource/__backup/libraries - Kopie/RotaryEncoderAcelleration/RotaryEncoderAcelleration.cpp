  /**
    * Includes Core Arduino functionality 
    **/
  #if ARDUINO < 100
  #include <WProgram.h>
  #else
  #include <Arduino.h>
  #endif
#include "RotaryEncoderAcelleration.h"

void RotaryEncoderAcelleration::initialize(uint8_t pinNumberA, uint8_t pinNumberB) {
	pinA.initialize(pinNumberA, 5);
	pinB.initialize(pinNumberB, 5);
	tps.initialize();

	position = 0;
	minValue = 0;
	maxValue = 1000;
}

void RotaryEncoderAcelleration::update() {
	pinA.update(); // toggle
	pinB.update(); // direction

	if (isTicked()) {
		tps.update(true);
		int speed = constrain(tps.getIntTPS_unsafe(), MIN_TPS, MAX_TPS) - MIN_TPS;
		long delta = max(1, (maxValue - minValue) / TICKS_AT_MAX_SPEED_FOR_FULL_SPAN);

		// Linear acceleration (very sensitive - not comfortable)
		// long step = 1 + delta * speed / (MAX_TPS - MIN_TPS);

		// Exponential acceleration - square (OK for [maxValue - minValue] = up to 5000)
		// long step = 1 + delta * speed * speed / ((MAX_TPS - MIN_TPS) * (MAX_TPS - MIN_TPS));

		// Exponential acceleration - cubic (most comfortable)
		long step = 1 + delta * speed * speed * speed /
				((MAX_TPS - MIN_TPS) * (MAX_TPS - MIN_TPS) * (MAX_TPS - MIN_TPS));

		position = constrain(position + (isIncrementing() ? step : -step),
				minValue, maxValue);
	} else {
		tps.update(false);
	}
}
