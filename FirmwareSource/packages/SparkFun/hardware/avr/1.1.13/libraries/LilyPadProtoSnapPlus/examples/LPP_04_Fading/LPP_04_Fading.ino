/*
LilyPad ProtoSnap Plus Activity 4: Fading LEDs
SparkFun Electronics
https://www.sparkfun.com/products/14346

Use for-loops to smoothly vary the brightness of LEDs

Follow the tutorial at: https://learn.sparkfun.com/tutorials/lilypad-protosnap-plus-activity-guide#4-fading-leds

This code is released under the MIT License (http://opensource.org/licenses/MIT)

******************************************************************************/

// In the previous activity we showed you how to manually change the brightness of a LED.
// In this activity we'll show you how to program the computer to do all the work.

// Create integer variables for the LED pins we'll be using:

int redLED = 6;
int yellowLED = 5;

void setup()
{
  // Set the LED pins to be outputs:
  
  pinMode(redLED, OUTPUT);
  pinMode(yellowLED, OUTPUT);
}

void loop()
{
  // The two "for loops" below will make a LED fade on and off in a "breathing" pattern.
  
  // Create a new integer variable called brightness:
  
  int brightness;
  
  // Now we'll have the program automatically change the value of brightness
  // using a command called "for".

  // for is like a tiny version of loop. The for command has several parts:
  // 1. something to do before starting (brightness = 0)
  // 2. a test to decide whether to keep going (brightness <= 255)
  // 3. a block of commands to run (everything within the {} below the for)
  // 4. a command to run before doing it again (brightness = brightness + 1)

  // Here's a for command which will start brightness at 0, check to see if it's less than
  // or equal to 255, run the commands after it, then add one to brightness and start over:

  for (brightness = 0; brightness <= 255; brightness = brightness + 1)
  {
    // Within the loop, we'll use brightnes variable to control the brigthness of the LEDs:

    analogWrite(redLED, brightness);
    analogWrite(yellowLED, brightness);

    // NOTE that not all pins work with analogWrite!
    // The ones with a "~" in front of them will change brightness,
    // the others will only turn on if brightness > 128.
    // Both types are used above, run the code and note the difference between them.
    
    // The delay command controls the speed - if you make the delay larger,
    // it will slow down the loop. Smaller, and it will run faster:

    delay(5);
  }

  // What if we want the LED to start at full brightness and fade to black?
  // We can easily set up the for loop to run in reverse:
  
  for (brightness = 255; brightness >= 0; brightness = brightness - 1)
  {
    analogWrite(redLED, brightness);
    analogWrite(yellowLED, brightness);
    delay(5);
  }
}

