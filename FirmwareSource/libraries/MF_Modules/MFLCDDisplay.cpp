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
  for (byte line=0;line!=_lines;line++) {
	_lcdDisplay->setCursor(0, line);
	_lcdDisplay->print(&string[_cols*line]);
  }
}

void MFLCDDisplay::attach(byte address, byte cols, byte lines)
{
  _address = address;
  _cols = cols;
  _lines = lines;
  _lcdDisplay = new LiquidCrystal_I2C( (uint8_t)0x27, (uint8_t)cols, (uint8_t)lines );
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
  _lcdDisplay->backlight();
}

void MFLCDDisplay::test() {
  if (!_initialized) return;
  _lcdDisplay->print("   Mobiflight   ");
  _lcdDisplay->setCursor(0, 1);
  _lcdDisplay->print("     Rocks!     ");
  _lcdDisplay->setCursor(0, 0);
}