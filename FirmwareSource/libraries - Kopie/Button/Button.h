#ifndef Button_h
#define Button_h

//#include <math.h>

// #include <wiring.h>

/**
 * A debounced button class.
 *
 * The debouncing is done via tracking the time of last
 * button toggle and not by delaying. This button class
 * uses the ATMEGA's internal pull-up resistors.
 *
 * The circuit: push button attached to pin_X from ground
 * http://www.arduino.cc/en/Tutorial/Debounce
 *
 * Optional components to hardware debounce a button:
 * http://www.ganssle.com/debouncing-pt2.htm
 *
 *     (internal 20k)       10k
 * pin:<---/\/\--------*----/\/\----|
 *                     |            |
 *              0.1uf ===            / switch
 *                     |            /
 * gnd:<---------------*------------|
 */
class Button {
	uint8_t buttonPin;
	boolean lastState;
	boolean buttonState;
	long lastToggleTime;	// used to debounce the button

	/**
	 * Debounce time in milliseconds, default 10
	 */
	int debounce;

public:
	/**
	 * Initializes the class.
	 *
	 * pin		Number of the pin there the button is attached.
	 *
	 * debounceMillis
	 * 			The button debounce time in milliseconds.
	 */
	void initialize(uint8_t pin, int debounceMillis = 10);

	/**
	 * Updates the state of the rotary knob.
	 * This method should be placed in the main loop of the program or
	 * might be invoked from an interrupt.
	 */
	void update(void);

	/**
	 * Has the button stated changed from isUp to isDown at the last update.
	 * This is to be used like an OnKeyDown.
	 */
	inline boolean isPressed(void) {
		return ((!buttonState) && lastState);
	}

	/**
	 * Has the button stated changed from isDown to isUp at the last update.
	 * This is to be used like an OnKeyUp.
	 */
	inline boolean isReleased(void) {
		return (buttonState && (!lastState));
	}

	/**
	 * Is the button down (pushed).
	 */
	inline boolean isDown(void) {
		return (!buttonState);
	}

	/**
	 * Is the button up.
	 */
	inline boolean isUp(void) {
		return (buttonState);
	}
};

#endif
