using M;
using MobiFlight.UI.Dialogs;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace MobiFlight
{
    public class MidiBoardManager
    {
        public event EventHandler Connected;
        public event ButtonEventHandler OnButtonPressed;      
        private readonly List<MidiBoard> MidiBoards = new List<MidiBoard>();
        private readonly List<MidiBoard> BoardsToBeRemoved = new List<MidiBoard>();
        private readonly Dictionary<string, MidiBoardDefinition> Definitions = new Dictionary<string, MidiBoardDefinition>();
        private readonly Timer ProcessTimer = new System.Windows.Forms.Timer();
        private int CheckAttachedRemovedCounter = 0;

        public MidiBoardManager ()
        {
            LoadDefinitions();
            ProcessTimer.Interval = 50;
            ProcessTimer.Tick += ProcessTimer_Tick;
        }

        private List<string> CollectErrorsInInputDefinition(MidiBoardDefinition def)
        {
            List<string> errorMessages = new List<string>();
            if (def.Inputs == null)
                errorMessages.Add($"No input definitions.");
            else
            {
                foreach (var item in def.Inputs)
                {
                    if (item.LabelIds.Length != item.MessageIds.Length)
                        errorMessages.Add($"{item.Label}: Number of LabelIds unequal to number of MessageIds");
                    if (!item.Label.Contains("%"))
                        errorMessages.Add($"{item.Label}: Label does not contain % placeholder.");
                }
            }
            return errorMessages;
        }

        private List<string> CollectErrorsInOutputDefinition(MidiBoardDefinition def)
        {
            List<string> errorMessages = new List<string>();              
            if (def.Outputs == null) return errorMessages;
            
            foreach (var item in def.Outputs)
            {
                if (item.LabelIds.Length != item.MessageIds.Length)
                    errorMessages.Add($"{item.Label}: Number of LabelIds unequal to number of MessageIds");
                if (!item.Label.Contains("%"))
                    errorMessages.Add($"{item.Label}: Label does not contain % placeholder.");
                if (!string.IsNullOrEmpty(item.RelatedInputLabel))
                {
                    if (item.RelatedIds.Length != item.MessageIds.Length)
                        errorMessages.Add($"{item.Label}: Number of RelatedIds unequal to number of MessageIds");
                    if (!item.RelatedInputLabel.Contains("%"))
                        errorMessages.Add($"{item.Label}: RelatedInputLabel does not contain % placeholder.");
                }
            }            
            return errorMessages;
        }


        private List<string> CollectAndLogDefinitionErrors(MidiBoardDefinition def)
        {
            List<string> errorMessages = new List<string>();
            errorMessages.AddRange(CollectErrorsInInputDefinition(def));
            errorMessages.AddRange(CollectErrorsInOutputDefinition(def));
            foreach (var error in errorMessages)
            {
                Log.Instance.log($"[{def.InstanceName}]: {error}", LogSeverity.Error);
            }
            return errorMessages;            
        }


        private void LoadDefinitions()
        {
            foreach (var definitionFile in Directory.GetFiles("MidiBoards", "*.midiboard.json"))
            {
                try
                {
                    var midiBoardDef = JsonConvert.DeserializeObject<MidiBoardDefinition>(File.ReadAllText(definitionFile));
                    if (CollectAndLogDefinitionErrors(midiBoardDef).Count == 0)
                    {
                        Definitions.Add(midiBoardDef.InstanceName, midiBoardDef);
                        Log.Instance.log($"Loaded midiBoard definition for {midiBoardDef.InstanceName}", LogSeverity.Info);
                    }                    
                }
                catch (Exception ex)
                {
                    Log.Instance.log($"Unable to load {definitionFile}: {ex.Message}", LogSeverity.Error);
                }
            }         
        }

        public bool AreMidiBoardsConnected()
        {
            return MidiBoards.Count > 0;
        }
      
        public void Startup()
        {
            ProcessTimer.Start();
        }

        private void ProcessTimer_Tick(object sender, EventArgs e)
        {    
            foreach (var board in BoardsToBeRemoved)
            {
                MidiBoards.Remove(board);
            }
            BoardsToBeRemoved.Clear();
            
            foreach (var mb in MidiBoards)
            {
                mb.ProcessMessages();
            }
            UpdateOnAttachedOrRemovedMidiBoards();           
        }

        private void UpdateOnAttachedOrRemovedMidiBoards()
        {
            CheckAttachedRemovedCounter++;
            // Check every 2 seconds
            if (CheckAttachedRemovedCounter < 40) return;   
            
            CheckAttachedRemovedCounter = 0;
            if (MidiDevice.InputsCount == MidiBoards.Count) return;

            Log.Instance.log($"Change in MIDI Board Setup detected.", LogSeverity.Info);
            foreach (var mb in MidiBoards)
            {
                mb.Shutdown();
            }
            MidiBoards.Clear();
            Connect();                      
        }

        public void Stop()
        {           
            foreach (var mb in MidiBoards)
            {
                mb.Stop();
            }           
        }

        public void Shutdown()
        {
            ProcessTimer.Stop();
            foreach (var mb in MidiBoards)
            {
                mb.Shutdown();
            }
        }

        public List<MidiBoard> GetMidiBoards()
        {
            return MidiBoards;
        }

        public void Connect()
        {
            // Midi input and output devices can reliably only be correlated via the name.
            // Several midi devices of the same type lead to identical names in input and output list.
            // Strategy:
            // Enumerate midi input devices, try to find and remove corresponding first output device.
            // If input device with same type is found again, append _2, _3 and so on to registered name.
            // Example:
            // Inputs : "X-TOUCH MINI", "MPD218", "X-TOUCH MINI", "KORGCONTROL"
            // Outputs: "MS Wavetable Synth", "X-TOUCH MINI", "MPD218", "X-TOUCH MINI"
            // -> Leads to MidiBoards: "X-TOUCH MINI", "MPD218", "X-TOUCH MINI_2", "KORGCONTROL" 
            Dictionary<string, int> registeredBoards = new Dictionary<string, int>();
            var inputList = new List<MidiInputDevice>(MidiDevice.Inputs);
            var outputList = new List<MidiOutputDevice>(MidiDevice.Outputs);
            inputList.ForEach(i => { Log.Instance.log($"Found MidiInput: {i.Name}, Index: {i.Index}.", LogSeverity.Debug); });
            outputList.ForEach(o => { Log.Instance.log($"Found MidiOutput: {o.Name}, Index {o.Index}.", LogSeverity.Debug); });

            while (inputList.Count > 0)
            {
                MidiInputDevice midiInput = inputList[0];
                MidiOutputDevice midiOutput = null;                
                string name = midiInput.Name;                
                MidiBoardDefinition def = Definitions.ContainsKey(name) ? Definitions[name] : null;
                string outputName = (def != null && !string.IsNullOrEmpty(def.DifferingOutputName)) ? def.DifferingOutputName : name;

                int index = outputList.FindIndex(o => o.Name == outputName);
                if (index >= 0)
                {
                    midiOutput = outputList[index];
                    outputList.RemoveAt(index);                   
                }
                inputList.RemoveAt(0);
                if (registeredBoards.ContainsKey(name))     
                    name = name + $"_{++registeredBoards[name]}";
                else
                    registeredBoards[name] = 1;
                                    
                MidiBoard mb = new MidiBoard(midiInput, midiOutput, name, def);
                ConnectBoard(mb);
            }

            if (AreMidiBoardsConnected())
            {
                Connected?.Invoke(this, null);
            }
        }

        private void ConnectBoard(MidiBoard board)
        {
            try
            {
                board.Connect();
                board.OnDisconnected += Mb_OnDisconnected;
                board.OnButtonPressed += Mb_OnButtonPressed;
                MidiBoards.Add(board);
                Log.Instance.log($"Adding attached Midi Device: {board.Name},  HasMidiOutput: {board.HasMidiOutput}.", LogSeverity.Info);
            }
            catch (MidiBoardInUsageByOtherProcessException ex)
            {
                TimeoutMessageDialog tmd = new TimeoutMessageDialog
                {
                    HasCancelButton = false,
                    StartPosition = FormStartPosition.CenterParent,
                    Message = string.Format(i18n._tr("uiMessageMidiBoardInUsageByOtherProcess"), board.Name),
                    Text = i18n._tr("Hint")
                };
                tmd.ShowDialog();
            }
        }

        private void Mb_OnDisconnected(object sender, EventArgs e)
        {
            MidiBoard mb = sender as MidiBoard;
            Log.Instance.log($"MidiBoard disconnected: {mb.Name}.", LogSeverity.Error);
            BoardsToBeRemoved.Add(mb);                    
        }

        private void Mb_OnButtonPressed(object sender, InputEventArgs e)
        {
            OnButtonPressed?.Invoke(sender, e);
        }

        public MidiBoard GetMidiBoardBySerial(string serial)
        {
            MidiBoard result = null;
            result = MidiBoards.Find(mb => mb.Serial == serial);
            return result;
        }

        public string MapDeviceNameToLabel(string boardName, string deviceName)
        {
            if (Definitions.ContainsKey(boardName))
            {
                var def = Definitions[boardName];   
                if (def.InputNameToLabelDictionary.ContainsKey(deviceName))
                {
                    return def.InputNameToLabelDictionary[deviceName];
                }
            }
            return deviceName;
        }

        public Dictionary<String, int> GetStatistics()
        {
            Dictionary<String, int> result = new Dictionary<string, int>();
            result["MidiBoards.Count"] = MidiBoards.Count();            
            foreach (MidiBoard mb in MidiBoards)
            {
                string key = "MidiBoard.Model." + mb.Name;

                if (!result.ContainsKey(key)) 
                    result[key] = 0;
                result[key] += 1;
            }
            return result;
        }
    }
}
