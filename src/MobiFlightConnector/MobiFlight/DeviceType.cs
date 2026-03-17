namespace MobiFlight
{
    public enum DeviceType
    {
        NotSet,              // 0 
        Button,              // 1
        EncoderSingleDetent, // 2 (retained for backwards compatibility, use Encoder for new configs)
        Output,              // 3
        LedModuleDeprecated, // 4
        StepperDeprecatedV1, // 5
        Servo,               // 6
        LcdDisplay,          // 7
        Encoder,             // 8
        StepperDeprecatedV2, // 9
        ShiftRegister,       // 10
        AnalogInput,         // 11
        InputShiftRegister,  // 12
        MultiplexerDriver,   // 13  Not a proper device, but index required for update events
        InputMultiplexer, 	 // 14
        Stepper,             // 15
        LedModule,           // 16
        CustomDevice         // 17        
    }
}
