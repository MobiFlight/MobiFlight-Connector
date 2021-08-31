// MFInputShifter.h

#ifndef MFInputShifter_h
#define MFInputShifter_h

#if ARDUINO >= 100
#include <Arduino.h>
#else
#include <WProgram.h>
#include <wiring.h>
#endif

/////////////////////////////////////////////////////////////////////
/// \class MFInputShifter MFInputShifter.h <MFInputShifter.h>
class MFInputShifter
{
public:
    MFInputShifter();
    void attach(uint8_t latchPin, uint8_t clockPin, uint8_t dataPin, uint8_t moduleCount);
    void trigger();
    void detach();
    void test();
    void clear();
    void update();

private:
    uint8_t _latchPin;    // Latch pin
    uint8_t _clockPin;    // Clock pin
    uint8_t _dataPin;     // Data/SI pin
    uint8_t _moduleCount; // Number of 8 bit modules in series. For a shift register with 16 bit one needs to select 2 modules a 8......
    bool _initialized = false;
    uint32_t _last;
    unsigned long _lastState; // Max 4 modules for now = 32 bit
};
#endif
