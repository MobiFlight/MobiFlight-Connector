/*
  LilyPad ProtoSnap Plus
  Template.ino
  SparkFun Electronics
  https://www.sparkfun.com/products/14346

  This example gives names to each pin of the ProtoSnap Plus and configures the
  serial interface.

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

//Inputs
#define BTNPIN 4    // Button (configure INPUT_PULLUP)
#define SWPIN 9     // Switch (configure INPUT_PULLUP)
#define LUXPIN A2   // Light sensor

//Buzzer
#define BZRPIN A3	// Buzzer

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
  //
}