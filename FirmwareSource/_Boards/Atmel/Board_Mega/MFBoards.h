#ifndef MFBoardMega_h
#define MFBoardMega_h

#define MF_SEGMENT_SUPPORT 1
#define MF_LCD_SUPPORT 1
#define MF_STEPPER_SUPPORT 1
#define MF_SERVO_SUPPORT 1
#define MF_ANALOG_SUPPORT 1
#define MF_SHIFTER_SUPPORT 1
#define MF_INPUT_SHIFTER_SUPPORT 1
// ALL          26544 (10.5%) / 4247 (51.8%)
// No Segments  24784 (9.8%) / 4102 (50.1%)
// No LCDs      25030 (9.9%) / 4212 (51.4%)
// NO Steppers  21398 (8.4%) / 4177 (51.0%)
// No Servos    24538 (9.7%) / 3827 (46.7%)

#define MODULE_MAX_PINS 69
#define MAX_OUTPUTS 40
#define MAX_BUTTONS 67 // 69 max. numbering, 0/1 for serial
#define MAX_LEDSEGMENTS 4
#define MAX_ENCODERS 20
#define MAX_STEPPERS 10
#define MAX_MFSERVOS 10
#define MAX_MFLCD_I2C 2
#define MAX_ANALOG_INPUTS 5
#define MAX_SHIFTERS 10
#define MAX_INPUT_SHIFTERS 10

#define STEPS 64
#define STEPPER_SPEED 400 // 300 already worked, 467, too?
#define STEPPER_ACCEL 800

#define MOBIFLIGHT_TYPE "MobiFlight Mega"
#define MOBIFLIGHT_SERIAL "1234567890"
#define MEMLEN_NAME "MobiFlight Mega"
#define EEPROM_SIZE 4096 // EEPROMSizeMega
#define MEMLEN_CONFIG 1024

#endif
