# Arduino EEPROMex library 

The EEPROMex library is an extension of the standard Arduino EEPROM library. It extends the functionality of the original Arduino EEPROM library with:

* Reading, writing to basic types. This includes  bytes, longs, ints, floats and doubles.
* Reading, writing to single bits. This helps efficient usage of the limited EEPROM memory.
* Reading, writing of any data format. This can be for example structs, strings, etc.
* Reading, writing of arrays of any format. By storing, for example, arrays of structs one can create a database like structure.
* Update functions. The function similar to write functions, but only update changed bytes. If structures have only changed partly, updating instead of writing can save a lot EEPROM wear and significantly increase speed.
* Basic memory allocation functionality. This is basically a counter of the first unallocated byte, and helps giving unique addresses to variables.
* Enabling write limitation: In theory one can burn out a memory cell in a few minutes: a write/erase cycle takes approximately 4 ms, so writing 100.000 times to a single cell takes 6 1/2 min. Limiting the number of writes during the debug phase helps prevent this.
* Debugging of writing out of memory range.

You can download the library here: 
[http://thijs.elenbaas.net/downloads/?did=3](http://thijs.elenbaas.net/downloads/?did=3)
 
And find detailed explanation and samples of the functionality here: 
[http://thijs.elenbaas.net/2012/07/extended-eeprom-library-for-arduino](http://thijs.elenbaas.net/2012/07/extended-eeprom-library-for-arduino)

### Using different data formats

The aim of the library is to also support other standard data types: it currently implements writing and reading to int, long, float and double.

### For reading:
```
uint8_t read(int address);
bool readBit(int address, byte bit)
uint8_t readByte(int address);
uint16_t readInt(int address);
uint32_t readLong(int address);
float readFloat(int address);
double readDouble(int address);
```

Where address is the starting position in EEPROM, and the return value the value read from EEPROM. 

### For writing:
```
bool write(int address, uint8_t value);
bool writeByte(int address, uint8_t value);
bool writeInt(int address, uint16_t value);
bool writeLong(int address, uint32_t value);
bool writeFloat(int address, float value);
bool writeDouble(int address, double value);
```
The update functions are different from the write functions: they will check per byte if the current value differs and only update the the cell with a different value. This will not only reduce wear, and can also significantly reduce write time.

```
bool update(int address, uint8_t value);
bool updateByte(int address, uint8_t value);
bool updateInt(int address, uint16_t value);
bool updateLong(int address, uint32_t value);
bool updateFloat(int address, float value);
bool updateDouble(int address, double);
```

### Manipulating Single bits

The following functions implements reading and writing single bits:

```
bool readBit(int address, byte bit)
```
Where bit is the write position in the byte, ranging from [0..7], with bit 0 being the right-most.  The return value is the read bit.

```
bool writeBit(int address, uint8_t bit, bool value)
bool updateBit(int address, uint8_t bit, bool value)
```

### Data blocks

Using the block functions any data can be read, written and updated:

```
int readBlock(int address, const T& value)
int writeBlock(int address, const T& value)
int updateBlock(int address, const T& value)
```

where T is the type of the data to read/write/update. This can be a basic type, but also a more complex type like a struct. The return value gives the number of bytes that have been read, written or updated.

One can also read/write arrays of data-blocks:

```
int readBlock(int address, const T[]; value, int items)
int writeBlock(int address, const T[]; value, int items)
int updateBlock(int address, const T[]; value, int items)
```

### Debugging EEPROM applications 

It is easy to burn out a memory cell in few minutes, so during debugging it would be very useful to limit the number of allowed writes. It is easy to put a bracket at the wrong location, and placing an EEPROM write inside of a loop, rather than outside, and introduce extensive writing causing wear.  The following function helps limit the number of writes.

```
setMaxAllowedWrites(int allowedWrites);
```

More writes than allowed will be refused and result in an error message. You can also set the address range used by the library:

```
setMemPool(int base, int memSize);
```

The lower value is used by the getAddress function, the upper value is  used for setting the EEPROM size. Writing outside the maximum size will result in an error message. The following EEPROM sizes are predefined

Based on processor:
```
*EEPROMSizeATmega168
*EEPROMSizeATmega328
*EEPROMSizeATmega1280
```
Based on board:
```
*EEPROMSizeUno
*EEPROMSizeUnoSMD
*EEPROMSizeLilypad
*EEPROMSizeDuemilanove
*EEPROMSizeMega
*EEPROMSizeDiecimila
*EEPROMSizeNano
```

### EEPROM performance

All of the read/write functions make sure the EEPROM is ready to be accessed. Since this may cause a delay of max 4ms, time-critical  applications should first poll the EEPROM e. g. using the isReady function before attempting any actual I/O:

```
bool isReady();
```
