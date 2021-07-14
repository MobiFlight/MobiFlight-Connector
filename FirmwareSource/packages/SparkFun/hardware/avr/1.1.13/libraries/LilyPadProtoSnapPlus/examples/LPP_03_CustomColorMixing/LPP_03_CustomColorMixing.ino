/*
LilyPad ProtoSnap Plus Activity 3: Custom Color Mixing
SparkFun Electronics
https://www.sparkfun.com/products/14346

Expand your color options using analogWrite and the RGB (Red Green Blue) LED

Follow the tutorial at: https://learn.sparkfun.com/tutorials/lilypad-protosnap-plus-activity-guide#3-custom-color-mixing

This code is released under the MIT License (http://opensource.org/licenses/MIT)

******************************************************************************/

// The LilyPad USB Plus has a built-in RGB (Red / Green / Blue) LED.
// In this activity we'll use analogWrite to control the brightness of the three LEDs.
// Here we'll create a rainbow of tertiary colors by adding a 50%-brightness option.

// Create integer variables for our LED pins:

// The built-in LED:

int RGB_red = 12;
int RGB_green = 13;
int RGB_blue = 14;

// The colored LEDs along the bottom edge of the board:

int redLED = 6;
int greenLED = A7;
int blueLED = A8;

void setup() {

// Make all of our LED pins outputs:

  pinMode(RGB_red, OUTPUT);
  pinMode(RGB_green, OUTPUT);
  pinMode(RGB_blue, OUTPUT);
  pinMode(redLED, OUTPUT);
  pinMode(greenLED, OUTPUT);
  pinMode(blueLED, OUTPUT);
}

void loop()
{
  // In this code we'll step through twelve rainbow colors (primary, secondary, tertiary).

  // Unlike digitalWrite, which can be only HIGH (on) or LOW (off),
  // analogWrite lets you smoothly change the brightness from 0 (off) to 255 (fully on).
  // When analogWrite is used with the RGB LED, you can create millions of colors!

  // In the analogWrite functions:
  // 0 is off
  // 128 is halfway on (used for the tertiary colors)
  // 255 is full brigthness.
  
  // Red

  analogWrite(RGB_red,255);
  analogWrite(RGB_green,0);
  analogWrite(RGB_blue,0);

  analogWrite(redLED,255);
  analogWrite(greenLED,0);
  analogWrite(blueLED,0);
  delay(1000);

  // Orange

  analogWrite(RGB_red,255);
  analogWrite(RGB_green,128);
  analogWrite(RGB_blue,0);

  analogWrite(redLED,255);
  analogWrite(greenLED,128);
  analogWrite(blueLED,0);
  delay(1000);

  // Yellow

  analogWrite(RGB_red,255);
  analogWrite(RGB_green,255);
  analogWrite(RGB_blue,0);

  analogWrite(redLED,255);
  analogWrite(greenLED,255);
  analogWrite(blueLED,0);
  delay(1000);

  // Chartruese

  analogWrite(RGB_red,128);
  analogWrite(RGB_green,255);
  analogWrite(RGB_blue,0);

  analogWrite(redLED,128);
  analogWrite(greenLED,255);
  analogWrite(blueLED,0);
  delay(1000);

  // Green

  analogWrite(RGB_red,0);
  analogWrite(RGB_green,255);
  analogWrite(RGB_blue,0);

  analogWrite(redLED,0);
  analogWrite(greenLED,255);
  analogWrite(blueLED,0);
  delay(1000);

  // Spring Green

  analogWrite(RGB_red,0);
  analogWrite(RGB_green,255);
  analogWrite(RGB_blue,128);

  analogWrite(redLED,0);
  analogWrite(greenLED,255);
  analogWrite(blueLED,128);
  delay(1000);

  // Cyan

  analogWrite(RGB_red,0);
  analogWrite(RGB_green,255);
  analogWrite(RGB_blue,255);

  analogWrite(redLED,0);
  analogWrite(greenLED,255);
  analogWrite(blueLED,255);
  delay(1000);

  // Azure

  analogWrite(RGB_red,0);
  analogWrite(RGB_green,128);
  analogWrite(RGB_blue,255);

  analogWrite(redLED,0);
  analogWrite(greenLED,128);
  analogWrite(blueLED,255);
  delay(1000);

  // Blue

  analogWrite(RGB_red,0);
  analogWrite(RGB_green,0);
  analogWrite(RGB_blue,255);

  analogWrite(redLED,0);
  analogWrite(greenLED,0);
  analogWrite(blueLED,255);
  delay(1000);

  // Violet

  analogWrite(RGB_red,128);
  analogWrite(RGB_green,0);
  analogWrite(RGB_blue,255);

  analogWrite(redLED,128);
  analogWrite(greenLED,0);
  analogWrite(blueLED,255);
  delay(1000);

  // Magenta
  
  analogWrite(RGB_red,255);
  analogWrite(RGB_green,0);
  analogWrite(RGB_blue,255);

  analogWrite(redLED,255);
  analogWrite(greenLED,0);
  analogWrite(blueLED,255);
  delay(1000);

  // Rose
  
  analogWrite(RGB_red,255);
  analogWrite(RGB_green,0);
  analogWrite(RGB_blue,128);
  
  analogWrite(redLED,255);
  analogWrite(greenLED,0);
  analogWrite(blueLED,128);
  delay(1000);

}
