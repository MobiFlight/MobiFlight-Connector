/*
  LilyPad ProtoSnap Plus
  Buzzer.ino
  SparkFun Electronics
  https://www.sparkfun.com/products/14346

  This is an adaption of the toneMelody example, with only the output pin
  changed.
  
  The original code is in the public domain.

  http://www.arduino.cc/en/Tutorial/Tone

  created 21 Jan 2010
  modified 30 Aug 2011
  by Tom Igoe
 
******************************************************************************/
#include "pitches.h"

//Buzzer
#define BZRPIN A3	// Buzzer

// notes in the melody:
int melody[] = {
  NOTE_C4, NOTE_G3, NOTE_G3, NOTE_A3, NOTE_G3, 0, NOTE_B3, NOTE_C4
};

// note durations: 4 = quarter note, 8 = eighth note, etc.:
int noteDurations[] = {
  4, 8, 8, 4, 4, 4, 4, 4
};

void setup() {
  // iterate over the notes of the melody:
  for (int thisNote = 0; thisNote < 8; thisNote++) {

    // to calculate the note duration, take one second
    // divided by the note type.
    //e.g. quarter note = 1000 / 4, eighth note = 1000/8, etc.
    int noteDuration = 1000 / noteDurations[thisNote];
    tone(BZRPIN, melody[thisNote], noteDuration);

    // to distinguish the notes, set a minimum time between them.
    // the note's duration + 30% seems to work well:
    int pauseBetweenNotes = noteDuration * 1.30;
    delay(pauseBetweenNotes);
    // stop the tone playing:
    noTone(BZRPIN);
  }
}

void loop() {
  // no need to repeat the melody.
}