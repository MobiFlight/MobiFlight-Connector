/*
  pins_arduino.h - Pin definition functions for Arduino
  Part of Arduino - http://www.arduino.cc/

  Copyright (c) 2007 David A. Mellis

  This library is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public
  License as published by the Free Software Foundation; either
  version 2.1 of the License, or (at your option) any later version.

  This library is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
  Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General
  Public License along with this library; if not, write to the
  Free Software Foundation, Inc., 59 Temple Place, Suite 330,
  Boston, MA  02111-1307  USA
*/

#ifndef Pins_Arduino_h
#define Pins_Arduino_h

#include <avr/pgmspace.h>

// Workaround for wrong definitions in "iom32u4.h".
// This should be fixed in the AVR toolchain.
#undef UHCON
#undef UHINT
#undef UHIEN
#undef UHADDR
#undef UHFNUM
#undef UHFNUML
#undef UHFNUMH
#undef UHFLEN
#undef UPINRQX
#undef UPINTX
#undef UPNUM
#undef UPRST
#undef UPCONX
#undef UPCFG0X
#undef UPCFG1X
#undef UPSTAX
#undef UPCFG2X
#undef UPIENX
#undef UPDATX
#undef TCCR2A
#undef WGM20
#undef WGM21
#undef COM2B0
#undef COM2B1
#undef COM2A0
#undef COM2A1
#undef TCCR2B
#undef CS20
#undef CS21
#undef CS22
#undef WGM22
#undef FOC2B
#undef FOC2A
#undef TCNT2
#undef TCNT2_0
#undef TCNT2_1
#undef TCNT2_2
#undef TCNT2_3
#undef TCNT2_4
#undef TCNT2_5
#undef TCNT2_6
#undef TCNT2_7
#undef OCR2A
#undef OCR2_0
#undef OCR2_1
#undef OCR2_2
#undef OCR2_3
#undef OCR2_4
#undef OCR2_5
#undef OCR2_6
#undef OCR2_7
#undef OCR2B
#undef OCR2_0
#undef OCR2_1
#undef OCR2_2
#undef OCR2_3
#undef OCR2_4
#undef OCR2_5
#undef OCR2_6
#undef OCR2_7

#define NUM_DIGITAL_PINS  26
#define NUM_ANALOG_INPUTS 12

#define TX_RX_LED_INIT	DDRD |= (1<<5), DDRB |= (1<<0)
#define TXLED0			PORTD |= (1<<5)
#define TXLED1			PORTD &= ~(1<<5)
#define RXLED0			PORTB |= (1<<0)
#define RXLED1			PORTB &= ~(1<<0)

#define PIN_WIRE_SDA         (11)  //Set for lilypad
#define PIN_WIRE_SCL         (10)  //Set for lilypad

static const uint8_t SDA = PIN_WIRE_SDA;
static const uint8_t SCL = PIN_WIRE_SCL;

#define LED_BUILTIN 13
#define LED_BUILTIN_RX 0  //ok for lilypad?
#define LED_BUILTIN_TX 1  //ok for lilypad?

// Map SPI port to 'new' pins D14..D17
#define PIN_SPI_SS    (1)  //ok for lilypad?
#define PIN_SPI_MOSI  (22)  //ok for lilypad?
#define PIN_SPI_MISO  (23)  //ok for lilypad?
#define PIN_SPI_SCK   (21)  //ok for lilypad?

static const uint8_t SS   = PIN_SPI_SS;
static const uint8_t MOSI = PIN_SPI_MOSI;
static const uint8_t MISO = PIN_SPI_MISO;
static const uint8_t SCK  = PIN_SPI_SCK;

// Mapping of analog pins as digital I/O
// A6-A11 share with digital pins
#define PIN_A0   (16)
#define PIN_A1   (17)
#define PIN_A2   (2)
#define PIN_A3   (3)
#define PIN_A4   (4)
#define PIN_A5   (5)
#define PIN_A6   (18)
#define PIN_A7   (7)
#define PIN_A8   (8)
#define PIN_A9   (9)
#define PIN_A10  (12)
#define PIN_A11  (24)

