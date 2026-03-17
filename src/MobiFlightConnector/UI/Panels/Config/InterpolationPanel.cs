using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using MobiFlight.Modifier;
using MobiFlight.UI.Panels.Modifier;

namespace MobiFlight.UI.Panels.Config
{
    public partial class InterpolationPanel : UserControl
    {
        DataTable dt = new DataTable();
        public Boolean Save { get; set; }
        
        public InterpolationPanel()
        {
            InitializeComponent();
            
            dt.Columns.Add("Input");
            dt.Columns.Add("Output");
            dt.Columns[0].DataType = System.Type.GetType("System.Double");
            dt.Columns[1].DataType = System.Type.GetType("System.Double");
            
            DataRow datarow1 = dt.NewRow();
            datarow1["Input"] = 0;
            datarow1["Output"] = 0;
            dt.Rows.Add(datarow1);

            DataRow datarow2 = dt.NewRow();
            datarow2["Input"] = 1024;
            datarow2["Output"] = 1024;
            dt.Rows.Add(datarow2);
            dt.ColumnChanged += new System.Data.DataColumnChangeEventHandler(Dt_ColumnChanged);

            dataGridView1.Columns[0].DataPropertyName = "Input";
            dataGridView1.Columns[1].DataPropertyName = "Output";
            dataGridView1.DataSource = dt;
            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);
            Save = false;

        }

        internal void syncFromConfig(Interpolation i)
        {
            if (i.Count == 0) return;

            dt.Rows.Clear();
            
            foreach (double Key in i.GetValues().Keys)
            {
                DataRow row = dt.NewRow();
                row["Input"] = Key.ToString();
                row["Output"] = i.GetValues()[Key].ToString();
                dt.Rows.Add(row);
            }
        }

        internal Interpolation syncToConfig(Interpolation i)
        {
            if (!Save) return i;

            i.Clear();

            foreach (DataRow row in dt.Rows)
            {
                float x = float.Parse(row[0].ToString());
                float y = float.Parse(row[1].ToString());
                i.Add(x, y);
            }

            return i;
        }

