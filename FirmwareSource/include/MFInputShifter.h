// MFInputShifter.h

#ifndef MFInputShifter_h
#define MFInputShifter_h

#if ARDUINO >= 100
#include <Arduino.h>
#else
#include <WProgram.h>
#include <wiring.h>
#endif

#define MAX_CHAINED_INPUT_SHIFTERS 4

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
    void attachHandler(byte eventId, inputShifterEvent newHandler);

private:
    const char *_name;
    uint8_t _latchPin;    // Latch pin
    uint8_t _clockPin;    // Clock pin
    uint8_t _dataPin;     // Data/SI pin
    uint8_t _moduleCount; // Number of 8 bit modules in series. For a shift register with 16 bit one needs to select 2 modules a 8......
    bool _initialized = false;
    uint32_t _last;
    uint8_t _lastState[MAX_CHAINED_INPUT_SHIFTERS]; // Max 4 chained modules for now = 32 bits

    void detectChanges(uint8_t lastState, uint8_t currentState, uint8_t module);
    void trigger(uint8_t pin, bool state);
    void clearLastState();
    inputShifterEvent _handlerList[2];
};
#endif
