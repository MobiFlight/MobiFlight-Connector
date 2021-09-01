// MFInputShifter.h

#ifndef MFInputShifter_h
#define MFInputShifter_h

#if ARDUINO >= 100
#include <Arduino.h>
#else
#include <WProgram.h>
#include <wiring.h>
#endif

// Maximum number of shifters allowed on an individual chain. While this is currently set to 4
// there is no technical limit in the code for how many can be chained. It is constrained only
// by available memory (one byte required per chip) and the time it takes to read all the bits in.
#define MAX_CHAINED_INPUT_SHIFTERS 4

extern "C"
{
    typedef void (*inputShifterEvent)(byte, uint8_t, const char *);
};

enum
{
    inputShifterOnPress,
    inputShifterOnRelease,
};

/////////////////////////////////////////////////////////////////////
/// \class MFInputShifter MFInputShifter.h <MFInputShifter.h>
class MFInputShifter
{
public:
    MFInputShifter(const char *name = "InputShifter");
    void attach(uint8_t latchPin, uint8_t clockPin, uint8_t dataPin, uint8_t moduleCount);
    void detach();
    void clear();
    void update();
    void attachHandler(byte eventId, inputShifterEvent newHandler);

private:
    const char *_name;
    uint8_t _latchPin;    // SH/~LD (latch) pin
    uint8_t _clockPin;    // CLK (clock) pin
    uint8_t _dataPin;     // SDO (data) pin
    uint8_t _moduleCount; // Number of 8 bit modules in series.
    bool _initialized = false;
    uint32_t _last;
    uint8_t _lastState[MAX_CHAINED_INPUT_SHIFTERS];

    void detectChanges(uint8_t lastState, uint8_t currentState, uint8_t module);
    void trigger(uint8_t pin, bool state);
    void clearLastState();
    inputShifterEvent _handlerList[2];
};
#endif
