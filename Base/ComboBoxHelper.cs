using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight;

namespace System
{
    public static class ComboBoxHelper
    {
        static public bool SetSelectedItem(ComboBox comboBox, string value)
        {
            Log.Instance.log("Set " + value + " in ComboBox: " + comboBox.Name, LogSeverity.Debug);
            if (comboBox.FindStringExact(value) != -1)
            {
                comboBox.SelectedIndex = comboBox.FindStringExact(value);
                return true;
            }
            return false;
        }

        static public bool SetSelectedItemByIndex(ComboBox comboBox, int index)
        {
            Log.Instance.log("Set " + index + " in ComboBox: " + comboBox.Name, LogSeverity.Debug);
            if (comboBox.Items.Count > index)
            {
                comboBox.SelectedIndex = index;
                return true;
            }
            return false;
        }

        static public bool SetSelectedItemByPart(ComboBox comboBox, string value)
        {
            foreach (object item in comboBox.Items)
            {
                if ((item.ToString()).Contains(value))
                {
                    comboBox.SelectedIndex = comboBox.FindStringExact(item.ToString());
                    return true;
                }
            }

            return false;
        }

        static public bool BindMobiFlightFreePins(ComboBox comboBox, List<MobiFlightPin> Pins, String CurrentPin, bool analogOnly = false)
        {
            // This function returns a list of all currently free pins, plus the specified one marked as free.
            // Required because, in a selection list for a device signal, the already assigned pin must be in the list in order to be selectable.
            
            if (Pins == null) return false;
            // Deep-clone list as 'Used' list
            List<MobiFlightPin> UsablePins = Pins.ConvertAll(pin => new MobiFlightPin(pin));
            // Mark current pin as free
            if (UsablePins.Exists(x => x.Pin == byte.Parse(CurrentPin)))
                UsablePins.Find(x => x.Pin == byte.Parse(CurrentPin)).Used = false;

            if (analogOnly == true)
            {
                UsablePins = UsablePins.FindAll(x => x.isAnalog == true);
            }

            // Assign the all-free list to the combobox
            comboBox.DataSource = UsablePins.FindAll(x => x.Used == false);
            comboBox.DisplayMember = "Name";
            comboBox.ValueMember = "Pin";

            return false;
        }
        static public void reassignPin(ComboBox comboBox, List<MobiFlightPin> pinList, ref string before)
        {
            string after = comboBox.SelectedItem.ToString();
            byte nBefore = byte.Parse(before);
            byte nAfter = byte.Parse(after);
            try {
                if (before != after) {
                    pinList.Find(x => x.Pin == nBefore).Used = false;
                    pinList.Find(x => x.Pin == nAfter).Used = true;
                }
            }
            catch (Exception e) {
                Log.Instance.log($"Pin reassignment from {before} to {after} went wrong", LogSeverity.Debug);
            }
            ComboBoxHelper.BindMobiFlightFreePins(comboBox, pinList, after);
            comboBox.SelectedValue = nAfter;
            before = after;
        }
    }
}
