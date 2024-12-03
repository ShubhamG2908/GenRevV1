using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Genrev.Domain.Data;
using Genrev.Domain.Data.Staging;
using System.Data;
using System.Data.OleDb;
using Dymeng.Data;
using Dymeng.Data.Csv;
using Dymeng.Validation;
using System.Globalization;


namespace Genrev.DomainServices.Data
{
    public class CsvImportHelper
    {


        string filePath;
        ImportType importType;        
        ImportStagingHelper stagingHelper;

        public CsvImportHelper(string filePath, ImportType importType, Genrev.Data.GenrevContext context) {
            this.filePath = filePath;
            this.importType = importType;
            this.stagingHelper = new ImportStagingHelper(context);
        }


        public DataTable LoadCsvToTable(string path, bool hasHeaders)
        {
            string text = (hasHeaders ? "Yes" : "No");
            string directoryName = Path.GetDirectoryName(path);
            string fileName = Path.GetFileName(path);
            string cmdText = "SELECT * FROM [" + fileName + "]";
            OleDbConnection connection = new OleDbConnection("Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + directoryName + ";Extended Properties=\"Text;HDR=" + text + "\"");
            OleDbCommand selectCommand = new OleDbCommand(cmdText, connection);
            OleDbDataAdapter oleDbDataAdapter = new OleDbDataAdapter(selectCommand);
            try
            {
                DataTable dataTable = new DataTable();
                dataTable.Locale = CultureInfo.CurrentCulture;
                oleDbDataAdapter.Fill(dataTable);
                return dataTable;
            }
            finally
            {
                ((IDisposable)(object)oleDbDataAdapter)?.Dispose();
            }
        }


        public List<ValidationError> ImportToStaging() {

            var errors = new List<ValidationError>();

            if (!File.Exists(filePath)) {
                throw new FileNotFoundException("File " + filePath + " not found.");
            }
            
            DataTable table = null;

            try {

                table = LoadCsvToTable(filePath, true);

            } catch (Exception e) {
                throw new FormatException("Unable to parse file", e);
            }
            
            switch (importType) {
                case ImportType.Companies:
                    throw new NotImplementedException();
                    //break;
                case ImportType.Personnel:
                    errors = stagingHelper.ImportToPersonnelStaging(table);
                    break;
                case ImportType.AccountTypes:
                    errors = stagingHelper.ImportToAccountTypesStaging(table);
                    break;
                case ImportType.CustomerTypes:
                    errors = stagingHelper.ImportToCustomerTypesStaging(table);
                    break;
                case ImportType.IndustryTypes:
                    errors = stagingHelper.ImportToIndustryTypesStaging(table);
                    break;
                case ImportType.Customers:
                    errors = stagingHelper.ImportToCustomersStaging(table);
                    break;
                case ImportType.MonthlyData:
                    errors = stagingHelper.ImportToMonthlyDataStaging(table);
                    break;
                default:
                    throw new ArgumentOutOfRangeException("The specified import type isn't registered");
            }
            
            return errors;
        }
        
    }
}
