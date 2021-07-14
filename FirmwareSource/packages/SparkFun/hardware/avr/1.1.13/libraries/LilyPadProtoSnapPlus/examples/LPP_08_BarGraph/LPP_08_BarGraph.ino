/*
LilyPad ProtoSnap Plus Activity 8: LED Bar Graph
SparkFun Electronics
https://www.sparkfun.com/products/14346

Play with the six LEDs on the LilyPad ProtoSnap Plus

Follow the tutorial at: https://learn.sparkfun.com/tutorials/lilypad-protosnap-plus-activity-guide#8-led-bar-graph

This code is released under the MIT License (http://opensource.org/licenses/MIT)

******************************************************************************/

// Create a variable for the light sensor input:

int sensorPin = A2;

// The six white LEDs on the LilyPad USB Plus are numbered 15 through 20.
// To make them easier to use, we'll put those numbers into an array.
// The initial [6] defines the size of the array (six elements).
// We're filling the array with predefined values, but you could do this
// in your code as well.

int bargraphLED[6] = {15,16,17,18,19,20};

// The array is indexed from 0 to 5; for example bargraphLED[2] = 17

void setup()
{
  int x;

  // Initialize the sensor pin as an input, but without a pullup
  // (Pullups are only used for switch inputs)
  
  pinMode(sensorPin, INPUT);

  // Initialize the bargraph LED pins as outputs
  // We'll use the matrix we defined above,
  // where the LEDs are indexed from 0 to 5

  for (x = 0; x <= 5; x++)
  {
    pinMode(bargraphLED[x], OUTPUT);
  }

  // Initialize the serial monitor

  Serial.begin(9600);
}

void loop()
{
  int sensorValue;

  // Read the sensor value (will be 0 to 1023):

  sensorValue = analogRead(sensorPin);

  // Print out the sensor reading:

  Serial.print("sensor value: ");
  Serial.println(sensorValue);

  // Display the sensor reading on the bar graph LEDs.
  // This is a new function that we created ourselves (see below).

  barGraph(sensorValue);
}

// Here we're making our own command called barGraph:
// The first "void" means we don't return anything from this command
// The "int value" is what we'll pass to the command (it must be an integer,
// and it will be called "value" in the command.

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

