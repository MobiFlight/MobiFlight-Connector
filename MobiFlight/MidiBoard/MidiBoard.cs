using M;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace MobiFlight
{
    public class MidiBoardNotConnectedException : Exception
    {
        public MidiBoardNotConnectedException(string Message) : base(Message) { }
    }

    public class MidiBoardInUsageByOtherProcessException : Exception
    {
        public MidiBoardInUsageByOtherProcessException(string Message) : base(Message) { }
    }

    public class MidiBoard
    {
        public static string SerialPrefix = "MI-";
        public event ButtonEventHandler OnButtonPressed;        
        public event EventHandler OnDisconnected;

        private MidiOutputDevice MidiOutput = null;
        private MidiInputDevice MidiInput = null;                      
        private Dictionary<string, MidiBoardDevice> Inputs = new Dictionary<string, MidiBoardDevice>();
        private Dictionary<string, MidiBoardOutputDevice> Outputs = new Dictionary<string, MidiBoardOutputDevice>();
        private ConcurrentQueue<MidiMessage> MidiMessageQueue = new ConcurrentQueue<MidiMessage>();
        private Dictionary<string, InputEventArgs> LatestDeviceAnalogInputValue = new Dictionary<string, InputEventArgs>();
        private MidiBoardDefinition Definition;

        // Necessary e.g. for X-Touch Mini. Control of output LEDs depends on the active layer.
        private string ActiveLayer;
        private string name;
        private string serial;
        private bool IsDisconnected = false;

        private int EncoderNeutral = 0;
        private int EncoderRightFast;
        private int EncoderLeftFast;

        public static bool IsMidiBoardSerial(string serial)
        {
            return (serial != null && serial.Contains(SerialPrefix));
        }

        public string Name { 
            get { return this.name; }  
        }

        public string Serial
        {
            get { return SerialPrefix + this.serial; }
        }

        public bool HasMidiOutput
        {
            get { return this.MidiOutput != null; }
        }


        public MidiBoard(MidiInputDevice midiInput, MidiOutputDevice midiOutput, string name, MidiBoardDefinition def)
        {                 
            this.MidiInput = midiInput;
            this.MidiOutput = midiOutput;
            this.name = name;
            // Two boards of the same type get a different postfix to their name in windows
            this.serial = GenerateSerial(name, int.MaxValue).ToString();
            this.Definition = def;
            if (def != null)
            {
                EncoderNeutral = def.EncoderNeutralPosition;
                if (!string.IsNullOrEmpty(def.InitialLayer))
                    ActiveLayer = def.InitialLayer;
            }            
            EncoderRightFast = EncoderNeutral + 3;
            EncoderLeftFast = EncoderNeutral - 3;
    }

        private int GenerateSerial(string s, int maxint)
        {
            // Simple hash algorithm, folding on a string, summed 4 bytes at a time 
            long sum = 0, mul = 1;
            for (int i = 0; i < s.Length; i++)
            {
                mul = (i % 4 == 0) ? 1 : mul * 256;
                sum += (long)s[i] * mul;
            }
            return (int)(Math.Abs(sum) % maxint);
        }

        private void SendInputEvent(InputEventArgs inputEventArgs)
        {
            OnButtonPressed?.Invoke(this, inputEventArgs);
            UpdateRelatedOutputDevices(inputEventArgs.DeviceId);
        }

        private void EnumerateDevices()
        {
            if (Definition != null && Definition.Inputs != null)
            {
                foreach (var inputDef in Definition.Inputs)
                {
                    for (int i = 0; i < inputDef.MessageIds.Length; i++)
                    {
                        string name = inputDef.GetNameWithIndex(i);
                        string label = inputDef.GetLabelWithIndex(i);

                        var inputDevice = new MidiBoardDevice() { Label = label, Name = name, Layer = inputDef.Layer };
                        if (inputDef.InputType == MidiBoardDeviceType.Button)
                            inputDevice.Type = DeviceType.Button;
                        else if (inputDef.InputType == MidiBoardDeviceType.EndlessKnob)
                            inputDevice.Type = DeviceType.Encoder;
                        else
                            inputDevice.Type = DeviceType.AnalogInput;
                        Inputs.Add(inputDevice.Name, inputDevice);
                        Log.Instance.log($"Added Input: {inputDevice.Name} Label: {inputDevice.Label} Type: {inputDevice.Type}.", LogSeverity.Debug);
                    }
                }
            }
        }

        public string MapDeviceNameToLabel(string deviceName)
        {
            if (Inputs.ContainsKey(deviceName))
                return Inputs[deviceName].Label;
            else
                return deviceName;
        }

        public void Connect()
        {
            EnumerateDevices();
            EnumerateOutputDevices();
            MidiInput.Input += MidiBoard_Input;
            try
            {
                MidiInput.Open();
                MidiInput.Start();
                MidiOutput?.Open();
            }
            catch (Exception ex)
            {
                throw new MidiBoardInUsageByOtherProcessException(ex.Message);
            }
        }

        private int ScaleAnalogValue(byte midiValue)
        {
            // Scale from 0..127 to range of 0..1023            
            return (int)Math.Round(8.0551 * midiValue);
        }        

        private void CheckAndExecuteLayerChange(string inputDeviceId)
        {
            // e.g. on X-TOUCH MINI only chance to detect layer change is via new incoming signals
            if (Inputs.ContainsKey(inputDeviceId))
            {
                var inputDevice = Inputs[inputDeviceId];
                if (ActiveLayer != inputDevice.Layer)
                {
                    // Execute layer change and update output LEDs
                    ActiveLayer = inputDevice.Layer;
                    foreach (var kvp in Outputs)
                    {
                        // Necessary because 1 button can relate to 2 different output devices.
                        // Standard LED and LED Blink. Only active output device must be updated.
                        if (kvp.Value.IsActive)
                            UpdateOutputDeviceState(kvp.Key);
                    }
                }
            }
        }

        private InputEventArgs GetDefaultInputEventArgs(MidiMessageType mType, byte channel, byte id = 0)
        {
            var inputEventArgs = new InputEventArgs();
            // Use channel+1 because all miditools show channel starting with 1, technically it starts with 0
            byte adaptedChannel = (byte)(channel + 1);
            inputEventArgs.DeviceId = MidiInputDefinition.GetName(mType, adaptedChannel, id);
            inputEventArgs.Name = Name;
            inputEventArgs.Serial = Serial;
            if (Inputs.ContainsKey(inputEventArgs.DeviceId))
            {
                inputEventArgs.DeviceLabel = Inputs[inputEventArgs.DeviceId].Label;
                inputEventArgs.Type = Inputs[inputEventArgs.DeviceId].Type;
            }
            else
            {
                // Input not found in configuration
                inputEventArgs.DeviceLabel = inputEventArgs.DeviceId;
                inputEventArgs.Type = DeviceType.Button; // default             
            }
            return inputEventArgs;
        }

        private void UpdateRelatedOutputDevices(string inputDeviceId)
        {
            if (Inputs.ContainsKey(inputDeviceId))
            {
                var inputDevice = Inputs[inputDeviceId];              
                foreach (var outputDevice in inputDevice.RelatedOutputDevices)
                {
                    // Necessary because 1 button can relate to 2 different output devices.
                    // Standard LED and LED Blink. Only active must be updated.
                    if (outputDevice.IsActive)
                        UpdateOutputDeviceState(outputDevice.Name);
                }
            }
        }

        private void ProcessNoteMidiMessage(byte channel, byte id, byte velocity, bool isNoteOff)
        { 
            var inputEventArgs = GetDefaultInputEventArgs(MidiMessageType.Note, channel, id);            
            CheckAndExecuteLayerChange(inputEventArgs.DeviceId); 

            // BUTTON
            if (inputEventArgs.Type == DeviceType.Button)
            {
                if (isNoteOff)
                    inputEventArgs.Value = (int)MobiFlightButton.InputEvent.RELEASE;
                else
                    inputEventArgs.Value = (int)MobiFlightButton.InputEvent.PRESS;
                SendInputEvent(inputEventArgs);
            }
            // ANALOG INPUT
            else if (inputEventArgs.Type == DeviceType.AnalogInput)
            {
                if (!isNoteOff)
                {
                    inputEventArgs.Value = ScaleAnalogValue(velocity);
                    LatestDeviceAnalogInputValue[inputEventArgs.DeviceId] = inputEventArgs;
                }
            }
        }

        private void ProcessPitchMidiMessage(byte channel, short pitch)
        {
            // only one available pitch per channel
            var inputEventArgs = GetDefaultInputEventArgs(MidiMessageType.Pitch, channel);
            CheckAndExecuteLayerChange(inputEventArgs.DeviceId);

            // Scale from 0..16383 to range of 0..1023   
            inputEventArgs.Value = (int)Math.Ceiling(pitch / 16.015);
            LatestDeviceAnalogInputValue[inputEventArgs.DeviceId] = inputEventArgs;          
        }


        private int EncoderValueToInputEvent(byte value)
        {        
            int inputEventValue = 0;
            // Handle 0 overflow and convert to signed int. 0 - 1 is 127 for the midi bytes.
            int newValue = (Math.Abs((EncoderNeutral - value)) <= 63) ? value : (value - 128);
            if (newValue >= EncoderRightFast)
            {
                inputEventValue = (int)MobiFlightEncoder.InputEvent.RIGHT_FAST;
            }
            else if (newValue > EncoderNeutral)
            {
                inputEventValue = (int)MobiFlightEncoder.InputEvent.RIGHT;
            }
            else if (newValue <= EncoderLeftFast)   
            {
                inputEventValue = (int)MobiFlightEncoder.InputEvent.LEFT_FAST;
            }
            else if (newValue < EncoderNeutral)  
            {
                inputEventValue = (int)MobiFlightEncoder.InputEvent.LEFT;
            }
            return inputEventValue;
        }

        private void ProcessCCMidiMessage(byte channel, byte id, byte value)
        {
            var inputEventArgs = GetDefaultInputEventArgs(MidiMessageType.CC, channel, id);
            CheckAndExecuteLayerChange(inputEventArgs.DeviceId);

            // BUTTON
            if (inputEventArgs.Type == DeviceType.Button)
            {
                if (value == 0)
                    inputEventArgs.Value = (int)MobiFlightButton.InputEvent.RELEASE;
                else
                    inputEventArgs.Value = (int)MobiFlightButton.InputEvent.PRESS;
                SendInputEvent(inputEventArgs);
            }
            // ANALOG INPUT
            else if (inputEventArgs.Type == DeviceType.AnalogInput)
            {
                inputEventArgs.Value = ScaleAnalogValue(value);
                LatestDeviceAnalogInputValue[inputEventArgs.DeviceId] = inputEventArgs;
            }     
            // ENCODER
            else if (inputEventArgs.Type == DeviceType.Encoder)
            {
                inputEventArgs.Value = EncoderValueToInputEvent(value);
                SendInputEvent(inputEventArgs);
            }            
        }

        private void SendLatestAnalogInputValues()
        {
            foreach (var kvp in LatestDeviceAnalogInputValue)
            {
                SendInputEvent(kvp.Value);
            }
            LatestDeviceAnalogInputValue.Clear();
        }

        public void ProcessMessages()
        {
            while (MidiMessageQueue.TryDequeue(out var message))
            {
                if (message is MidiMessageNoteOn)
                {
                    var m = (MidiMessageNoteOn)message;
                    ProcessNoteMidiMessage(m.Channel, m.NoteId, m.Velocity, isNoteOff: false);
                }
                else if (message is MidiMessageNoteOff)
                {
                    var m = (MidiMessageNoteOff)message;
                    ProcessNoteMidiMessage(m.Channel, m.NoteId, m.Velocity, isNoteOff: true);
                }
                else if (message is MidiMessageCC)
                {
                    var m = (MidiMessageCC)message;
                    ProcessCCMidiMessage(m.Channel, m.ControlId, m.Value);
                }
                else if (message is MidiMessageChannelPitch)
                {
                    var m = (MidiMessageChannelPitch)message;
                    ProcessPitchMidiMessage(m.Channel, m.Pitch);
                }
            }
            SendLatestAnalogInputValues();           
        }

        private void MidiBoard_Input(object sender, MidiInputEventArgs args)
        {
            MidiMessageQueue.Enqueue(args.Message);
        }

        private MidiBoardDevice FindInputDeviceByLabel(string label)
        {            
            var list = Inputs.Where(kvp => kvp.Value.Label == label);
            if (list.Count() > 0)
            {
                return list.First().Value;
            }
            else
            {
                Log.Instance.log($"Error MidiBoard could not find related input device: {label}.", LogSeverity.Error);            
                return null;
            }
        }

        private void CreateOutputDevice(MidiOutputDefinition output, int index, MidiBoardDevice relatedInput, bool isBlink)
        {
            string label = output.GetLabelWithIndex(index);
            MidiBoardOutputDevice od = new MidiBoardOutputDevice()
            {
                Label = isBlink ? $"{label} - Blink " : label,
                Name = isBlink ? $"{label} - Blink " : label,
                Layer = output.Layer,
                MessageType = output.MessageType,
                Channel = output.MessageChannel,
                Id = output.MessageIds[index],
                ValueOn = isBlink ? output.ValueBlinkOn.Value : output.ValueOn,
                ValueOff = output.ValueOff
            };
            Outputs.Add(od.Name, od);
            Log.Instance.log($"Added Output: {od.Name} Label: {od.Label} Type: {od.Type}.", LogSeverity.Debug);
            Log.Instance.log($"Channel: {od.Channel} Id: {od.Id} ValueOn: {od.ValueOn} ValueOff: {od.ValueOff}.", LogSeverity.Debug);
            relatedInput?.RelatedOutputDevices.Add(od); // create reference to update on input change     
        }

        private void EnumerateOutputDevices()
        {
            Outputs.Clear();
            if (Definition != null && Definition.Outputs != null)
            {
                foreach (var outputDef in Definition.Outputs)
                {                    
                    for (int i = 0; i < outputDef.MessageIds.Length; i++)
                    {
                        // Get related input if configured
                        MidiBoardDevice relatedInput = null;
                        if (!string.IsNullOrEmpty(outputDef.RelatedInputLabel))
                        {
                            string relatedLabel = outputDef.RelatedInputLabel.Replace("%", outputDef.RelatedIds[i]);                        
                            relatedInput = FindInputDeviceByLabel(relatedLabel);
                        }

                        // Add regular LED output device
                        CreateOutputDevice(outputDef, i, relatedInput, isBlink: false);

                        // Add additional LED blink device, if blink value configured 
                        if (outputDef.ValueBlinkOn != null)
                        {
                            CreateOutputDevice(outputDef, i, relatedInput, isBlink: true);
                        }
                    }
                }
            }
        }

        public List<MidiBoardDevice> GetAvailableDevices()
        {
            return Inputs.Values.ToList();
        }

        public List<MidiBoardOutputDevice> GetAvailableOutputDevices()
        {
            return Outputs.Values.ToList();
        }

        public void SetOutputDeviceState(string name, byte state)
        {            
            if (Outputs.ContainsKey(name))
            {
                Outputs[name].IsActive= true;
                if (Outputs[name].State != state)
                {
                    Outputs[name].State = state;
                    UpdateOutputDeviceState(name);
                }
            }
        }

        private void UpdateOutputDeviceState(string name)
        {
            Log.Instance.log($"UpdateOutputDeviceState Name: {name}, HasMidiOutput: {HasMidiOutput}, IsDisconnected: {IsDisconnected}.", LogSeverity.Debug);
            if (HasMidiOutput && !IsDisconnected)
            {                
                var dev = Outputs[name];
                // Only update if part of active layer
                if (ActiveLayer == dev.Layer)
                {
                    byte outputValue = dev.State == 0 ? dev.ValueOff : dev.ValueOn;
                    Log.Instance.log($"Updating Output: {name} with State: {dev.State} and Value: {outputValue}.", LogSeverity.Debug);
                    byte adaptedChannel = (byte)(dev.Channel - 1);
                    try
                    {
                        if (dev.MessageType == MidiMessageType.CC)
                            MidiOutput.Send(new MidiMessageCC(dev.Id, outputValue, adaptedChannel));
                        else if (dev.MessageType == MidiMessageType.Note)
                            MidiOutput.Send(new MidiMessageNoteOn(dev.Id, outputValue, adaptedChannel));
                    }
                    catch (Exception ex)
                    {
                        // Midiboard disconnected
                        IsDisconnected = true;
                        OnDisconnected?.Invoke(this, null);
                    }
                }                                              
            }      
        }

        public void Stop()
        {
            foreach (var kvp in Outputs)
            {
                kvp.Value.State = 0;
                if (kvp.Value.IsActive)
                    UpdateOutputDeviceState(kvp.Key);
                kvp.Value.IsActive = false;                
            }
        }

        public void Shutdown()
        {
            try
            {
                if (this.MidiOutput.IsOpen)
                {
                    MidiOutput.Close();
                }
                if (this.MidiInput.IsOpen)
                {
                    MidiInput.Input -= MidiBoard_Input;
                    MidiInput.Close();
                }
            }
            catch (Exception)
            {
                // Do nothing. Exception when MidiBoard already was disconnected from PC.
            }
        }
    }
}
