/*
 * EEPROMVar 
 *
 * Demonstrates the usage of EEPROMvar. A c++ OOP approach 
 * where a variable can store and restore itself from EEPROM
 * This example code is in the public domain.
 */

#include <EEPROMex.h>
#include <EEPROMvar.h>

// start reading from the first byte (address 120) of the EEPROM

const int maxAllowedWrites = 20;
const int memBase          = 120;

void readAndWriteVar(EEPROMVar<float> &floatVar) { 
    Serial.println("----------------------------------------------");     
    Serial.println("writing and retreiving EEPROMVar of type float");     
    Serial.println("----------------------------------------------");  

    floatVar = 10.5;    // EEPROMVar supports = operator
    floatVar++;         // EEPROMVar supports ++ operator
    floatVar+=8.5;      // EEPROMVar supports += operator
    floatVar/=2;        // EEPROMVar supports /= operator
    
    float input = floatVar;
    floatVar.save();     // store EEPROMVar to EEPROM
    
    floatVar = 0.0;      // reset 
    floatVar.restore();  // restore EEPROMVar to EEPROM
    
    Serial.print("adress: ");
    Serial.println(floatVar.getAddress());
    Serial.print("input: ");
    Serial.println(input);
    Serial.print("output: ");
    Serial.println(floatVar);
    Serial.println();
}
     
void setup()
{
  Serial.begin(9600); 
  while (!Serial) {
    ; // wait for serial port to connect. Needed for Leonardo only
  }

  delay(100);
  Serial.println();  
  
  // start reading from position memBase (address 0) of the EEPROM. Set maximumSize to EEPROMSizeUno 
  // Writes before membase or beyond EEPROMSizeUno will only give errors when _EEPROMEX_DEBUG is set
  EEPROM.setMemPool(memBase, EEPROMSizeUno);  
    
  // Set maximum allowed writes to maxAllowedWrites. 
  // More writes will only give errors when _EEPROMEX_DEBUG is set
  EEPROM.setMaxAllowedWrites(maxAllowedWrites);
  
  // Create Eeprom variables first and in the same order
  EEPROMVar<float> eepromFloat(5.5);  // initial value 5.5
  
  readAndWriteVar(eepromFloat);
}

void loop()
{
  // Nothing to do during loop
}
    
