/*
LilyPad ProtoSnap Plus Activity 7: Sensing Light
SparkFun Electronics
https://www.sparkfun.com/products/14346

Explore analog input from the light sensor

Follow the tutorial at: https://learn.sparkfun.com/tutorials/lilypad-protosnap-plus-activity-guide#7-sensing-light

This code is released under the MIT License (http://opensource.org/licenses/MIT)

******************************************************************************/

// Create variables for the pins we'll use:

int sensorPin = A2;

int redLED = 6;
int greenLED = A7;
int blueLED = A8;

void setup()
{
  // Initialize the sensor pin as an input, but without a pullup
  // (Pullups are only used for switch inputs)
  
  pinMode(sensorPin, INPUT);

  // Initialize the output pins:
  
  pinMode(redLED, OUTPUT);
  pinMode(greenLED, OUTPUT);
  pinMode(blueLED, OUTPUT);
 
  // Initialize the serial monitor:

  Serial.begin(9600);
}

void loop()
{
  int sensorValue;

  // Read the sensor value (will be 0 to 1023):

  sensorValue = analogRead(sensorPin);

  // Print out the sensor reading to the serial monitor:

  Serial.print("sensor value: ");
  Serial.println(sensorValue);

  // Since the sensor value is 0 to 1023,
  // and analogWrite needs a value from 0 to 255,
  // we'll divide the sensor value by four to scale it down:

  analogWrite(redLED,sensorValue / 4);
  analogWrite(greenLED,sensorValue / 4);
  analogWrite(blueLED,sensorValue / 4);
}

