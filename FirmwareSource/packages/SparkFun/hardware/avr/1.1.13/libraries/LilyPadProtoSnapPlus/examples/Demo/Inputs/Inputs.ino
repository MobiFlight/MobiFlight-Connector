/*
  LilyPad ProtoSnap Plus
  Inputs.ino
  SparkFun Electronics
  https://www.sparkfun.com/products/14346

  This example prints the state of each input to the serial monitor.

******************************************************************************/

//Inputs
#define BTNPIN 4    // Button (configure INPUT_PULLUP)
#define SWPIN 9     // Switch (configure INPUT_PULLUP)
#define LUXPIN A2   // Light sensor


void setup() {
  //Configure inputs
  pinMode(BTNPIN, INPUT_PULLUP);
  pinMode(SWPIN, INPUT_PULLUP);
  pinMode(LUXPIN, INPUT);

  //Configure serial port
  Serial.begin(9600);
  delay(3000); //wait 3 seconds so the user can get the serial port open.
  Serial.println("Sketch started.");

}

void loop() {
  //Print button state
  Serial.print("Button: ");
  if (digitalRead(BTNPIN) == LOW) {
    Serial.println("Pressed");
  } else {
    Serial.println("Not pressed");
  }

  //Print switch state
  Serial.print("Switch: ");
  if (digitalRead(SWPIN) == LOW) {
    Serial.println("On");
  } else {
    Serial.println("Off");
  }

  //Print light sensor state
  Serial.print("Light Sensor Value: ");
  Serial.println(analogRead(LUXPIN));

  //Print a spare line and wait
  Serial.println();
  delay(1000);
}