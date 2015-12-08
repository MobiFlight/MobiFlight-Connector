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
        public InterpolationPanel()
        {
            InitializeComponent();
            
            dt.Columns.Add("Input");
            //dt.Columns[0].DataType = System.Type.GetType("System.Decimal");
            dt.Columns.Add("Output");

            dataGridView1.Columns.Clear();
            dataGridView1.DataSource = dt;

            dataGridView1.Sort(dataGridView1.Columns[0], ListSortDirection.Ascending);

        }

        internal void syncFromConfig(Interpolation i)
        {
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
                if (e.FormattedValue.ToString() == "") return;
                float.Parse(e.FormattedValue.ToString());
            }
            catch (Exception)
            {
                (sender as DataGridView).Rows[e.RowIndex].ErrorText = "Only numbers please.";
                e.Cancel = true;
            }
        }
    }
}
