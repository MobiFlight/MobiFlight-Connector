  /**
    * Includes Core Arduino functionality 
    **/
  #if ARDUINO < 100
  #include <WProgram.h>
  #else
  #include <Arduino.h>
  #endif
#include <Button.h>

void Button::initialize(uint8_t pin, int debounceMillis) {
	buttonPin = pin;
	debounce = debounceMillis;
	pinMode(pin, INPUT);
	digitalWrite(pin, HIGH);
	lastToggleTime = millis();
	lastState = buttonState = digitalRead(buttonPin);
}

void Button::update() {
	boolean curReading = digitalRead(buttonPin);
	long now = millis();
	lastState = buttonState;
	if (curReading != buttonState) {
		if (now - lastToggleTime >= debounce) {
			// Button state has not changed for #debounce# milliseconds. Consider it is stable.
			buttonState = curReading;
		}
		lastToggleTime = now;
	} else if (now - lastToggleTime >= debounce) {
		// Forward the last toggle time a bit
		lastToggleTime = now - debounce - 1;
	}
}
