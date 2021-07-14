/*
  LilyPad ProtoSnap Plus
  LEDs.ino
  SparkFun Electronics
  https://www.sparkfun.com/products/14346

  This example walks through each of the LEDs, using PWM where possible.

******************************************************************************/
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

#define DELAY_A_MS 2 //Delay 2ms per PWM step

void setup() {
  //Config LED pins
  pinMode(INTLED0, OUTPUT);
  pinMode(INTLED1, OUTPUT);
  pinMode(INTLED2, OUTPUT);
  pinMode(INTLED3, OUTPUT);
  pinMode(INTLED4, OUTPUT);
  pinMode(INTLED5, OUTPUT);

  pinMode(EXTLED5, OUTPUT);
  pinMode(EXTLED6, OUTPUT);
  pinMode(EXTLED7, OUTPUT);
  pinMode(EXTLED8, OUTPUT);

  pinMode(INTLEDR, OUTPUT);
  pinMode(INTLEDG, OUTPUT);
  pinMode(INTLEDB, OUTPUT);

}

void loop() {
  digitalWrite(INTLED0, HIGH);   // turn the LED on (HIGH is the voltage level)
  delay(500);                       // wait for a second
  digitalWrite(INTLED0, LOW);    // turn the LED off by making the voltage LOW

  digitalWrite(INTLED1, HIGH);   // turn the LED on (HIGH is the voltage level)
  delay(500);                       // wait for a second
  digitalWrite(INTLED1, LOW);    // turn the LED off by making the voltage LOW

  digitalWrite(INTLED2, HIGH);   // turn the LED on (HIGH is the voltage level)
  delay(500);                       // wait for a second
  digitalWrite(INTLED2, LOW);    // turn the LED off by making the voltage LOW

  digitalWrite(INTLED3, HIGH);   // turn the LED on (HIGH is the voltage level)
  delay(500);                       // wait for a second
  digitalWrite(INTLED3, LOW);    // turn the LED off by making the voltage LOW

  digitalWrite(INTLED4, HIGH);   // turn the LED on (HIGH is the voltage level)
  delay(500);                       // wait for a second
  digitalWrite(INTLED4, LOW);    // turn the LED off by making the voltage LOW

  digitalWrite(INTLED5, HIGH);   // turn the LED on (HIGH is the voltage level)
  delay(500);                       // wait for a second
  digitalWrite(INTLED5, LOW);    // turn the LED off by making the voltage LOW

  digitalWrite(EXTLED5, HIGH);   // turn the LED on (HIGH is the voltage level)
  delay(500);                       // wait for a second
  digitalWrite(EXTLED5, LOW);    // turn the LED off by making the voltage LOW

  for (int i = 0; i < 255; i++)
  {
    analogWrite(EXTLED6, i);
    delay(DELAY_A_MS);
  }
  analogWrite(EXTLED6, 0);

  for (int i = 0; i < 255; i++)
  {
    analogWrite(EXTLED7, i);
    delay(DELAY_A_MS);
  }
  analogWrite(EXTLED7, 0);

  for (int i = 0; i < 255; i++)
  {
    analogWrite(EXTLED8, i);
    delay(DELAY_A_MS);
  }
  analogWrite(EXTLED8, 0);

  for (int i = 0; i < 255; i++)
  {
    analogWrite(INTLEDR, i);
    delay(DELAY_A_MS);
  }
  analogWrite(INTLEDR, 0);

  for (int i = 0; i < 255; i++)
  {
    analogWrite(INTLEDG, i);
    delay(DELAY_A_MS);
  }
  analogWrite(INTLEDG, 0);

  for (int i = 0; i < 255; i++)
  {
    analogWrite(INTLEDB, i);
    delay(DELAY_A_MS);
  }
  analogWrite(INTLEDB, 0);
}