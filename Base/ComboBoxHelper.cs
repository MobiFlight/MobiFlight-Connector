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
    }
}