        private void dataGridView1_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
           /* try
            {
                //if ((e.RowIndex + 1) == (sender as DataGridView).Rows.Count) return;
                if (e.FormattedValue.ToString() == "") return;
                float.Parse(e.FormattedValue.ToString());
                (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = null;
                (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = null;
            }
            catch (Exception)
            {
                MessageBox.Show(i18n._tr("uiMessageConfigWizardInterpolationOnlyNumbersPlease"), i18n._tr("hint"));
                (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = i18n._tr("uiMessageConfigWizardInterpolationOnlyNumbersPlease");
                (sender as DataGridView).Rows[e.RowIndex].Cells[e.ColumnIndex].ToolTipText = i18n._tr("uiMessageConfigWizardInterpolationOnlyNumbersPlease");
                e.Cancel = true;
            }*/
        }
        
        private void dataGridView1_CellContentClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                System.Windows.Forms.DataGridView datagridview1 = (sender as DataGridView);
                removeButton.Enabled = (!datagridview1.Rows[e.RowIndex].IsNewRow && datagridview1.Rows.Count > 2);
            }

            dataGridView1.BeginEdit(false);
        }

        private void dataGridView1_CellEnter(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            // don't go into edit mode
            // if the element is not visible
            // CellEnter is triggered during loading the interpolation config
            // And if we .BeginEdit() then, it will throw an exception
            if (!this.Visible) return;

            DataGridView dgv = (sender as DataGridView);
            if (e.RowIndex != dgv.RowCount - 1) return;
            dgv.BeginEdit(true);
        }

        
        private void dataGridView1_RowValidating(object sender, System.Windows.Forms.DataGridViewCellCancelEventArgs e)
        {
            /*
            if (((sender as DataGridView).Rows[e.RowIndex].Cells[0].ToString() == "") || 
                 (sender as DataGridView).Rows[e.RowIndex].Cells[1].ToString() == "")
            {
                (sender as DataGridView).Rows[e.RowIndex].ErrorText = "Only numbers please.";
                e.Cancel = true;
            }
            else
            {
                (sender as DataGridView).Rows[e.RowIndex].ErrorText = "";
            }
            */
        }

        private void Dt_ColumnChanged(object sender, System.Data.DataColumnChangeEventArgs e)
        {
            this.Save = true;
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            System.Collections.IEnumerator ienumerator1 = this.dataGridView1.SelectedRows.GetEnumerator();
            foreach (DataGridViewRow row in dataGridView1.SelectedRows)
            {
                this.dataGridView1.Rows.Remove(row);
            }
        }

        internal void SetEditMode(bool Enabled)
        {
            this.dataGridView1.Enabled = Enabled;
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            System.Windows.Forms.DataGridView datagridview1 = (sender as DataGridView);
            if (dataGridView1 == null) return;
            System.Windows.Forms.DataGridViewRow datagridviewrow1 = datagridview1.Rows[e.RowIndex];

            if (datagridviewrow1.IsNewRow)
            {
                if (e.RowIndex < 2) return;

                double num1 = (double)(this.dataGridView1.Rows[(datagridviewrow1.Index - 1)].Cells[0].Value) + (double)(this.dataGridView1.Rows[(datagridviewrow1.Index - 1)].Cells[0].Value) - (double)(this.dataGridView1.Rows[(datagridviewrow1.Index - 2)].Cells[0].Value);
                double num2 = (double)(this.dataGridView1.Rows[(datagridviewrow1.Index - 1)].Cells[1].Value) + (double)(this.dataGridView1.Rows[(datagridviewrow1.Index - 1)].Cells[1].Value) - (double)(this.dataGridView1.Rows[(datagridviewrow1.Index - 2)].Cells[1].Value);
                datagridviewrow1.Cells[0].Value = (double)(num1);
                datagridviewrow1.Cells[1].Value = (double)(num2);
            }
        }

        private void dataGridView1_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            DataGridView datagridview1 = (sender as DataGridView);
            if (dataGridView1 == null) return;
            DataGridViewRow datagridviewrow1 = datagridview1.Rows[e.RowIndex];

            try {
                if (datagridviewrow1.IsNewRow && e.ColumnIndex == 1)
                {
                    double tmp = double.Parse(datagridviewrow1.Cells[0].Value.ToString());
                    double tmp1 = double.Parse(datagridviewrow1.Cells[1].Value.ToString());
                    dataGridView1.NotifyCurrentCellDirty(true);
                }
            } catch (Exception)
            {
            }
            datagridview1.EndEdit();
        }

        private void dataGridView1_KeyUp(object sender, KeyEventArgs e)
        {
            /*
            if (e.KeyCode == Keys.Tab && e.Modifiers == Keys.None && dataGridView1.SelectedRows.Count > 0)
            {
                if (dataGridView1.SelectedRows[0].Index + 2 == dataGridView1.Rows.Count && dataGridView1.CurrentCell.ColumnIndex==1)
                {
                    dataGridView1.Rows[dataGridView1.Rows.Count - 1].Selected = true;
                    dataGridView1.CurrentCell = dataGridView1.Rows[dataGridView1.Rows.Count - 1].Cells[0];
                    dataGridView1.BeginEdit(false);
                    e.Handled = true;
                }
                
            }
            */
        }

        private void dataGridView1_SelectionChanged(object sender, EventArgs e)
        {
            if (dataGridView1.CurrentCell == null) return;
            if (dataGridView1.CurrentCell.RowIndex >= 0)
            {
                System.Windows.Forms.DataGridView datagridview1 = (sender as DataGridView);
                removeButton.Enabled = (!datagridview1.Rows[dataGridView1.CurrentCell.RowIndex].IsNewRow && datagridview1.Rows.Count > 3);
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            DataRow datarow2 = dt.NewRow();
            int index1 = dataGridView1.RowCount - 2;
            int index2 = dataGridView1.RowCount - 1;

            double num1 = (double)(this.dataGridView1.Rows[index2].Cells[0].Value) + (double)(this.dataGridView1.Rows[index2].Cells[0].Value) - (double)(this.dataGridView1.Rows[index1].Cells[0].Value);
            double num2 = (double)(this.dataGridView1.Rows[index2].Cells[1].Value) + (double)(this.dataGridView1.Rows[index2].Cells[1].Value) - (double)(this.dataGridView1.Rows[index1].Cells[1].Value);

            datarow2["Input"] = num1;
            datarow2["Output"] = num2;
            dt.Rows.Add(datarow2);

            dataGridView1.FirstDisplayedScrollingRowIndex = index2;
        }
    }
}
