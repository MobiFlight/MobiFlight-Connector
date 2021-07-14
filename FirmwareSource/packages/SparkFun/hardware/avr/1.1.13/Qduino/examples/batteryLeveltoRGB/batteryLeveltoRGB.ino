/* Qduino Fuel Gauge / RGB LED

   This sketch demonstrates both the RGB LED and the
   battery fuel gauge -> the RGB LED is green when 75%
   or more full, yellow 50% - 74%, orange 25% - 49%, red
   when battery is less than 25% full.

   Fuel Gauge Drivers forked from Matt Newberry
   
   created on 25 May 15
   made by Quin Etnyre
*/

#include "Arduino.h"
#include "Wire.h"
#include "Qduino.h"

fuelGauge battery;  // initialize the library

qduino q;

void setup(){
  
 Wire.begin(); 
 battery.setup();
 
 q.setup();
 
}

void loop(){
  int charge = battery.chargePercentage();
  battery.reset();
  
  if(charge >= 75) {
    
    q.setRGB("green");
    
  } else if(charge >= 50 && charge < 75) {
    
    q.setRGB("yellow");
    
  } else if(charge >= 25 && charge < 50) {
    
    q.setRGB("orange");
    
  } else if(charge < 25) {
    
    q.setRGB("red");
  }
  
  delay(1000);
  
  q.ledOff();
}
