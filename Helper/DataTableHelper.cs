using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TestConsole.Helper
{
    public class DataTableHelper
    {
        public static DataTable ToDataTable<T>(List<T> datas) where T : class
        {
            DataTable dt = new DataTable();
            if (datas == null || !datas.Any())
            {
                return dt;
            }
            // Get properties of the first item to define columns
            var properties = typeof(T).GetProperties();
            foreach (var prop in properties)
            {
                dt.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            }
            // Add rows
            foreach (var data in datas)
            {
                var row = dt.NewRow();
                foreach (var prop in properties)
                {
                    row[prop.Name] = prop.GetValue(data) ?? DBNull.Value;
                }
                dt.Rows.Add(row);
            }
            return dt;
        }
    }
}
