/*
  LilyPad ProtoSnap Plus
  Demo (Factory-Installed) Code
  SparkFun Electronics
  https://www.sparkfun.com/products/14346

  Load the demo to show off all features of the Prototyping Board.

  This sketch is complex, so use the other examples to get going if you are
  new to programming.

  -Switch-
  On: Cycle through the onboard LEDs.
  Off: Display light reading on the bar graph.

  -Button-
  Press to play a song.


  This code is covered by the MTT license and is completely free for any use.

  Virtuoso beeps by Marshall Taylor

******************************************************************************/
#include "notes.h"
#include "timeKeeper.h"

//Pairs of single-color LEDs
#define EXTLED5 5 	// yellow -- digital output only
#define EXTLED6 6 	// red -- PWM capable 
#define EXTLED7 7 	// green -- PWM capable 
#define EXTLED8 8 	// blue -- PWM capable 

//Center RGB LED, PWM capable
#define INTLEDR 12	// red channel
#define INTLEDG 13	// green channel
#define INTLEDB 14	// blue channel

//White bar-graph LEDs
#define INTLED0 15	// Left most
#define INTLED1 16	//
#define INTLED2 17	//
#define INTLED3 18	//
#define INTLED4 19	//
#define INTLED5 20	// Right most

//Inputs
#define BTNPIN 4    // Button (configure INPUT_PULLUP)
#define SWPIN 9     // Switch (configure INPUT_PULLUP)
#define LUXPIN A2   // Light sensor

//Buzzer
#define BZRPIN A3	// Buzzer

#define DELAY_A_MS 2

TimeKeeper stateTimer;

bool lastSwitchState;

// notes in the melody
int melody[] = {
  NOTE_C4, 0, NOTE_G3, 0, NOTE_G3, 0, NOTE_A3, 0, NOTE_G3, 0, NOTE_F3, 0, NOTE_E3, 0, NOTE_D3, 0, NOTE_E3, 0, NOTE_F3, 0, NOTE_E3, 0, NOTE_F3, 0, NOTE_G3, 0, NOTE_F3, 0, NOTE_G3, 0, NOTE_A3, 0, NOTE_G3, 0, NOTE_A3, 0, NOTE_B3, NOTE_F4, NOTE_G4, NOTE_B3, NOTE_F4, NOTE_G4, NOTE_B3, NOTE_F4, NOTE_G4, NOTE_B3, NOTE_F4, NOTE_G4, NOTE_B3, NOTE_F4, NOTE_G4, NOTE_B3, NOTE_F4, NOTE_G4, NOTE_B3, NOTE_F4, NOTE_G4, 0, NOTE_C4, NOTE_E4, NOTE_G4, NOTE_C4, NOTE_E4, NOTE_G4, NOTE_C4, NOTE_E4, NOTE_G4, NOTE_C4, NOTE_E4, NOTE_G4, NOTE_C4, NOTE_E4, NOTE_G4, NOTE_C4, NOTE_E4, NOTE_G4, NOTE_C4, NOTE_E4, NOTE_G4, NOTE_C4, NOTE_E4, NOTE_G4, NOTE_C4, NOTE_E4, NOTE_G4
};

// note durations
int noteDurations[] = {
  154, 38, 46, 104, 38, 4, 164, 28, 118, 254, 30, 6, 70, 14, 62, 4, 62, 4, 62, 4, 62, 4, 62, 4, 62, 4, 62, 10, 62, 16, 62, 28, 62, 28, 136, 182, 250, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 150, 250, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20, 20
};

void setup() {
  // initialize digital pin LED_BUILTIN as an output.
  pinMode(LUXPIN, INPUT);
  pinMode( BTNPIN, INPUT_PULLUP);
  pinMode( SWPIN, INPUT_PULLUP);
  lastSwitchState = digitalRead( SWPIN ) ^ 0x01; //insure switch position is detected on boot.
  resetLEDs();

}

int state = 0;

