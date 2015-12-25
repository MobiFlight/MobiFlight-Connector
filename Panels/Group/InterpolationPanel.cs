using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace MobiFlight.Panels.Group
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
            
            foreach (float Key in i.GetValues().Keys)
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
            try
            {
                if ((e.RowIndex + 1) == (sender as DataGridView).Rows.Count) return;
                if (e.FormattedValue.ToString() == "") return;
                float.Parse(e.FormattedValue.ToString());
            }
            catch (Exception)
            {
                (sender as DataGridView).Rows[e.RowIndex].ErrorText = "Only numbers please.";
                e.Cancel = true;
            }
        }
        
        private void dataGridView1_CellContentClick(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            if (e.RowIndex >= 0) 
            {
                System.Windows.Forms.DataGridView datagridview1 = (sender as DataGridView);
                removeButton.Enabled = (!datagridview1.Rows[e.RowIndex].IsNewRow && datagridview1.Rows.Count > 3);
            }
        }

        private void dataGridView1_CellEnter(object sender, System.Windows.Forms.DataGridViewCellEventArgs e)
        {
            System.Windows.Forms.DataGridView datagridview1 = (sender as DataGridView);
            if (datagridview1 != null)
            {
                if (e.RowIndex < 2) return;

                System.Windows.Forms.DataGridViewRow datagridviewrow1 = datagridview1.Rows[e.RowIndex];
                if (datagridviewrow1.IsNewRow)
                {
                    double num1 = ((double)(this.dataGridView1.Rows[(datagridviewrow1.Index - 1)].Cells[0].Value) + Math.Round((((double)(this.dataGridView1.Rows[(datagridviewrow1.Index - 1)].Cells[0].Value) - (double)(this.dataGridView1.Rows[(datagridviewrow1.Index - 2)].Cells[0].Value)) / 2), 0));
                    double num2 = ((double)(this.dataGridView1.Rows[(datagridviewrow1.Index - 1)].Cells[1].Value) + Math.Round((((double)(this.dataGridView1.Rows[(datagridviewrow1.Index - 1)].Cells[1].Value) - (double)(this.dataGridView1.Rows[(datagridviewrow1.Index - 2)].Cells[1].Value)) / 2), 0));
                    datagridviewrow1.Cells[0].Value = (double)(num1);
                    datagridviewrow1.Cells[1].Value = (double)(num2);
                    datagridview1.EndEdit();
                }
            }
        }

        
        private void dataGridView1_RowValidating(object sender, System.Windows.Forms.DataGridViewCellCancelEventArgs e)
        {
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

    }
}
