// MFCore.cpp
//
// Copyright (C) 2013-2014

#include "MFCore.h"
#include <CmdMessenger.h>  // CmdMessenger

MFCore::MFCore() 
{
}
void MFCore::test()
{
  CmdMessenger cmdMessenger = CmdMessenger(Serial);
  cmdMessenger.attach(MFCore::CallbackFunction);
}
void MFCore::CallbackFunction()
{
}
