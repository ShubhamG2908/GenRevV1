using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

namespace Genrev.Data.DTOs
{
    public class CustomerIDsTable
    {
        
        public static DataTable ToDataTable(int[] ids) {

            DataTable table = new DataTable();

            table.Columns.Add("ID");
            foreach (int i in ids) {
                table.Rows.Add(i);
            }

            return table;

        }

    }
}