static const uint8_t A0 = PIN_A0;
static const uint8_t A1 = PIN_A1;
static const uint8_t A2 = PIN_A2;
static const uint8_t A3 = PIN_A3;
static const uint8_t A4 = PIN_A4;
static const uint8_t A5 = PIN_A5;
static const uint8_t A6 = PIN_A6;	
static const uint8_t A7 = PIN_A7;	
static const uint8_t A8 = PIN_A8;	
static const uint8_t A9 = PIN_A9;	
static const uint8_t A10 = PIN_A10;	
static const uint8_t A11 = PIN_A11;	

//Interrupt enable
//This identifies the register
#define digitalPinToPCICR(p) ((((p) == 1) || ((p) == 4) || ((p) == 6) || ((p) == 8) || ((p) == 12) || ((p) == 21) || ((p) == 22) || ((p) == 23) || ((p) == A4) || ((p) == A8) || ((p) == A10)) ? (&PCICR) : ((uint8_t *)0))
// this returns bit position
#define digitalPinToPCICRbit(p) 0

//Interrupt source
//This identifies the register
#define digitalPinToPCMSK(p) ((((p) == 1) || ((p) == 4) || ((p) == 6) || ((p) == 8) || ((p) == 12) || ((p) == 21) || ((p) == 22) || ((p) == 23) || ((p) == A4) || ((p) == A8) || ((p) == A10)) ? (&PCMSK0) : ((uint8_t *)0))
//This returns bit position:
#define digitalPinToPCMSKbit(p) ( ((p) == 1) ? 0 : ((p) == 21) ? 1 : ((p) == 22) ? 2 : ((p) == 23) ? 3 : ((p) == 6) ? 7 : (((p) == 4)||((p) == A4)) ? 4 : (((p) == 8)||((p) == A8)) ? 5 : (((p) == 12)||((p) == A10)) ? 6 : (p - A8 + 4) )

//	__AVR_ATmega32U4__ has an unusual mapping of pins to channels
extern const uint8_t PROGMEM analog_pin_to_channel_PGM[];
#define analogPinToChannel(P)  ( pgm_read_byte( analog_pin_to_channel_PGM + (P) ) )// I don't understand, not modifying for now

#define digitalPinHasPWM(p)         ((p) == 6 || (p) == 7 || (p) == 8 || (p) == 10 || (p) == 12 || (p) == 13 || (p) == 14)

//This must be "INT" function
#define digitalPinToInterrupt(p) ((p) == 10 ? 0 : ((p) == 11 ? 1 : ((p) == 16 ? 2 : ((p) == 17 ? 3 : ((p) == 18 ? 4 : NOT_AN_INTERRUPT)))))

#ifdef ARDUINO_MAIN

// On the Arduino board, digital pins are also used
// for the analog output (software PWM).  Analog input
// pins are a separate set.

// these arrays map port names (e.g. port B) to the
// appropriate addresses for various functions (e.g. reading
// and writing)
const uint16_t PROGMEM port_to_mode_PGM[] = {
	NOT_A_PORT,
	NOT_A_PORT,
	(uint16_t) &DDRB,
	(uint16_t) &DDRC,
	(uint16_t) &DDRD,
	(uint16_t) &DDRE,
	(uint16_t) &DDRF,
};

const uint16_t PROGMEM port_to_output_PGM[] = {
	NOT_A_PORT,
	NOT_A_PORT,
	(uint16_t) &PORTB,
	(uint16_t) &PORTC,
	(uint16_t) &PORTD,
	(uint16_t) &PORTE,
	(uint16_t) &PORTF,
};

const uint16_t PROGMEM port_to_input_PGM[] = {
	NOT_A_PORT,
	NOT_A_PORT,
	(uint16_t) &PINB,
	(uint16_t) &PINC,
	(uint16_t) &PIND,
	(uint16_t) &PINE,
	(uint16_t) &PINF,
};

