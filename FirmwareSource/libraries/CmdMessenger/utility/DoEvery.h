#ifndef DoEvery_h
#define DoEvery_h
#define LIBRARY_VERSION	1.0.0

#include <inttypes.h>

#if ARDUINO >= 100
#include <Arduino.h> 
#else
#include <WProgram.h> 
#endif


class DoEvery
{
public:
	DoEvery(long);
	void reset();
	bool check();
	bool before(double);
private:
	unsigned long period;
	unsigned long lastTime;
};

#endif