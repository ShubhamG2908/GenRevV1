using Dymeng.Validation;
using Genrev.Domain.Data;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OleDb;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text.RegularExpressions;


namespace Genrev.DomainServices.Data
{
	public class CsvImportHelper
	{


		string filePath;
		ImportType importType;
		ImportStagingHelper stagingHelper;

		public CsvImportHelper(string filePath, ImportType importType, Genrev.Data.GenrevContext context)
		{
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
				dataTable.Locale = CultureInfo.InvariantCulture;
				oleDbDataAdapter.Fill(dataTable);
				return dataTable;
			}
			catch (Exception e)
			{
				Log.Debug("LoadCsvToTable: " + e.Message.ToString());
				return null;
			}
			finally
			{
				((IDisposable)(object)oleDbDataAdapter)?.Dispose();
			}
		}
		public DataTable LoadFileToTable(string path, bool hasHeaders)
		{
			var ext = Path.GetExtension(path)?.ToLowerInvariant();

			if (ext == ".xls" || ext == ".xlsx" || ext == ".xlsm")
			{
				try
				{
					string hdr = (hasHeaders ? "Yes" : "No");
					string excelExtProps = ext == ".xls" ? "Excel 8.0" : "Excel 12.0 Xml";
					string connStr = $"Provider=Microsoft.ACE.OLEDB.12.0;Data Source={path};Extended Properties=\"{excelExtProps};HDR={hdr};IMEX=1\"";

					using (var conn = new OleDbConnection(connStr))
					{
						conn.Open();
						var schema = conn.GetOleDbSchemaTable(System.Data.OleDb.OleDbSchemaGuid.Tables, null);
						if (schema == null || schema.Rows.Count == 0)
						{
							return null;
						}

						string sheetName = schema.Rows[0]["TABLE_NAME"].ToString();
						string cmdText = "SELECT * FROM [" + sheetName + "]";
						using (var cmd = new OleDbCommand(cmdText, conn))
						using (var adapter = new OleDbDataAdapter(cmd))
						{
							var dt = new DataTable();
							dt.Locale = CultureInfo.InvariantCulture;
							adapter.Fill(dt);
							return dt;
						}
					}
				}
				catch (Exception ex)
				{
					Log.Error(ex, "LoadFileToTable (Excel) failed");
				}
			}

			return LoadCsvToTable(path, hasHeaders);
		}

