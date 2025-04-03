using GraphQL.Client.Http;
using GraphQL.Client.Serializer.Newtonsoft;
using MobiFlight.ProSim;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace MobiFlight.UI.Panels.Config
{
    public partial class ProSimDatarefPanel : UserControl
    {
        public event EventHandler ModifyTabLink;

        private Dictionary<string, DataRefDescription> _dataRefDescriptions;

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

        private void button1_Click(object sender, EventArgs e)
        {
            var host = Properties.Settings.Default.ProSimHost;
            var port = Properties.Settings.Default.ProSimPort;
            var _connection = new GraphQLHttpClient($"http://{host}:{port}/graphql", new NewtonsoftJsonSerializer());
            Task.Run(() =>
            {
                var dataRefDescriptions = _connection.SendQueryAsync<DataRefData>(new GraphQL.GraphQLRequest
                {
                    Query = @"
                {
                    dataRef {
                    dataRefDescriptions: list {
                    		name
                    		description
                    		canRead
                    		canWrite
                    		dataType
                    		dataUnit
                        __typename
                    }
                    __typename
                    }
                }
                "
                }).Result;

                _dataRefDescriptions = dataRefDescriptions.Data.DataRef.DataRefDescriptions.ToDictionary(drd => drd.Name);
                dataRefDescriptionsComboBox.Items.AddRange(_dataRefDescriptions.Keys.ToArray());
            });


            //var proSimConnection = new GraphQLHttpClient();
            //var host = !string.IsNullOrWhiteSpace(Properties.Settings.Default.ProSimHost)
            //    ? Properties.Settings.Default.ProSimHost
            //    : "localhost";
            //proSimConnection.Connect(host);

            //_dataRefDescriptions = proSimConnection.getDataRefDescriptions().ToDictionary(drd => drd.Name);
            //dataRefDescriptionsComboBox.Items.AddRange(_dataRefDescriptions.Keys.ToArray());
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