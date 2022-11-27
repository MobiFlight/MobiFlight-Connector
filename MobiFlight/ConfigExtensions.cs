using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MobiFlight
{
    public static class ConfigExtensions
    {
        public static List<ListItem> GetConfigsWithGuidAndLabel(this DataSet dataSet, string filterGuid)
        {
            var result = new List<ListItem>();
            DataView dv = new DataView(dataSet.Tables["config"]);
            dv.RowFilter = "guid <> '" + filterGuid + "'";

            foreach (DataRow row in dv.ToTable().Rows)
            {
                result.Add(new ListItem()
                {
                    Label = row["description"] as string,
                    Value = row["guid"].ToString()
                });
            }

            return result;
        }
    }
}
