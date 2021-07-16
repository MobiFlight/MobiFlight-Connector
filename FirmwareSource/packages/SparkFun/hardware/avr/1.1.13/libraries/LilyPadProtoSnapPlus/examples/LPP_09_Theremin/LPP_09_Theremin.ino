/*
LilyPad ProtoSnap Plus Activity 9: Theremin
SparkFun Electronics
https://www.sparkfun.com/products/14346

A Theremin is an electronic musical instrument that is played by
moving your hands over it. In this activity we'll create a Theremin
using the light sensor and the buzzer.

Follow the tutorial at: https://learn.sparkfun.com/tutorials/lilypad-protosnap-plus-activity-guide#9-theremin-project

This code is released under the MIT License (http://opensource.org/licenses/MIT)

******************************************************************************/

// Create variables for the pins we'll be using

int sensorPin = A2;
int buttonPin = A4;
int buzzer = A3;
int bargraphLED[6] = {15,16,17,18,19,20};

// Set the highest and lowest frequencies
// (Change these and see what happens)

int highestFrequency = 1047; // C6
int lowestFrequency = 523; // C5

void setup()
{
  int x;

  // Initialize the pins we'll be using
  
  pinMode(sensorPin, INPUT);
  pinMode(buzzer, OUTPUT);
  pinMode(buttonPin, INPUT_PULLUP);
  
  for (x = 0; x <= 5; x++)
  {
    pinMode(bargraphLED[x],OUTPUT);
  }

  // Initialize the serial monitor

  Serial.begin(9600);
}

void loop()
{
  int sensorValue;
  int frequency;

  // Read the sensor value (will be 0 to 255):

  sensorValue = analogRead(sensorPin);

  // Print out the sensor reading:

  Serial.print("sensor value: ");
  Serial.println(sensorValue);

  // Display the sensor reading on the bar graph LEDs:

  barGraph(sensorValue);

  // Play a tone based on the light level:

  // The light sensor will return a value from 0 to 1023,
  // but we want to map this to a specific range of frequencies.
  // We'll use a built-in fuction called "map" that transforms one range
  // of values (0 to 1023) to another (lowestFrequency to highestFrequency):

  frequency = map(sensorValue,0,1023,lowestFrequency,highestFrequency);

  if (digitalRead(buttonPin) == LOW) // If the button is pressed:
  {
    tone(buzzer,frequency);
  }
  else
  {
    noTone(buzzer);
  }
}

void barGraph(int value)
{
  // Create a LED bargraph using value as an input.
  // Value should be in the range 0 to 1023.

  int x;
  
  // Step through the bargraph LEDs,
  // Turn them on or off depending on value.

  // Value will be in the range 0 to 1023.
  // There are 6 LEDs in the bargraph.
  // 1023 divided by 6 is 170, so 170 will be our threshold
  // between each LED (0,42,84, etc.)

  for (x=0; x <= 5; x++)
  {
    if (value > (x*170) )
    {
      digitalWrite(bargraphLED[x], HIGH);
    }
    else
    {
      digitalWrite(bargraphLED[x], LOW);
    }    
  }
}