void loop()
{
  int luxReading;

  switch (state)
  {
    case 0:
      resetLEDs();
      digitalWrite(INTLED0, HIGH);   // turn the LED on (HIGH is the voltage level)
      stateTimer.mClear();
      state++;
      break;
    case 1:
      if (stateTimer.mGet() > 500) {
        digitalWrite(INTLED0, LOW);    // turn the LED off by making the voltage LOW
        digitalWrite(INTLED1, HIGH);   // turn the LED on (HIGH is the voltage level)
        stateTimer.mClear();
        state++;
      }
      break;
    case 2:
      if (stateTimer.mGet() > 500) {
        digitalWrite(INTLED1, LOW);    // turn the LED off by making the voltage LOW
        digitalWrite(INTLED2, HIGH);   // turn the LED on (HIGH is the voltage level)
        stateTimer.mClear();
        state++;
      }
      break;
    case 3:
      if (stateTimer.mGet() > 500) {
        digitalWrite(INTLED2, LOW);    // turn the LED off by making the voltage LOW
        digitalWrite(INTLED3, HIGH);   // turn the LED on (HIGH is the voltage level)
        stateTimer.mClear();
        state++;
      }
      break;
    case 4:
      if (stateTimer.mGet() > 500) {
        digitalWrite(INTLED3, LOW);    // turn the LED off by making the voltage LOW
        digitalWrite(INTLED4, HIGH);   // turn the LED on (HIGH is the voltage level)
        stateTimer.mClear();
        state++;
      }
      break;
    case 5:
      if (stateTimer.mGet() > 500) {
        digitalWrite(INTLED4, LOW);    // turn the LED off by making the voltage LOW
        digitalWrite(INTLED5, HIGH);   // turn the LED on (HIGH is the voltage level)
        stateTimer.mClear();
        state++;
      }
      break;
    case 6:
      if (stateTimer.mGet() > 500) {
        digitalWrite(INTLED5, LOW);    // turn the LED off by making the voltage LOW
        digitalWrite(EXTLED5, HIGH);   // turn the LED on (HIGH is the voltage level)
        stateTimer.mClear();
        state++;
      }
      break;
    case 7:
      if (stateTimer.mGet() > 500) {
        digitalWrite(EXTLED5, LOW);    // turn the LED off by making the voltage LOW
        stateTimer.mClear();
        state++;
      }
      break;
    case 8:
      if (stateTimer.mGet() < 512) {
        analogWrite(EXTLED6, stateTimer.mGet() >> 2);
      } else {
        analogWrite(EXTLED6, 0);
        stateTimer.mClear();
        state++;
      }
      break;
    case 9:
      if (stateTimer.mGet() < 512) {
        analogWrite(EXTLED7, stateTimer.mGet() >> 2);
      } else {
        analogWrite(EXTLED7, 0);
        stateTimer.mClear();
        state++;
      }
      break;
    case 10:
      if (stateTimer.mGet() < 512) {
        analogWrite(EXTLED8, stateTimer.mGet() >> 2);
      } else {
        analogWrite(EXTLED8, 0);
        stateTimer.mClear();
        state++;
      }
      break;
    case 11:
      if (stateTimer.mGet() < 512) {
        analogWrite(INTLEDR, stateTimer.mGet() >> 2);
      } else {
        analogWrite(INTLEDR, 0);
        stateTimer.mClear();
        state++;
      }
      break;
    case 12:
      if (stateTimer.mGet() < 512) {
        analogWrite(INTLEDG, stateTimer.mGet() >> 2);
      } else {
        analogWrite(INTLEDG, 0);
        stateTimer.mClear();
        state++;
      }
      break;
    case 13:
      if (stateTimer.mGet() < 512) {
        analogWrite(INTLEDB, stateTimer.mGet() >> 2);
      } else {
        analogWrite(INTLEDB, 0);
        digitalWrite(INTLED0, HIGH);   // turn the LED on (HIGH is the voltage level)
        stateTimer.mClear();
        state = 1;
      }
      break;
    case 14:
      resetLEDs();
      pinMode(INTLED0, INPUT);
      pinMode(INTLED1, INPUT);
      pinMode(INTLED2, INPUT);
      pinMode(INTLED3, INPUT);
      pinMode(INTLED4, INPUT);
      pinMode(INTLED5, INPUT);
      state++;
      break;
    case 15:
      luxReading = analogRead(LUXPIN);
      drawBarGraph(luxReading);
      break;
    default:
      break;
  }

  //Process inputs
  bool currentSwitchState = digitalRead( SWPIN );
  if ( currentSwitchState != lastSwitchState)
  {
    lastSwitchState = currentSwitchState; //save the current state
    if (currentSwitchState == 0)
    {
      //Do one thing
      state = 0;
    }
    else
    {
      //Do the other thing
      state = 14;
    }
  }
  if ( digitalRead( BTNPIN ) == 0 )
  {
    playSong();
  }

  //Increment timers and wait
  delay(2);
  stateTimer.mIncrement(2);

}

