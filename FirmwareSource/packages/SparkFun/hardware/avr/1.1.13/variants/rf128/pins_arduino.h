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

  $Id: wiring.h 249 2007-02-03 16:52:51Z mellis $
*/

/*
	This version of pins_arduino.h is for the Zigduino r1
	Pierce Nichols 2011 Oct 11
	Amended for the SparkFun ATmega128RFA1 -- Jim Lindblom 2013 Jun 29
*/

#ifndef Pins_Arduino_h
#define Pins_Arduino_h

#include <avr/pgmspace.h>

#define NUM_DIGITAL_PINS            26
#define NUM_ANALOG_INPUTS           8
#define analogInputToDigitalPin(p)  ((p < 8) ? (p) + 26 : -1)
#define digitalPinHasPWM(p)         ((p) == 3 ||(p) == 4 ||(p) == 5 ||(p) == 8 ||(p) == 9 ||(p) == 19 ||(p) == 34 || (p) == 35)

// Dev board specific defines: RF RX and TX LEDs:
#define RXLED_DDR  DDRB
#define RXLED_PORT PORTB
#define RXLED_POS  PB6

#define TXLED_DDR  DDRB
#define TXLED_PORT PORTB
#define TXLED_POS  PB7  

const static uint8_t SS   = 10;
const static uint8_t MOSI = 11;
const static uint8_t MISO = 12;
const static uint8_t SCK  = 13;

const static uint8_t SDA = 14;
const static uint8_t SCL = 15;
const static uint8_t LED = 34;
const static uint8_t LED1 = 34;
const static uint8_t LED2 = 35;

const static uint8_t A0 = 26;
const static uint8_t A1 = 27;
const static uint8_t A2 = 28;
const static uint8_t A3 = 29;
const static uint8_t A4 = 30;
const static uint8_t A5 = 31;
const static uint8_t A6 = 32;
const static uint8_t A7 = 33;

// A majority of the pins are NOT PCINTs, SO BE WARNED (i.e. you cannot use them as receive pins)
// Only pins available for RECEIVE (TRANSMIT can be on any pin):
// Pins: 7, 8, 9, 10, 11, 12, 13, 20

#define digitalPinToPCICR(p)    ( (((p) >= 7) && ((p) <= 13)) || \
                                  ((p) == 20) ? (&PCICR) : ((uint8_t *)0) )

#define digitalPinToPCICRbit(p) ( ((p) == 7) ? 1 : 0 ) 

#define digitalPinToPCMSK(p)    ( ((((p) >= 8) && ((p) <= 13)) || ((p) == 20)) ? (&PCMSK0) : \
                                ( ((p) == 7) ? (&PCMSK1) : \
                                ((uint8_t *)0) ) )

