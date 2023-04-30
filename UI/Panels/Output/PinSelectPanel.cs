using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.Base;

namespace MobiFlight.UI.Panels
{
    public partial class PinSelectPanel : UserControl
    {
        public const char POSITION_SEPERATOR = '|';
        public bool WideStyle = false;
        public event EventHandler<List<ListItem>> SelectionChanged;

        public PinSelectPanel()
        {
            InitializeComponent();
            checkedListBox.DisplayMember = "Label";
            checkedListBox.ValueMember = "Value";
            checkedListBox.Items.Clear();
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

        internal void SetSelectedPinsFromString(string pins, string serial)
        {
            if (SerialNumber.IsRawSerial(serial))
            {
                serial = SerialNumber.ExtractSerial(serial);
            }

            var splitPins = pins.Split(POSITION_SEPERATOR);
            foreach (string pin in splitPins)
            {
                var pinValue = pin;
                SetSelectedPin(pinValue, true);
            }
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

        private void checkedListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var selection = new List<ListItem>();

            foreach (ListItem checkedItem in checkedListBox.CheckedItems)
            {
                selection.Add(checkedItem);
            }

            SelectionChanged?.Invoke(this, selection);
        }
    }
}