		private DataTable ConvertForecasToCsvFormat(DataTable wide)
		{
			// Output table schema
			DataTable table = new DataTable();

			table.Columns.Add("CustomerID");
			table.Columns.Add("SalespersonID");
			table.Columns.Add("Period");
			table.Columns.Add("SalesForecast");
			table.Columns.Add("SalesTarget");
			table.Columns.Add("GPPForecast");
			table.Columns.Add("GPPTarget");
			table.Columns.Add("CallsForecast");
			table.Columns.Add("CallsTarget");
			table.Columns.Add("Strategy");
			table.Columns.Add("Potential");
			table.Columns.Add("CurrentOpportunity");
			table.Columns.Add("FutureOpportunity");
			table.Columns.Add("MarketShare");
			table.Columns.Add("AtRisk");
			table.Columns.Add("RiskExplanation");

			// Regex used to detect month name from header
			var monthRegex = new Regex(
				@"\b(Jan|January|Feb|February|Mar|March|Apr|April|May|Jun|June|Jul|July|Aug|August|Sep|September|Sept|Oct|October|Nov|November|Dec|December)\b",
				RegexOptions.IgnoreCase);

			// Metric identification
			var salesForecastRegex = new Regex(@"Sales\s*Forecast", RegexOptions.IgnoreCase);
			var salesTargetRegex = new Regex(@"Sales\s*Target", RegexOptions.IgnoreCase);
			var gppForecastRegex = new Regex(@"GPP\s*Forecast", RegexOptions.IgnoreCase);
			var gppTargetRegex = new Regex(@"GPP\s*Target", RegexOptions.IgnoreCase);
			var callsForecastRegex = new Regex(@"Calls\s*Forecast", RegexOptions.IgnoreCase);
			var callsTargetRegex = new Regex(@"Calls\s*Target", RegexOptions.IgnoreCase);

			// Map column index → (MetricName , Month)
			var columnIndexMapper = new Dictionary<int, (string Metric, string Month)>();

			// HEADER ROW from Excel (client requirement)
			var headerRow = wide.Rows[2].ItemArray;

			for (int i = 0; i < headerRow.Length; i++)
			{
				var text = headerRow[i]?.ToString()?.Trim();

				if (string.IsNullOrWhiteSpace(text))
					continue;

				// Static columns
				if (text.Equals("Salesperson", StringComparison.OrdinalIgnoreCase))
					columnIndexMapper.Add(i, ("Salesperson", ""));

				else if (text.Equals("Customer Name", StringComparison.OrdinalIgnoreCase))
					columnIndexMapper.Add(i, ("Customer", ""));

				else if (text.Equals("Strategy", StringComparison.OrdinalIgnoreCase))
					columnIndexMapper.Add(i, ("Strategy", ""));

				else if (text.Equals("Potential", StringComparison.OrdinalIgnoreCase))
					columnIndexMapper.Add(i, ("Potential", ""));

				else if (text.Equals("Current Opportunity", StringComparison.OrdinalIgnoreCase))
					columnIndexMapper.Add(i, ("CurrentOpportunity", ""));

				else if (text.Equals("Future Opportunity", StringComparison.OrdinalIgnoreCase))
					columnIndexMapper.Add(i, ("FutureOpportunity", ""));

				else if (text.Equals("Market Share", StringComparison.OrdinalIgnoreCase))
					columnIndexMapper.Add(i, ("MarketShare", ""));

				else if (text.Equals("At Risk", StringComparison.OrdinalIgnoreCase))
					columnIndexMapper.Add(i, ("AtRisk", ""));

				else if (text.Equals("Risk Explanation", StringComparison.OrdinalIgnoreCase))
					columnIndexMapper.Add(i, ("RiskExplanation", ""));

				// Month based columns
				else
				{
					var match = monthRegex.Match(text);

					if (!match.Success)
						continue;

					var month = match.Value.ToLower();

					if (salesForecastRegex.IsMatch(text))
						columnIndexMapper.Add(i, ("SalesForecast", month));

					else if (salesTargetRegex.IsMatch(text))
						columnIndexMapper.Add(i, ("SalesTarget", month));

					else if (gppForecastRegex.IsMatch(text))
						columnIndexMapper.Add(i, ("GPPForecast", month));

					else if (gppTargetRegex.IsMatch(text))
						columnIndexMapper.Add(i, ("GPPTarget", month));

					else if (callsForecastRegex.IsMatch(text))
						columnIndexMapper.Add(i, ("CallsForecast", month));

					else if (callsTargetRegex.IsMatch(text))
						columnIndexMapper.Add(i, ("CallsTarget", month));
				}
			}

			// Determine year from Excel header
			int year = 0;
			int.TryParse(wide.Rows[0][1]?.ToString(), out year);

			if (year == 0)
				year = DateTime.Now.Year;

			// Group columns by month
			var monthGroups = columnIndexMapper
				.Where(x => !string.IsNullOrEmpty(x.Value.Month))
				.GroupBy(x => x.Value.Month);

			var deafultGroup = columnIndexMapper.Where(x => string.IsNullOrEmpty(x.Value.Month)).ToList();
			// Ensure strategy columns inserted only once per customer
			HashSet<string> strategyInsertedCustomers = new HashSet<string>();

			for (int i = 3; i < wide.Rows.Count; i++)
			{
				bool isEmptyRow = true;
				bool isStrategyValueSet = false;
				// Skip GRAND TOTALS row
				var customerName = wide.Rows[i][deafultGroup.First(x => x.Value.Metric == "Customer").Key]?.ToString();
				var salesperson = wide.Rows[i][deafultGroup.First(x => x.Value.Metric == "Salesperson").Key]?.ToString();

				if (!string.IsNullOrWhiteSpace(customerName) && customerName.ToUpper().Contains("TOTAL"))
					continue;
				foreach (var monthGroup in monthGroups)
				{
					

					DataRow row = table.NewRow();

					// Extract data for monthly data and update the "row" accordingly
					foreach (var item in monthGroup.Select(x => new { x.Value.Metric, x.Key }).ToList())
					{
						var value = wide.Rows[i][item.Key];
						if (string.IsNullOrWhiteSpace(value.ToString().Trim()) || value.ToString() == "#DIV/0!")
							continue;

						isEmptyRow = false;

						row[item.Metric] = CleanNumber(wide.Rows[i][item.Key]);
					}

					// Skip the month entry
					// if there is no valid record available
					//if (isEmptyRow)
					//	continue;

					if (string.IsNullOrWhiteSpace(customerName) || string.IsNullOrWhiteSpace(salesperson))
						continue;

					int monthNumber = DateTime.ParseExact(
						monthGroup.Key.Substring(0, 3),
						"MMM",
						CultureInfo.InvariantCulture).Month;

					row["Period"] = new DateTime(year, monthNumber, 1).ToString("MM/dd/yyyy");

					row["SalespersonID"] = wide.Rows[i][deafultGroup.First(x => x.Value.Metric == "Salesperson").Key];
					row["CustomerID"] = wide.Rows[i][deafultGroup.First(x => x.Value.Metric == "Customer").Key];


					// Set the default field for the 1st month only.
					// No need to repeate it for any other month
					if (monthGroup.Key.StartsWith("jan", StringComparison.OrdinalIgnoreCase))
					{
						row["Strategy"] = wide.Rows[i][deafultGroup.First(x => x.Value.Metric == "Strategy").Key];
						row["Potential"] = CleanNumber(wide.Rows[i][deafultGroup.First(x => x.Value.Metric == "Potential").Key]);
						row["CurrentOpportunity"] = CleanNumber(wide.Rows[i][deafultGroup.First(x => x.Value.Metric == "CurrentOpportunity").Key]);
						row["FutureOpportunity"] = CleanNumber(wide.Rows[i][deafultGroup.First(x => x.Value.Metric == "FutureOpportunity").Key]);
						row["MarketShare"] = CleanNumber(wide.Rows[i][deafultGroup.First(x => x.Value.Metric == "MarketShare").Key]);
						row["AtRisk"] = CleanNumber(wide.Rows[i][deafultGroup.First(x => x.Value.Metric == "AtRisk").Key]);
						row["RiskExplanation"] = wide.Rows[i][deafultGroup.First(x => x.Value.Metric == "RiskExplanation").Key];

						isStrategyValueSet = true;
					}

					// Remove any incorrect value
					for (int v = 0; v < row.ItemArray.Count(); v++)
					{
						if (row.ItemArray[v].ToString() == "#DIV/0!")
							row.ItemArray[v] = "";
					}

					// Add the row in the table with valid month value
					table.Rows.Add(row);
				}
			}

			return table;
		}

