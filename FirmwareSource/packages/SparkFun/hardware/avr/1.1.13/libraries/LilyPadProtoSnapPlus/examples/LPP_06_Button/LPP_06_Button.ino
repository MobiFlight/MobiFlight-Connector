/*
LilyPad ProtoSnap Plus Activity 6: Buttons and Switches
SparkFun Electronics
https://www.sparkfun.com/products/14346

Explore digital input and program flow control using the button and switch

Follow the tutorial at: https://learn.sparkfun.com/tutorials/lilypad-protosnap-plus-activity-guide#6-buttons-and-switches

This code is released under the MIT License (http://opensource.org/licenses/MIT)

******************************************************************************/

// Create integer variables for the pins we'll be using

int buttonPin = A4;
int switchPin = A9;

int buttonLED = A5;
int switchLED = A8;

void setup()
{
  // Initialize the button and switch pins as inputs with pullups.
  // Pullups keep the inputs from "floating" when a switch or button is open / unpressed.

  pinMode(buttonPin, INPUT_PULLUP);
  pinMode(switchPin, INPUT_PULLUP);

  // Initialize the LED pins as outputs:

  pinMode(buttonLED, OUTPUT);
  pinMode(switchLED, OUTPUT);
}

void loop()
{
  // This code will read the positions of the button and switch,
  // then use the "if" command to make LEDs follow these states.
  
  // Create variables to store the button and switch input values:

  int buttonState;
  int switchState;

  // Read and save the states of the button and switch:

  buttonState = digitalRead(buttonPin);
  switchState = digitalRead(switchPin);

  // The if-else statement lets you do different things based on different inputs:

  // The button will read as LOW when it's pressed

  if (buttonState == LOW) // Check to see if buttonState is LOW (pressed) 
  {
    digitalWrite(buttonLED,HIGH); // If buttonState is LOW (pressed), turn on the LED
  }
  else
  {
    digitalWrite(buttonLED,LOW); // If buttonState is HIGH (unpressed), turn off the LED
  }

  if (switchState == LOW) // Check to see if switchState is LOW (switch is on)
  {
    digitalWrite(switchLED,HIGH); // If switchState is LOW (on), turn on the LED
  }
  else
  {
    digitalWrite(switchLED,LOW); // If switchState is HIGH (off), turn off the LED
  }
}

