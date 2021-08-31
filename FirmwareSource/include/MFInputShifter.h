// MFInputShifter.h

#ifndef MFInputShifter_h
#define MFInputShifter_h

#if ARDUINO >= 100
#include <Arduino.h>
#else
#include <WProgram.h>
#include <wiring.h>
#endif

extern "C"
{
    typedef void (*inputShifterEvent)(byte, uint8_t, const char *);
};

enum
{
    shifterOnPress,
    shifterOnRelease,
};

/////////////////////////////////////////////////////////////////////
/// \class MFInputShifter MFInputShifter.h <MFInputShifter.h>
class MFInputShifter
{
public:
    MFInputShifter(const char *name = "InputShifter");
    void attach(uint8_t latchPin, uint8_t clockPin, uint8_t dataPin, uint8_t moduleCount);
    void detach();
    void test();
    void clear();
    void update();
    void attachHandler(uint8_t pin, byte eventId, inputShifterEvent newHandler);

private:
    const char *_name;
    uint8_t _latchPin;    // Latch pin
    uint8_t _clockPin;    // Clock pin
    uint8_t _dataPin;     // Data/SI pin
    uint8_t _moduleCount; // Number of 8 bit modules in series. For a shift register with 16 bit one needs to select 2 modules a 8......
    bool _initialized = false;
    uint32_t _last;
    uint8_t _lastState; // Max 1 modules for now = 8 bits

    void detectChanges(uint8_t lastState, uint8_t currentState);
    void trigger(uint8_t pin, bool state);
    inputShifterEvent _handlerList[8][2];
};
#endif
