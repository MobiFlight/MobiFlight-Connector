#ifndef MFBoardUno_h
#define MFBoardUno_h

#define MF_SEGMENT_SUPPORT 1
#define MF_LCD_SUPPORT 1
#define MF_STEPPER_SUPPORT 1
#define MF_SERVO_SUPPORT 1
#define MF_MCP_SUPPORT 1
#define MF_JOYSTICK_SUPPORT 0
#define MF_ANALOG_SUPPORT 0
#define MF_SHIFTER_SUPPORT 0
// ALL          24602 (76.3%) / 1551 (75.7%)
// No Segments  22860 (70.9%) / 1418 (69.2%)
// No LCDs      23072 (71.5%) / 1516 (74.0%)
// NO Steppers  19586 (60.7%) / 1513 (73.9%)
// No Servos    23014 (71.3%) / 1458 (71.2%)
// No MCP23017  22792 (70.7%) / 1544 (75.4%)

#define MODULE_MAX_PINS 13
#define MAX_OUTPUTS 8
#define MAX_BUTTONS 8
#define MAX_LEDSEGMENTS 1
#define MAX_ENCODERS 2
#define MAX_STEPPERS 2
#define MAX_MFSERVOS 2
#define MAX_MFLCD_I2C 2
#define MAX_ANALOG_INPUTS 0
#define MAX_SHIFTERS 0
#define MAX_MCP_EXPANDER 1

#define STEPS 64
#define STEPPER_SPEED 400 // 300 already worked, 467, too?
#define STEPPER_ACCEL 800

#define NATIVE_MAX_PINS 19 // max Pins from module, w/o Port Expander
#define MCP_PIN_BASE 100   // first Pin number from Port Expander
#define MODULE_MAX_PINS NATIVE_MAX_PINS + ((MAX_MCP_EXPANDER * 16) * MF_MCP_SUPPORT)

#if MF_MCP_SUPPORT == 1
#include <MFMCP23017.h>
extern MFMCP23017 mcp_expander[];
#endif

#define MOBIFLIGHT_TYPE "MobiFlight Uno"
#define MOBIFLIGHT_SERIAL "0987654321"
#define MEMLEN_NAME "MobiFlight Uno"
#define EEPROM_SIZE 1024 // EEPROMSizeUno
#define MEMLEN_CONFIG 256

#endif