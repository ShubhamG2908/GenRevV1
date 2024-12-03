using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.Data.OleDb;
using System.IO;

namespace Genrev.Data.Importing
{
    class CommonServices
    {
        
        public static DataTable GetTableFromCsv(string path, bool containsHeaders) {

            string header = containsHeaders ? "Yes" : "No";

            string pathOnly = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);

            string sql = "SELECT * FROM [" + fileName + "]";

            using (OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + pathOnly + ";Extended Properties=\"Text;HDR=" + header + "\""))
            using (OleDbCommand cmd = new OleDbCommand(sql, conn))
            using (OleDbDataAdapter adapter = new OleDbDataAdapter(cmd)) {
                
                DataTable table = new DataTable();
                adapter.Fill(table);
                adapter.Dispose();
                
                return table;
                
            }

        }
    }
}
