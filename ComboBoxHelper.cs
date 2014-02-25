using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace System
{
    public static class ComboBoxHelper
    {
        static public bool SetSelectedItem(ComboBox comboBox, string value)
        {
            if (comboBox.FindStringExact(value) != -1)
            {
                comboBox.SelectedIndex = comboBox.FindStringExact(value);
                return true;
            }
            return false;
        }

        static public bool SetSelectedItemByPart(ComboBox comboBox, string value)
        {
            foreach (string item in comboBox.Items)
            {
                if (item.Contains(value))
                {
                    comboBox.SelectedIndex = comboBox.FindStringExact(item);
                    return true;
                }
            }

            return false;
        }
    }
}
