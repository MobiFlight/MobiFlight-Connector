/* Qduino RGB LED Example
  
   This example controls the user RGB LED near the top
   of the board.
   
   Possible colors for setRGB(color) function:
   
   red
   orange
   yellow
   green
   cyan
   blue
   purple
   pink
   white
  
   created on 30 Jun 15
   made by Quin Etnyre
*/

#include "Qduino.h"
#include "Wire.h"

qduino q;  // initialize the library

void setup() {
  
  q.setup();

}

void loop() {
  
  q.setRGB(100, 150, 200);  // r, g, b values
  
  delay(500);
  
  q.setRGB("red");
  
  delay(500);
  
  q.setRGB("orange");
  
  delay(500);
  
  q.setRGB("yellow");

  delay(500);
  
  q.setRGB("green");
  
  delay(500);
  
  q.setRGB("cyan");
  
  delay(500);
  
  q.setRGB("blue");
  
  delay(500);
  
  q.setRGB("purple");
  
  delay(500);
  
  q.setRGB("pink");
  
  delay(500);
  
  q.setRGB("white");
  
  delay(500);
  
  q.rainbow(3);  // number between 1&5
  
  q.ledOff();
  
  delay(500);
  
}
