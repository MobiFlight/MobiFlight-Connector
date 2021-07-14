/* Qduino RGB LED Example
  
   This example controls the user RGB LED near the top
   of the board.
   
   Possible colors for setRGB(color) function:
   
   RED
   ORANGE
   YELLOW
   GREEN
   CYAN
   BLUE
   PURPLE
   PINK
   WHITE
  
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
  
  q.setRGB(RED);
  
  delay(500);
  
  q.setRGB(ORANGE);
  
  delay(500);
  
  q.setRGB(YELLOW);

  delay(500);
  
  q.setRGB(GREEN);
  
  delay(500);
  
  q.setRGB(CYAN);
  
  delay(500);
  
  q.setRGB(BLUE);
  
  delay(500);
  
  q.setRGB(PURPLE);
  
  delay(500);
  
  q.setRGB(PINK);
  
  delay(500);
  
  q.setRGB(WHITE);
  
  delay(500);
  
  q.rainbow(3);  // number between 1&5
  
  q.ledOff();
  
  delay(500);
}
