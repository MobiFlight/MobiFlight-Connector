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

//Includes
#include "timeKeeper.h"

#define MAXTIME 60000

//**********************************************************************//
//
//  Time Keeper
//    Keeps time in number of milliseconds
//
//**********************************************************************//
TimeKeeper::TimeKeeper( void )
{
    timeElapsed = 0;

}

void TimeKeeper::mClear( void )
{
    timeElapsed = 0;

}

uint16_t TimeKeeper::mGet( void )
{
    return timeElapsed;
}

void TimeKeeper::mIncrement( uint8_t var_Increment )
{
    //clamp it.
	if( timeElapsed < MAXTIME )
	{
		timeElapsed += var_Increment;
	}
}
