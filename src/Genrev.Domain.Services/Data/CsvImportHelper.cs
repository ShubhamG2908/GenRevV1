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
				@"\b(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Sept|Oct|Nov|Dec)\b",
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

			// Ensure strategy columns inserted only once per customer
			HashSet<string> strategyInsertedCustomers = new HashSet<string>();

			for (int i = 3; i < wide.Rows.Count; i++)
			{
				foreach (var monthGroup in monthGroups)
				{
					DataRow row = table.NewRow();

					int monthNumber = DateTime.ParseExact(
						monthGroup.Key.Substring(0, 3),
						"MMM",
						CultureInfo.InvariantCulture).Month;

					row["Period"] = new DateTime(year, monthNumber, 1).ToString("MM/dd/yyyy");

					string customerId = "";

					string strategy = "";
					string potential = "";
					string currentOpp = "";
					string futureOpp = "";
					string marketShare = "";
					string atRisk = "";
					string riskExp = "";

					// Read static columns
					foreach (var map in columnIndexMapper)
					{
						object value = wide.Rows[i][map.Key];

						switch (map.Value.Metric)
						{
							case "Salesperson":
								row["SalespersonID"] = value;
								break;

							case "Customer":
								customerId = value?.ToString();
								row["CustomerID"] = customerId;
								break;

							case "Strategy":
								strategy = value?.ToString();
								break;

							case "Potential":
								potential = CleanNumber(value?.ToString());
								break;

							case "CurrentOpportunity":
								currentOpp = CleanNumber(value?.ToString());
								break;

							case "FutureOpportunity":
								futureOpp = CleanNumber(value?.ToString());
								break;

							case "MarketShare":
								marketShare = CleanNumber(value?.ToString());
								break;

							case "AtRisk":
								atRisk = CleanNumber(value?.ToString());
								break;

							case "RiskExplanation":
								riskExp = value?.ToString();
								break;
						}
					}

					// Insert these fields only once per customer
					if (!string.IsNullOrWhiteSpace(customerId) &&
						!strategyInsertedCustomers.Contains(customerId))
					{
						row["Strategy"] = strategy;
						row["Potential"] = potential;
						row["CurrentOpportunity"] = currentOpp;
						row["FutureOpportunity"] = futureOpp;
						row["MarketShare"] = marketShare;
						row["AtRisk"] = atRisk;
						row["RiskExplanation"] = riskExp;

						strategyInsertedCustomers.Add(customerId);
					}

					// Read month metrics
					foreach (var col in monthGroup)
					{
						object value = wide.Rows[i][col.Key];

						row[col.Value.Metric] = CleanNumber(value);
					}

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

		public List<ValidationError> ImportToStaging()
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

				for (int r = 1; r < table.Rows.Count; r++)
				{
					var row = table.Rows[r];
					if (string.IsNullOrWhiteSpace(row[0]?.ToString()))
					{
						for (int c = 0; c < table.Columns.Count; c++)
						{
							var v = (table.Rows[r][c] ?? string.Empty).ToString().Trim();
							if (!string.IsNullOrWhiteSpace(v))
							{
								row[0] = v;
								break;
							}
						}
					}
					if (string.IsNullOrWhiteSpace(row[1]?.ToString()))
					{
						for (int c = 1; c < table.Columns.Count; c++)
						{
							var v = (table.Rows[r][c] ?? string.Empty).ToString().Trim();
							if (!string.IsNullOrWhiteSpace(v))
							{
								row[1] = v;
								break;
							}
						}
					}
					if (string.IsNullOrWhiteSpace(row[2]?.ToString()))
					{
						for (int c = 2; c < table.Columns.Count; c++)
						{
							var v = (table.Rows[r][c] ?? string.Empty).ToString().Trim();
							if (string.IsNullOrWhiteSpace(v)) continue;
							DateTime dt;
							var m = System.Text.RegularExpressions.Regex.Match(v, "^(Jan|Feb|Mar|Apr|May|Jun|Jul|Aug|Sep|Oct|Nov|Dec)([ -]?(\\d{2,4}))?$", System.Text.RegularExpressions.RegexOptions.IgnoreCase);
							if (m.Success)
							{
								int mon = DateTime.ParseExact(m.Groups[1].Value, "MMM", CultureInfo.InvariantCulture).Month;
								int yr = DateTime.Now.Year;
								if (m.Groups[2].Success && !string.IsNullOrWhiteSpace(m.Groups[2].Value))
								{
									int.TryParse(m.Groups[2].Value, out yr);
									if (yr < 100) yr += 2000;
								}
								row[2] = new DateTime(yr, mon, 1).ToString("MM/dd/yyyy");
								break;
							}
							if (DateTime.TryParse(v, out dt))
							{
								row[2] = dt.ToString("MM/dd/yyyy");
								break;
							}
						}
					}
				}

				table = DataTableTrimHelper.RemoveEmptyRows(table);
				//table = DataTableTrimHelper.RemoveEmptyColumns(table);
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
						errors = stagingHelper.ImportToForecastDataStaging(table);
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
