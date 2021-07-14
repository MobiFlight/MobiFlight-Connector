/*
LilyPad ProtoSnap Plus Activity 1: Blinking LEDs
SparkFun Electronics
https://www.sparkfun.com/products/14346

Blink the pair of yellow LEDs attached to sew tab A5 on the LilyPad USB Plus

Follow the tutorial at: https://learn.sparkfun.com/tutorials/lilypad-protosnap-plus-activity-guide#1-blinking-leds

This example is based on: Blink by Scott Fitzgerald
https://www.arduino.cc/en/Tutorial/Blink

This code is released under the MIT License (http://opensource.org/licenses/MIT)

******************************************************************************/

// The setup function runs once when the microcontroller starts up or resets:

void setup()
{
  // Before you use a sew tab (pin), you must set it to be either an input or output:
  
  pinMode(A5, OUTPUT);     // Set pin A5 to be an output
}

// After the setup function runs, the loop function runs over and over forever:

void loop()
{
  digitalWrite(A5, HIGH);  // Give pin A5 a HIGH voltage level (on), which lights up the LED
  delay(1000);             // Pause for 1000 milliseconds (one second), the LED stays on
  digitalWrite(A5, LOW);   // Give pin A5 a LOW voltage level (off), which turns off the LED
  delay(1000);             // Pause for 1000 milliseconds (one second), the LED stays off
}

