using MobiFlight.Base;
using MobiFlight.CustomDevices;
using MobiFlight.Firmware;
using MobiFlight.UI.Panels.Settings.Device;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight
{
    public static class DeviceReferenceFactory
    {
        public static List<ControllerDevice> Create(BaseDevice dev, Board board = null)
        {
            var result = new List<ControllerDevice>();
            switch (dev.Type)
            {
                case DeviceType.Button:
                case DeviceType.Encoder:
                case DeviceType.AnalogInput:
                case DeviceType.Servo:
                    result.Add(CreateDevice(dev.Name, dev.Type));
                    break;

                case DeviceType.Output:
                    result.Add(CreateDevice(dev.Name, dev.Type, OutputCapability(dev as Output, board)));
                    break;

                case DeviceType.LcdDisplay:
                    result.Add(CreateDevice(dev.Name, dev.Type, LcdCapability(dev as LcdDisplay)));
                    break;

                case DeviceType.Stepper:
                    result.Add(CreateDevice(dev.Name, dev.Type, StepperCapability(dev as Stepper)));
                    break;

                case DeviceType.CustomDevice:
                    result.Add(CreateDevice(dev.Name, dev.Type, CustomDeviceCapability(dev as Firmware.CustomDevice)));
                    break;

                case DeviceType.LedModule:
                    var led = dev as LedModule;
                    int ledCount = int.TryParse(led?.NumModules, out int nl) ? nl : 1;
                    var ledCapability = new DeviceCapability { Digits = LedDigits(led) };
                    for (int i = 1; i <= ledCount; i++)
                        result.Add(CreateDevice($"{dev.Name}:{i}", DeviceType.LedModule, ledCapability));
                    break;

                case DeviceType.InputMultiplexer:
                    var mux = dev as InputMultiplexer;
                    int muxPinCount = int.TryParse(mux?.NumBytes, out int nb) ? nb * 8 : 16;
                    for (int i = 0; i < muxPinCount; i++)
                        result.Add(CreateDevice($"{dev.Name}:{i}", DeviceType.Button));
                    break;

                case DeviceType.InputShiftRegister:
                    var isr = dev as InputShiftRegister;
                    int isrPinCount = int.TryParse(isr?.NumModules, out int nm) ? nm * 8 : 8;
                    for (int i = 0; i < isrPinCount; i++)
                        result.Add(CreateDevice($"{dev.Name}:{i}", DeviceType.Button));
                    break;

                case DeviceType.ShiftRegister:
                    var sr = dev as ShiftRegister;
                    int srPinCount = int.TryParse(sr?.NumModules, out int numModules) ? numModules * 8 : 8;
                    result.Add(CreateDevice(dev.Name, DeviceType.ShiftRegister, new DeviceCapability { Pins = srPinCount }));
                    break;
            }
            return result;
        }

        /// <summary>Extension board connected to an Arcaze. An Arcaze has at most one type of extension.</summary>
        public enum ArcazeExtension
        {
            None,
            LedDriver,
            DisplayDriver
        }

        private const int ArcazeBasePinCount = 20;
        private const int ArcazeDisplayDigits = 8;
        private static readonly string[] ArcazeBasePorts = { "A", "B" };

        // Each LED driver module provides three ports with 20, 20 and 8 outputs.
        private static readonly (string Port, int PinCount)[][] ArcazeLedDriverModulePorts =
        {
            new[] { ("C", 20), ("D", 20), ("E", 8) },
            new[] { ("F", 20), ("G", 20), ("H", 8) },
            new[] { ("I", 20), ("J", 20), ("K", 8) },
            new[] { ("L", 20), ("M", 20), ("N", 8) },
        };

        private static readonly string[] ArcazeDisplayAddresses =
            { "0", "1", "2", "3", "4", "5", "6", "7", "8", "9", "A", "B", "C", "D", "E", "F" };
        private static readonly int[] ArcazeDisplayConnectors = { 1, 2 };

        /// <summary>
        /// Arcaze boards don't report their devices,
        /// so outputs and displays are created from the configured extension.
        /// Only as many extension modules as configured are created.
        /// </summary>
        public static List<ControllerDevice> CreateForArcaze(ArcazeExtension extension, int numModules)
        {
            var result = new List<ControllerDevice>();

            foreach (var port in ArcazeBasePorts)
                result.AddRange(CreateArcazeOutputs(port, ArcazeBasePinCount));

            switch (extension)
            {
                case ArcazeExtension.LedDriver:
                    var ledDriverPorts = ArcazeLedDriverModulePorts.Take(numModules).SelectMany(module => module);
                    foreach (var (port, pinCount) in ledDriverPorts)
                        result.AddRange(CreateArcazeOutputs(port, pinCount));
                    break;

                case ArcazeExtension.DisplayDriver:
                    var displayCapability = new DeviceCapability { Digits = ArcazeDisplayDigits };
                    foreach (var address in ArcazeDisplayAddresses.Take(numModules))
                        foreach (var connector in ArcazeDisplayConnectors)
                            result.Add(CreateDevice($"{address}:{connector}", DeviceType.LedModule, displayCapability));
                    break;
            }

            return result;
        }

        private static IEnumerable<ControllerDevice> CreateArcazeOutputs(string port, int pinCount)
        {
            // Arcaze pins are stored as port and two digit pin, e.g. "A01"
            return Enumerable.Range(1, pinCount)
                .Select(pin => CreateDevice($"{port}{pin:00}", DeviceType.Output, new DeviceCapability { Pwm = false }));
        }

        private static ControllerDevice CreateDevice(string name, DeviceType type, DeviceCapability capability = null)
        {
            return new ControllerDevice { Name = name, Label = name, Type = type, Capabilities = capability };
        }

        private static DeviceCapability OutputCapability(Output output, Board board)
        {
            var pin = board?.Pins.Find(p => p.Pin.ToString() == output?.Pin);
            return new DeviceCapability { Pwm = pin?.isPWM ?? false };
        }

        private static DeviceCapability LcdCapability(LcdDisplay lcd)
        {
            return new DeviceCapability { Rows = lcd?.Lines, Columns = lcd?.Cols };
        }

        private static int LedDigits(LedModule led) => led?.ModelType switch
        {
            LedModule.MODEL_TYPE_TM1637_4DIGIT => 4,
            LedModule.MODEL_TYPE_TM1637_6DIGIT => 6,
            _ => 8
        };

        private static DeviceCapability StepperCapability(Stepper stepper)
        {
            var profile = MFStepperPanel.Profiles.Find(p => p.Value.id == stepper?.Profile)?.Value;
            return new DeviceCapability { Profile = profile, AutoZero = stepper?.BtnPin != Stepper.AUTOHOME_NONE };
        }

        private static DeviceCapability CustomDeviceCapability(Firmware.CustomDevice device)
        {
            var definition = CustomDeviceDefinitions.GetDeviceByType(device?.CustomType);
            var messageTypes = definition?.MessageTypes
                .Select(m => new CustomDeviceMessageType(m.Id, m.Label, m.Description))
                .ToList();
            return new DeviceCapability { MessageTypes = messageTypes };
        }
    }
}