const uint8_t PROGMEM digital_pin_to_port_PGM[] = {
	PD, // D0
	PB,	// D1
	PD, // D2
	PF,	// D3
	PB,	// D4
	PF, // D5
	PB, // D6
	PD, // D7
	PB, // D8
	PF,	// D9
	PD, // D10
	PD,	// D11
	PB, // D12
	PC, // D13
	PC,	// D14
	PE,	// D15
	PF,	// D16
	PF,	// D17
	PF,	// D18
	PD, // D19
	PD, // D20
	PB, // D21
	PB, // D22
	PB, // D23
	PD, // D24
	PE, // D25
};

const uint8_t PROGMEM digital_pin_to_bit_mask_PGM[] = {
	_BV(5), // D0
	_BV(0),	// D1
	_BV(4), // D2
	_BV(7),	// D3
	_BV(4),	// D4
	_BV(4), // D5
	_BV(7), // D6
	_BV(7), // D7
	_BV(5), // D8
	_BV(6),	// D9
	_BV(0), // D10
	_BV(1),	// D11
	_BV(6), // D12
	_BV(6), // D13
	_BV(7),	// D14
	_BV(6),	// D15
	_BV(0),	// D16
	_BV(1),	// D17
	_BV(5),	// D18
	_BV(3), // D19
	_BV(2), // D20
	_BV(1), // D21
	_BV(2), // D22
	_BV(3), // D23
	_BV(6), // D24
	_BV(2), // D25

};

const uint8_t PROGMEM digital_pin_to_timer_PGM[] = {
	NOT_ON_TIMER,	
	NOT_ON_TIMER,
	NOT_ON_TIMER,
	TIMER0B,//3
	NOT_ON_TIMER,
	NOT_ON_TIMER,
	TIMER0A,//6
	TIMER4D,//7
	TIMER1A,//8
	NOT_ON_TIMER,
	NOT_ON_TIMER,
	NOT_ON_TIMER,
	TIMER1B,//12	
	TIMER3A,//13
	TIMER4A,//14
	NOT_ON_TIMER,
	NOT_ON_TIMER,
	NOT_ON_TIMER,
	NOT_ON_TIMER,
	NOT_ON_TIMER,
	NOT_ON_TIMER,
	NOT_ON_TIMER,
	NOT_ON_TIMER,
	NOT_ON_TIMER,
	NOT_ON_TIMER,
	NOT_ON_TIMER,
};

const uint8_t PROGMEM analog_pin_to_channel_PGM[] = {
	5,	// A0	
	1,	// A1	
	8,	// A2	
	7,	// A3	
	11,	// A4	
	4,	// A5	
	0,	// A6	
	10,	// A7	
	12,	// A8	
	6,	// A9	
	13,	// A10	
	9	// A11	
};

#endif /* ARDUINO_MAIN */

// These serial port names are intended to allow libraries and architecture-neutral
// sketches to automatically default to the correct port name for a particular type
// of use.  For example, a GPS module would normally connect to SERIAL_PORT_HARDWARE_OPEN,
// the first hardware serial port whose RX/TX pins are not dedicated to another use.
//
// SERIAL_PORT_MONITOR        Port which normally prints to the Arduino Serial Monitor
//
// SERIAL_PORT_USBVIRTUAL     Port which is USB virtual serial
//
// SERIAL_PORT_LINUXBRIDGE    Port which connects to a Linux system via Bridge library
//
// SERIAL_PORT_HARDWARE       Hardware serial port, physical RX & TX pins.
//
// SERIAL_PORT_HARDWARE_OPEN  Hardware serial ports which are open for use.  Their RX & TX
//                            pins are NOT connected to anything by default.
#define SERIAL_PORT_MONITOR        Serial
#define SERIAL_PORT_USBVIRTUAL     Serial
#define SERIAL_PORT_HARDWARE       Serial1
#define SERIAL_PORT_HARDWARE_OPEN  Serial1

// Alias SerialUSB to Serial
#define SerialUSB SERIAL_PORT_USBVIRTUAL

#endif /* Pins_Arduino_h */