#define digitalPinToPCMSKbit(p) ( ((p) == 7) ? 0 : \
                                ( ((p) == 8) ? 4 : \
                                ( ((p) == 9) ? 7 : \
                                ( ((p) == 10) ? 6 : \
                                ( ((p) == 11) ? 5 : \
                                ( ((p) == 12) ? 3 : \
                                ( ((p) == 13) ? 1 : \
                                ( ((p) == 20) ? 2 : \
                                0 ) ) ) ) ) )

#ifdef ARDUINO_MAIN

const uint16_t PROGMEM port_to_mode_PGM[] = {
	NOT_A_PORT,
	NOT_A_PORT,
	(uint16_t)&DDRB,
	NOT_A_PORT,
	(uint16_t)&DDRD,
	(uint16_t)&DDRE,
	(uint16_t)&DDRF,
	(uint16_t)&DDRG,
	NOT_A_PORT,
	NOT_A_PORT,
	NOT_A_PORT,
	NOT_A_PORT,
	NOT_A_PORT,
};

const uint16_t PROGMEM port_to_output_PGM[] = {
	NOT_A_PORT,
	NOT_A_PORT,
	(uint16_t)&PORTB,
	NOT_A_PORT,
	(uint16_t)&PORTD,
	(uint16_t)&PORTE,
	(uint16_t)&PORTF,
	(uint16_t)&PORTG,
	NOT_A_PORT,
	NOT_A_PORT,
	NOT_A_PORT,
	NOT_A_PORT,
	NOT_A_PORT,
};

const uint16_t PROGMEM port_to_input_PGM[] = {
	NOT_A_PIN,
	NOT_A_PIN,
	NOT_A_PIN,
	(uint16_t)&PINC,
	(uint16_t)&PIND,
	(uint16_t)&PINE,
	(uint16_t)&PINF,
	(uint16_t)&PING,
	NOT_A_PIN,
	NOT_A_PIN,
	NOT_A_PIN,
	NOT_A_PIN,
	NOT_A_PIN,
};

const uint8_t PROGMEM digital_pin_to_port_PGM[] = {
	// PORTLIST		
	// -------------------------------------------		
	PE	, // PE 0 ** 0 ** D0 / USART0_RX	
	PE	, // PE 1 ** 1 ** D1 / USART0_TX	
	PE	, // PE 2 ** 2 ** D2
	PE	, // PE 3 ** 3 ** D3 / PWM
	PE	, // PE 4 ** 4 ** D4 / PWM
	PE	, // PE 5 ** 5 ** D5 / PWM
	PE	, // PE 6 ** 6 ** D6
	PE	, // PE 7 ** 7 ** D7
	PB	, // PB 5 ** 8 ** D8 / PWM
	PB	, // PB 4 ** 9 ** D9 / PWM
	PB	, // PB 0 ** 10 ** D10 / SPI_SSN
	PB	, // PB 2 ** 11 ** D11 / SPI_MOSI
	PB	, // PB 3 ** 12 ** D12 / SPI_MISO
	PB	, // PB 1 ** 13 ** D13 / SPI_SCK
	PD	, // PD 1 ** 14 ** D14 / I2C_SDA
	PD	, // PD 0 ** 15 ** D15 / I2C_SCL
	PG	, // PG 0 ** 16 ** D16
	PG	, // PG 1 ** 17 ** D17
	PG	, // PG 2 ** 18 ** D18
	PG	, // PG 5 ** 19 ** D19 / PWM
	PD	, // PD 2 ** 20 ** D20 / USART1_RX
	PD	, // PD 3 ** 21 ** D21 / USART1_TX
	PD	, // PD 4 ** 22 ** D22
	PD	, // PD 5 ** 23 ** D23
	PD	, // PD 6 ** 24 ** D24
	PD	, // PD 7 ** 25 ** D25
	PF	, // PF 0 ** 26 ** A0 / D26
	PF	, // PF 1 ** 27 ** A1 / D27
	PF	, // PF 2 ** 28 ** A2 / D28
	PF	, // PF 3 ** 29 ** A3 / D29
	PF	, // PF 4 ** 30 ** A4 / D30	
	PF	, // PF 5 ** 31 ** A5 / D31	
	PF	, // PF 6 ** 32 ** A6 / D32	
	PF	, // PF 7 ** 33 ** A7 / D33
	PB	, // PB 6 ** 34 ** D34 / LED1 / LED / PWM
	PB	, // PB 7 ** 35 ** D35 / LED2 / PWM
};

const uint8_t PROGMEM digital_pin_to_bit_mask_PGM[] = {
	// PIN IN PORT		
	// -------------------------------------------		
	_BV( 0 )	, // PE 0 ** 0 ** USART0_RX	
	_BV( 1 )	, // PE 1 ** 1 ** USART0_TX	
	_BV( 2 )	, // PE 2 ** 2 ** D2
	_BV( 3 )	, // PE 3 ** 3 ** D3 / PWM
	_BV( 4 )	, // PE 4 ** 4 ** D4 / PWM
	_BV( 5 )	, // PE 5 ** 5 ** D5 / PWM
	_BV( 6 )	, // PE 6 ** 6 ** D6
	_BV( 7 )	, // PE 7 ** 7 ** D7
	_BV( 5 )	, // PB 5 ** 8 ** D8 / PWM
	_BV( 4 )	, // PB 4 ** 9 ** D9 / PWM
	_BV( 0 )	, // PB 0 ** 10 ** D10 / SPI_SSN
	_BV( 2 )	, // PB 2 ** 11 ** D11 / SPI_MOSI
	_BV( 3 )	, // PB 3 ** 12 ** D12 / SPI_MISO
	_BV( 1 )	, // PB 1 ** 13 ** D13 / SPI_SCK
	_BV( 1 )	, // PD 1 ** 14 ** D14 / I2C_SDA
	_BV( 0 )	, // PD 0 ** 15 ** D15 / I2C_SCL
	_BV( 0 )	, // PG 0 ** 16 ** D16
	_BV( 1 )	, // PG 1 ** 17 ** D17	
	_BV( 2 )	, // PG 2 ** 18 ** D18	
	_BV( 5 )	, // PG 5 ** 19 ** D19 / PWM
	_BV( 2 )	, // PD 2 ** 20 ** D20 / USART1_RX
	_BV( 3 )	, // PD 3 ** 21 ** D21 / USART1_TX
	_BV( 4 )	, // PD 4 ** 22 ** D22
	_BV( 5 )  	, // PD 5 ** 23 ** D23
	_BV( 6 )  	, // PD 6 ** 24 ** D24
	_BV( 7 )	, // PD 7 ** 25 ** D25
	_BV( 0 )	, // PF 0 ** 26 ** A0 / D26
	_BV( 1 )	, // PF 1 ** 27 ** A1 / D27
	_BV( 2 )	, // PF 2 ** 28 ** A2 / D28
	_BV( 3 )	, // PF 3 ** 29 ** A3 / D29
	_BV( 4 )	, // PF 4 ** 30 ** A4 / D30	
	_BV( 5 )	, // PF 5 ** 31 ** A5 / D31	
	_BV( 6 )	, // PF 6 ** 32 ** A6 / D32	
	_BV( 7 )	, // PF 7 ** 33 ** A7 / D33	
	_BV( 6 )	, // PB 6 ** 34 ** D34 / LED1 / LED / PWM
	_BV( 7 )	, // PB 7 ** 35 ** D35 / LED2 / PWM
};

// !!!
const uint8_t PROGMEM digital_pin_to_timer_PGM[] = {
	// TIMERS		
	// -------------------------------------------	
	NOT_ON_TIMER	, // PE 0 ** 0 ** USART0_RX	
	NOT_ON_TIMER	, // PE 1 ** 1 ** USART0_TX	
	NOT_ON_TIMER	, // PE 2 ** 2 ** D2
	TIMER3A			, // PE 3 ** 3 ** D3 / PWM
	TIMER3B			, // PE 4 ** 4 ** D4 / PWM
	TIMER3C			, // PE 5 ** 5 ** D5 / PWM
	NOT_ON_TIMER	, // PE 6 ** 6 ** D6
	NOT_ON_TIMER	, // PE 7 ** 7 ** D7
	TIMER1A			, // PB 5 ** 8 ** D8 / PWM
	TIMER2A			, // PB 4 ** 9 ** D9 / PWM
	NOT_ON_TIMER	, // PB 0 ** 10 ** D10 / SPI_SSN
	NOT_ON_TIMER	, // PB 2 ** 11 ** D11 / SPI_MOSI
	NOT_ON_TIMER	, // PB 3 ** 12 ** D12 / SPI_MISO
	NOT_ON_TIMER	, // PB 1 ** 13 ** D13 / SPI_SCK
	NOT_ON_TIMER	, // PD 1 ** 14 ** D14 / I2C_SDA
	NOT_ON_TIMER	, // PD 0 ** 15 ** D15 / I2C_SCL
	NOT_ON_TIMER	, // PG 0 ** 16 ** D16
	NOT_ON_TIMER	, // PG 1 ** 17 ** D17	
	NOT_ON_TIMER	, // PG 2 ** 18 ** D18	
	TIMER0B			, // PG 5 ** 19 ** D19 / PWM
	NOT_ON_TIMER	, // PD 2 ** 20 ** D20 / USART1_RX
	NOT_ON_TIMER	, // PD 3 ** 21 ** D21 / USART1_TX
	NOT_ON_TIMER	, // PD 4 ** 22 ** D22
	NOT_ON_TIMER  	, // PD 5 ** 23 ** D23
	NOT_ON_TIMER  	, // PD 6 ** 24 ** D24
	NOT_ON_TIMER	, // PD 7 ** 25 ** D25
	NOT_ON_TIMER	, // PF 0 ** 26 ** A0 / D26
	NOT_ON_TIMER	, // PF 1 ** 27 ** A1 / D27
	NOT_ON_TIMER	, // PF 2 ** 28 ** A2 / D28
	NOT_ON_TIMER	, // PF 3 ** 29 ** A3 / D29
	NOT_ON_TIMER	, // PF 4 ** 30 ** A4 / D30	
	NOT_ON_TIMER	, // PF 5 ** 31 ** A5 / D31	
	NOT_ON_TIMER	, // PF 6 ** 32 ** A6 / D32	
	NOT_ON_TIMER	, // PF 7 ** 33 ** A7 / D33	
	TIMER1B			, // PB 6 ** 34 ** D34/ PWM
	TIMER1C			, // PB 7 ** 35 ** D35 / PWM
};	

#endif

#endif