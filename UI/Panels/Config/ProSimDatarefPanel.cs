using MobiFlight.ProSim;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using WebSocketSharp;

namespace MobiFlight.UI.Panels.Config
{
    public partial class ProSimDatarefPanel : UserControl
    {
        public event EventHandler ModifyTabLink;

        private Dictionary<string, DataRefDescription> _dataRefDescriptions;
        private List<DataRefDescription> _canReadDataRefDescriptions;
        private List<DataRefDescription> _canReadDataRefDescriptionsFiltered;

        private ExecutionManager _executionManager;

        [Description("ProSim DataRef Path"), Category("Data")]
        public string Path
        {
            get => DatarefPathTextBox.Text;
            set => DatarefPathTextBox.Text = value;
        }

        public ProSimDatarefPanel()
        {
            InitializeComponent();
            transformOptionsGroup1.setMode(true);
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToResizeRows = false;

        }

        public void Init(ExecutionManager executionManager)
        {
            _executionManager = executionManager;
        }

        internal void syncToConfig(OutputConfigItem config)
        {
            config.Source = new Base.ProSimSource() { 
                ProSimDataRef = new ProSim.ProSimDataRef()
                {
                    Path = DatarefPathTextBox.Text.Trim()
                }
            };
            transformOptionsGroup1.syncToConfig(config);
        }

        internal void syncFromConfig(OutputConfigItem config)
        {
            if (!(config.Source is Base.ProSimSource)) return;
            
            DatarefPathTextBox.Text = (config.Source as Base.ProSimSource).ProSimDataRef.Path;
            transformOptionsGroup1.syncFromConfig(config);
        }

        private void ProSimDatarefPanel_Load(object sender, EventArgs e)
        {
            dataRefDescriptionsComboBox.SelectedValueChanged += DataRefDescriptionsComboBox_SelectedValueChanged;
            dataRefDescriptionsComboBox.KeyDown += DataRefDescriptionsComboBox_KeyDown;
            dataRefDescriptionsComboBox.KeyUp += DataRefDescriptionsComboBox_KeyUp;
        }

        private void DataRefDescriptionsComboBox_KeyDown(object sender, KeyEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            if (e.KeyCode == Keys.Enter)
            {
                if (cb.DroppedDown && cb.SelectedIndex >= 0)
                {
                    // Accept the selected item from dropdown
                    cb.Text = cb.SelectedItem.ToString();
                }

                if (!string.IsNullOrEmpty(cb.Text))
                {
                    int index = cb.FindStringExact(cb.Text);
                    if (index >= 0)
                    {
                        cb.SelectedIndex = index;
                    }
                    else
                    {
                        // Handle new item
                        cb.Items.Add(cb.Text);
                        cb.SelectedIndex = cb.Items.Count - 1;
                    }

                    e.Handled = true;
                    e.SuppressKeyPress = true;
                    cb.DroppedDown = false; // Close the dropdown
                }
            }
            else if (e.KeyCode == Keys.Down)
            {
                // Ensure dropdown is visible when navigating
                if (!cb.DroppedDown)
                {
                    cb.DroppedDown = true;
                }
            }
        }

        private void DataRefDescriptionsComboBox_KeyUp(object sender, KeyEventArgs e)
        {
            ComboBox cb = (ComboBox)sender;

            if (e.KeyCode == Keys.Down && cb.DroppedDown)
            {
                // Update text with the currently highlighted item
                if (cb.SelectedIndex >= 0)
                {
                    cb.Text = cb.Items[cb.SelectedIndex].ToString();
                    cb.SelectionStart = cb.Text.Length; // Move cursor to end
                }
            }
        }
        private void DataRefDescriptionsComboBox_SelectedValueChanged(object sender, EventArgs e)
        {
            DatarefPathTextBox.Text = dataRefDescriptionsComboBox.SelectedItem.ToString();
        }

        public void LoadDataRefDescriptions()
        {
            if (_executionManager == null)
            {
                return; // Silently return if not initialized
            }

            var proSimCache = _executionManager.GetProSimCache();
            if (!proSimCache.IsConnected())
            {
                return; // Silently return if not connected
            }

            try
            {
                // Get the dataref descriptions from the already-connected ProSimCache
                _dataRefDescriptions = proSimCache.GetDataRefDescriptions();
                _canReadDataRefDescriptions = _dataRefDescriptions.Values.Where(drd => drd.CanRead).ToList();

                if (_dataRefDescriptions.Count > 0)
                {
                    // Marshal the UI update to the main thread
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new System.Action(() =>
                        {
                            dataGridView1.DataSource = _canReadDataRefDescriptions;
                            dataRefDescriptionsComboBox.Items.Clear();
                            dataRefDescriptionsComboBox.Items.AddRange(_dataRefDescriptions.Keys.ToArray());
                        }));
                    }
                    else
                    {
                        dataGridView1.DataSource = _canReadDataRefDescriptions;
                        dataRefDescriptionsComboBox.Items.Clear();
                        dataRefDescriptionsComboBox.Items.AddRange(_dataRefDescriptions.Keys.ToArray());
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error retrieving ProSim dataref descriptions: {ex.Message}", LogSeverity.Error);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            LoadDataRefDescriptions();
        }

        private void textBox1_TextChanged(object sender, EventArgs e)
        {
            if (!textBox1.Text.IsNullOrEmpty()) {
                var words = textBox1.Text.Split(' ').Select(w => w.ToLower()).ToArray();
                _canReadDataRefDescriptionsFiltered = _canReadDataRefDescriptions
                    .Where(drd => words.All(drd.Name.ToLower().Contains)
                    || words.All(drd.Description.ToLower().Contains)
                    || (words.Length > 1 && drd.Name.ToLower().Contains(words[0]) && words.Skip(1).All(drd.Description.ToLower().Contains))).ToList();
                dataGridView1.DataSource = _canReadDataRefDescriptionsFiltered;
            } else
            {
                dataGridView1.DataSource = _canReadDataRefDescriptions;
            }
        }

        private void dataGridView1_RowEnter(object sender, DataGridViewCellEventArgs e)
        {
            var drd = dataGridView1.Rows[e.RowIndex].DataBoundItem as DataRefDescription;
            DatarefPathTextBox.Text = drd.Name;
        }
    }

    public class AutoCompleteComboBox : ComboBox
    {
        protected override bool IsInputKey(Keys keyData)
        {
            switch ((keyData & (Keys.Alt | Keys.KeyCode)))
            {
                case Keys.Enter:
                case Keys.Escape:
                    if (this.DroppedDown)
                    {
                        this.DroppedDown = false;
                        return false;
                    }
                    break;
            }
            return base.IsInputKey(keyData);
        }
    }
} 