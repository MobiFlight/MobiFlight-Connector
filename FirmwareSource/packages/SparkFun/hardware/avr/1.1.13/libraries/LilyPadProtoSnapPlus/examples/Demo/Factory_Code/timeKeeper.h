//**********************************************************************//
//
//  MIT License: http://opensource.org/licenses/MIT
//
//  Written by:  Marshall Taylor
//  Changelog (YYYY/MM/DD):
//    2015/6/26: Created
//    2015/10/08: Initial Release
//
//**********************************************************************//
#ifndef TIMEKEEPER_H_INCLUDED
#define TIMEKEEPER_H_INCLUDED

#include "stdint.h"

//Classes
class TimeKeeper
{
public:
    TimeKeeper( void );
    void mClear( void );
    uint16_t mGet( void );
    void mIncrement( uint8_t );

private:
    uint16_t timeElapsed;

};


#endif // FILE_H_INCLUDED

