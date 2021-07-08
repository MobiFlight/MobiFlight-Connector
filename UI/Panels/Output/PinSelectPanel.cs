using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels
{
    public partial class PinSelectPanel : UserControl
    {
        const char POSITION_SEPERATOR = '|';
        public bool WideStyle = false;

        public PinSelectPanel()
        {
            InitializeComponent();
            checkedListBox.DisplayMember = "Label";
            checkedListBox.ValueMember = "Value";
        }

        public void SetSelectedPin(string value, bool isChecked)
        {

            ListItem li = checkedListBox.Items.Cast<ListItem>().ToList().Find(p => p.Value == value);
            if (li!=null)
            {
                int index = checkedListBox.Items.IndexOf(li);
                if (index >= 0 && index < checkedListBox.Items.Count)
                {                    
                    checkedListBox.SetItemChecked(index, isChecked);
                }
            }
        }

        public void SetPins(List<ListItem> pins)
        {
            checkedListBox.Items.Clear();
            foreach (var pin in pins)
            {
                checkedListBox.Items.Add(pin);
            }
        }

        internal string SetSelectgedPinsFromString(string pins, string serial)
        {
            if (string.IsNullOrEmpty(pins))
            {
                return string.Empty;
            }
            string port = string.Empty;

            if (serial != null && serial.Contains('/'))
            {
                serial = serial.Split('/')[1].Trim();
            }

            var splitPins = pins.Split(POSITION_SEPERATOR);
            foreach (string pin in splitPins)
            {
                var pinValue = pin;
                if (serial != null && serial.IndexOf("SN") != 0)
                {
                    pinValue = pin.Substring(1);

                    // TODO: Only last port is used. I don't know how ports work here. 
                    // Letr it to Sebastian to fix.
                    port = pin.Substring(0, 1);
                }
                SetSelectedPin(pinValue, true);
            }
            return port;
        }

        internal string GetSelectedPinString()
        {
            StringBuilder pins = new StringBuilder();
            int counter = 0;
            foreach (ListItem checkedItem in checkedListBox.CheckedItems)
            {
                if (counter > 0)
                {
                    pins.Append("|");
                }
                pins.Append(checkedItem.Value);
                counter++;
            }
            return pins.ToString();
        }

    }
}
