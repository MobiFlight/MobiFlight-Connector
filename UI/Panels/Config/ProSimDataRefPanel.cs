using MobiFlight.ProSim;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Forms;
using WebSocketSharp;

namespace MobiFlight.UI.Panels.Config
{
    public partial class ProSimDataRefPanel : UserControl
    {
        public event EventHandler ModifyTabLink;

        private Dictionary<string, DataRefDescription> _dataRefDescriptions;
        private List<DataRefDescription> _canReadDataRefDescriptions;
        private List<DataRefDescription> _canReadDataRefDescriptionsFiltered;
        private bool _isLoading = true;
        private bool _isOutputMode = true;

        private IExecutionManager _executionManager;

        [Description("ProSim DataRef Path"), Category("Data")]
        public string Path
        {
            get => DatarefPathTextBox.Text;
            set => DatarefPathTextBox.Text = value;
        }

        [Description("ProSim Transform Group"), Category("Data")]
        public TransformOptionsGroup TransformOptionsGroup
        {
            get => transformOptionsGroup1;
            set => transformOptionsGroup1 = value;
        }

        public ProSimDataRefPanel()
        {
            InitializeComponent();
            dataGridView1.AutoGenerateColumns = false;
            dataGridView1.AllowUserToResizeRows = false;
            dataGridView1.DataBindingComplete += DataGridView1_DataBindingComplete;
        }

        private void DataGridView1_DataBindingComplete(object sender, DataGridViewBindingCompleteEventArgs e)
        {
            _isLoading = false;
        }

        public void SetMode(bool isOutputPanel)
        {
            _isOutputMode = isOutputPanel;
            transformOptionsGroup1.setMode(isOutputPanel);
        }

        public void Init(IExecutionManager executionManager)
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
                _isLoading = true;
                // Get the dataref descriptions from the already-connected ProSimCache
                _dataRefDescriptions = proSimCache.GetDataRefDescriptions();
                _canReadDataRefDescriptions = _dataRefDescriptions.Values
                    .Where(drd => _isOutputMode ? drd.CanRead : drd.CanWrite)
                    .ToList();

                if (_dataRefDescriptions.Count > 0)
                {
                    // Marshal the UI update to the main thread
                    if (this.InvokeRequired)
                    {
                        this.Invoke(new System.Action(() =>
                        {
                            _canReadDataRefDescriptions.Sort((drd1, drd2) => drd2.Name.CompareTo(drd1.Name));
                            dataGridView1.DataSource = null;
                            dataGridView1.DataSource = _canReadDataRefDescriptions;
                        }));
                    }
                    else
                    {
                        _canReadDataRefDescriptions.Sort((drd1, drd2) => drd2.Name.CompareTo(drd1.Name));
                        dataGridView1.DataSource = null;
                        dataGridView1.DataSource = _canReadDataRefDescriptions;
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Instance.log($"Error retrieving ProSim dataref descriptions: {ex.Message}", LogSeverity.Error);
            }
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
            if (!_isLoading && dataGridView1.SelectedRows.Count > 0) 
            {
                var drd = dataGridView1.Rows[e.RowIndex].DataBoundItem as DataRefDescription;
                DatarefPathTextBox.Text = drd.Name;
            }
        }
    }
} 