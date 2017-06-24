// MFSegments.cpp
//
// Copyright (C) 2013-2014

#include "MFLCDDisplay.h"

MFLCDDisplay::MFLCDDisplay()
{
  _initialized = false;
}

void MFLCDDisplay::display(char *string)
{
  if (!_initialized) return;
  char readBuffer[21] = "";
  for(byte l=0;l!=_lines;l++) {
    _lcdDisplay->setCursor(0, l);
    memcpy(readBuffer, string+_cols*l, _cols);
    _lcdDisplay->print(readBuffer);
  }
}

void MFLCDDisplay::attach(byte address, byte cols, byte lines)
{
  _address = address;
  _cols = cols;
  _lines = lines;
  _lcdDisplay = new LiquidCrystal_I2C( (uint8_t)address, (uint8_t)cols, (uint8_t)lines );
  _initialized = true;
  _lcdDisplay->begin();
  _lcdDisplay->backlight();
  test();
}

void MFLCDDisplay::detach()
{
  if (!_initialized) return;
  delete _lcdDisplay;
  _initialized = false;
}

void MFLCDDisplay::powerSavingMode(bool state)
{
  if (state)
    _lcdDisplay->noBacklight();
  else
    _lcdDisplay->backlight();
}

void MFLCDDisplay::test() {
  if (!_initialized) return;
  _lcdDisplay->print("   Mobiflight   ");
  _lcdDisplay->setCursor(0, 1);
  _lcdDisplay->print("     Rocks!     ");
  _lcdDisplay->setCursor(0, 0);
}