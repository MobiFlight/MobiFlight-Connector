#ifndef RotaryEncoderAcelleration_h
#define RotaryEncoderAcelleration_h

#include "../TicksPerSecond/TicksPerSecond.h"
#include "../Button/Button.h"

/**
 * Minimum rotary encoder tick per second to start acceleration.
 * If the speed of ticking is below this value no acceleration
 * is considered, i.e. ticking is by 1.
 */
#define MIN_TPS 5

/**
 * Maximum rotary encoder tick per second when accelerating.
 * If the speed of ticking is above this value then acceleration
 * is considered at full speed.
 */
#define MAX_TPS 30

/**
 * The number of ticks that a rotary encoder should make at full speed
 * to go from minValue to maxValue. If rotary encoder has 20 ticks for
 * a 360 degrees rotation then 5 rotations at full speed will be needed
 * to go from minValue to maxValue.
 */
#define TICKS_AT_MAX_SPEED_FOR_FULL_SPAN 100

/**
 * Quadrature (Rotary encoder) sensitive to rotation speed.
 * When the quadrature is rotated a #position# variable is
 * incremented/decremented. If the quadrature is rotated fast
 * the increment/decrement step is increased.
 *
 * Can be used with or without interrupt!
 *
 * Uses:
 *   the timer0 via the millis() function.
 *   pinA, pinB can be connected to any digital pin.
 *
 *   http://hacks.ayars.org/2009/12/using-quadrature-encoder-rotary-switch.html
 */
class RotaryEncoderAcelleration {
private:
	long minValue;
	long maxValue;
	volatile long position;

public:
	Button pinA;
	Button pinB;
	TicksPerSecond tps;

	/**
	 * Initializes the class, sets ports (pinA and pinB) to output mode.
	 */
	void initialize(uint8_t pinNumberA, uint8_t pinNumberB);

	/**
	 * Updates the state of the rotary encoder.
	 * This method should be placed in the main loop of the program or
	 * might be invoked from an interrupt.
	 */
	void update();

	/**
	 * Has the rotary encoder been ticked at the last update
	 */
	inline boolean isTicked() {
		return pinA.isPressed();
	}

	/**
	 * Has the rotary encoder been rotated in incrementing direction at the last update.
	 * If the method returns TRUE the direction is incrementing.
	 */
	inline boolean isIncrementing() {
		return pinB.isUp();
	}

	/**
	 * Gets the position of the encoder. If the update method is called from an
	 * interrupt use the safe method getPosition() instead.
	 */
	inline long getPosition_unsafe() {
		return position;
	}

	/**
	 * Sets the position of the encoder. If the update method is called from an
	 * interrupt use the safe method getPosition() instead.
	 */
	inline void setPosition_unsafe(long newPosition) {
		position = constrain(newPosition, minValue, maxValue);
	}

	/**
	 * Gets the position of the encoder.
	 */
	inline long getPosition() {
		disableInterrupts();
		long result = position;
		restoreInterrupts();
		return result;
	}

	/**
	 * Sets the position of the encoder.
	 */
	inline void setPosition(long newPosition) {
		disableInterrupts();
		setPosition_unsafe(newPosition);
		restoreInterrupts();
	}

	/**
	 * Sets the minValue and maxValue for the rotary encoder and fixes
	 * the position if it is out of bounds.
	 */
	inline void setMinMax(long newMinValue, long newMaxValue) {
		disableInterrupts();
		minValue = newMinValue;
		maxValue = newMaxValue;
		setPosition_unsafe(position);
		restoreInterrupts();
	}
};

#endif