		private string CleanNumber(object value)
		{
			if (value == null)
				return "";

			string s = value.ToString();

			s = s.Replace("$", "")
				 .Replace(",", "")
				 .Replace("%", "")
				 .Trim();

			decimal d;

			if (decimal.TryParse(s, out d))
				return d.ToString(CultureInfo.InvariantCulture);

			return "";
		}

		public List<ValidationError> ImportToStaging(int companyId)
		{

			var errors = new List<ValidationError>();

			if (!File.Exists(filePath))
			{
				throw new FileNotFoundException("File " + filePath + " not found.");
			}

			DataTable table = null;
			try
			{
				if (importType == ImportType.ForecastData)
				{
					table = LoadFileToTable(filePath, true);
				}
				else
				{
					table = LoadCsvToTable(filePath, true);
				}

				if (table == null)
				{
					throw new FormatException("Unable to parse file");
				}

				if (importType == ImportType.ForecastData)
				{
					table = ConvertForecasToCsvFormat(table);
				}

				table = DataTableTrimHelper.RemoveEmptyRows(table);
			}
			catch (Exception e)
			{
				throw new FormatException("Unable to parse file", e);
			}

			switch (importType)
			{
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
				case ImportType.ForecastData:
					try
					{
						errors = stagingHelper.ImportToForecastDataStaging(table,companyId);
					}
					catch (Exception ex)
					{
						var err = new ValidationError();
						err.ID = -1;
						err.Message = "Validation failed while importing forecast data: " + ex.Message;
						err.Exception = ex;
						errors = new List<ValidationError>() { err };
					}
					break;
				case ImportType.AreaOfResponsibility:
					errors = stagingHelper.ImportToAreaOfResponsibilityStaging(table);
					break;
				default:
					throw new ArgumentOutOfRangeException("The specified import type isn't registered");
			}

			return errors;
		}
	}
}