void playSong( void )
{
  //Save TCCR3A and TCCR3B
  byte temp_TCCR3A = TCCR3A;
  byte temp_TCCR3B = TCCR3B;

  for (int thisNote = 0; thisNote < 85; thisNote++) {

    int noteDuration = (float)noteDurations[thisNote] * 2.3;
    tone(BZRPIN, melody[thisNote], noteDuration);

    delay(noteDuration);
    // stop the tone playing:
    noTone(BZRPIN);
  }

  //Restore TCCR3A and TCCR3B
  TCCR3A = temp_TCCR3A;
  TCCR3B = temp_TCCR3B;

}


void drawBarGraph( int inputVar )
{
  int increment = (200 / 7);
  if ( inputVar > (increment * 1) ) {
    digitalWrite(INTLED0, 1);
  } else {
    digitalWrite(INTLED0, 0);
  }
  if ( inputVar > (increment * 2) ) {
    digitalWrite(INTLED1, 1);
  } else {
    digitalWrite(INTLED1, 0);
  }
  if ( inputVar > (increment * 3) ) {
    digitalWrite(INTLED2, 1);
  } else {
    digitalWrite(INTLED2, 0);
  }
  if ( inputVar > (increment * 4) ) {
    digitalWrite(INTLED3, 1);
  } else {
    digitalWrite(INTLED3, 0);
  }
  if ( inputVar > (increment * 5) ) {
    digitalWrite(INTLED4, 1);
  } else {
    digitalWrite(INTLED4, 0);
  }
  if ( inputVar > (increment * 6) ) {
    digitalWrite(INTLED5, 1);
  } else {
    digitalWrite(INTLED5, 0);
  }

}

void resetLEDs( void )
{
  digitalWrite( INTLED0, 0 );
  digitalWrite( INTLED1, 0 );
  digitalWrite( INTLED2, 0 );
  digitalWrite( INTLED3, 0 );
  digitalWrite( INTLED4, 0 );
  digitalWrite( INTLED5, 0 );

  pinMode(INTLED0, OUTPUT);
  pinMode(INTLED1, OUTPUT);
  pinMode(INTLED2, OUTPUT);
  pinMode(INTLED3, OUTPUT);
  pinMode(INTLED4, OUTPUT);
  pinMode(INTLED5, OUTPUT);

  digitalWrite( EXTLED5, 0 );
  analogWrite(EXTLED6, 0);
  analogWrite(EXTLED7, 0);
  analogWrite(EXTLED8, 0);
  analogWrite(INTLEDR, 0);
  analogWrite(INTLEDG, 0);
  analogWrite(INTLEDB, 0);

  pinMode(EXTLED5, OUTPUT);
  pinMode(EXTLED6, OUTPUT);
  pinMode(EXTLED7, OUTPUT);
  pinMode(EXTLED8, OUTPUT);
  pinMode(INTLEDR, OUTPUT);
  pinMode(INTLEDG, OUTPUT);
  pinMode(INTLEDB, OUTPUT);

}