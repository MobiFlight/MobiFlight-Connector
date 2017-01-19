#include <EEPROMex.h>

/*
  EEPROMvar.h - EEPROM variable library
  Copyright (c) 2012 Thijs Elenbaas.  All right reserved.
  
  based on class by AlphaBeta

  This library is free software; you can redistribute it and/or
  modify it under the terms of the GNU Lesser General Public
  License as published by the Free Software Foundation; either
  version 2.1 of the License, or (at your option) any later version.

  This library is distributed in the hope that it will be useful,
  but WITHOUT ANY WARRANTY; without even the implied warranty of
  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
  Lesser General Public License for more details.

  You should have received a copy of the GNU Lesser General Public
  License along with this library; if not, write to the Free Software
  Foundation, Inc., 51 Franklin St, Fifth Floor, Boston, MA  02110-1301  USA
*/

template<typename T> class EEPROMVar 
{
	public:
	  EEPROMVar(T init) {
		address = EEPROM.getAddress(sizeof(T));	
		var = init;
	  }
	  operator T () { 
		return var; 
	  }
	  EEPROMVar &operator=(T val) {
		var = val;
		return *this;
	  }
	  
	  void operator+=(T val) {
		var += T(val); 
	  }
	  void operator-=(T val) {
		var -= T(val); 
	  }	 	  
	  void operator++(int) {
		var += T(1); 
	  }
	  void operator--(int) {
		var -= T(1); 
	  }
	  void operator++() {
		var += T(1); 
	  }
	  void operator--() {
		var -= T(1); 
	  }
	  template<typename V>
		void operator /= (V divisor) {
		var = var / divisor;
	  }
	  template<typename V>
		void operator *= (V multiplicator) {
		var = var * multiplicator;
	  }
	  void save(){	   	   
	    EEPROM.writeBlock<T>(address, var);
	  }
	  
	  void update(){	   	   
	    EEPROM.updateBlock<T>(address, var);
	  }
	  
	  int getAddress(){	   	   
	    return address;
	  }
	  
	  void restore(){
	  	EEPROM.readBlock<T>(address, var);
	  }
	protected:	
	  T var;
	  int address;
};