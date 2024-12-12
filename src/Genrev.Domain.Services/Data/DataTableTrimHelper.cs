using System.Data;

namespace Genrev.DomainServices.Data
{
    public class DataTableTrimHelper
    {
        public static DataTable RemoveEmptyRows(DataTable table)
        {
            for (int i = table.Rows.Count - 1; i >= 0; i--)
            {
                bool isEmptyRow = true;
                foreach (var item in table.Rows[i].ItemArray)
                {
                    if (item != null && item.ToString().Trim() != string.Empty)
                    {
                        isEmptyRow = false;
                        break;
                    }
                }
                if (isEmptyRow)
                    table.Rows.RemoveAt(i);
            }
            return table;
        }

        public static DataTable RemoveEmptyColumns(DataTable table)
        {
            for (int i = table.Columns.Count - 1; i >= 0; i--)
            {
                bool isEmptyColumn = true;
                foreach (DataRow row in table.Rows)
                {
                    if (row[i] != null && row[i].ToString().Trim() != string.Empty)
                    {
                        isEmptyColumn = false;
                        break;
                    }
                }
                if (isEmptyColumn)
                    table.Columns.RemoveAt(i);
            }
            return table;
        }
    }
}
