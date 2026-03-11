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

			var headerValuesToExports = new List<string>()
			{
				 "Salesperson",
				 "Customer Name",
				 "Risk Explanation",
				 "At Risk",
				 "Market Share",
				 "Future Opportunity",
				 "Current Opportunity",
				 "Potential",
				 "Strategy"
			};

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

			var monthRegex = new Regex(
				@"\b(Jan|January|Feb|February|Mar|March|Apr|April|May|Jun|June|Jul|July|Aug|August|Sep|Sept|September|Oct|October|Nov|November|Dec|December)\b",
				RegexOptions.IgnoreCase);

			var salesForecastRegex = new Regex(@"Sales\s*Forecast", RegexOptions.IgnoreCase);
			var salesTargetRegex = new Regex(@"Sales\s*Target", RegexOptions.IgnoreCase);
			var gppForecastRegex = new Regex(@"GPP\s*Forecast", RegexOptions.IgnoreCase);
			var gppTargetRegex = new Regex(@"GPP\s*Target", RegexOptions.IgnoreCase);
			var callsForecastRegex = new Regex(@"Calls\s*Forecast", RegexOptions.IgnoreCase);
			var callsTargetRegex = new Regex(@"Calls\s*Target", RegexOptions.IgnoreCase);

			var columnMap = new Dictionary<string, Dictionary<string, int>>();

			var columnIndexMapper = new Dictionary<int, (string, string)>();
			var headerRow = wide.Rows[2].ItemArray;

			for (int i = 0; i < headerRow.Length; i++)
			{
				var text = headerRow[i].ToString().Trim();
				if (headerValuesToExports.Contains(text))
				{
					columnIndexMapper.Add(i, (text, string.Empty));
				}
				else
				{
					var result = monthRegex.Match(text);
					if (result.Success)
					{
						if (salesForecastRegex.IsMatch(text))
						{
							columnIndexMapper.Add(i, ("SalesForecast", result.Value.ToLower()));
						}
						else if (salesTargetRegex.IsMatch(text))
						{
							columnIndexMapper.Add(i, ("SalesTarget", result.Value.ToLower()));
						}
						else if (gppForecastRegex.IsMatch(text))
						{
							columnIndexMapper.Add(i, ("GppForecast", result.Value.ToLower()));
						}
						else if (gppTargetRegex.IsMatch(text))
						{
							columnIndexMapper.Add(i, ("GppTarget", result.Value.ToLower()));
						}
						else if (callsForecastRegex.IsMatch(text))
						{
							columnIndexMapper.Add(i, ("CallsForecast", result.Value.ToLower()));
						}
						else if (callsTargetRegex.IsMatch(text))
						{
							columnIndexMapper.Add(i, ("CallsTarget", result.Value.ToLower()));
						}
					}
				}

			}

			columnIndexMapper.GroupBy(x => x.Value.Item2);

			int year = 0;
			int.TryParse(wide.Rows[0].ItemArray[1]?.ToString(), out year);

			if (year == 0)
			{
				year = DateTime.Now.Year;
			}

			var monthGroups = columnIndexMapper
				.Where(x => !string.IsNullOrEmpty(x.Value.Item2))
				.GroupBy(x => x.Value.Item2.ToLower());

			HashSet<string> strategyInsertedCustomers = new HashSet<string>();

			for (int i = 3; i < wide.Rows.Count; i++)
			{
				foreach (var monthGroup in monthGroups)
				{
					DataRow row = table.NewRow();

					string monthText = monthGroup.Key;

					int monthNumber = DateTime.ParseExact(
						monthText.Substring(0, 3),
						"MMM",
						CultureInfo.InvariantCulture
					).Month;

					DateTime periodDate = new DateTime(year, monthNumber, 1);

					row["Period"] = periodDate.ToString("MM/dd/yyyy");

					string customerId = "";
					string strategyValue = "";

					foreach (var map in columnIndexMapper)
					{
						object value = wide.Rows[i][map.Key];

						if (map.Value.Item1.Equals("Salesperson", StringComparison.OrdinalIgnoreCase))
							row["SalespersonID"] = value;

						else if (map.Value.Item1.Equals("Customer Name", StringComparison.OrdinalIgnoreCase))
						{
							customerId = value?.ToString();
							row["CustomerID"] = customerId;
						}

						else if (map.Value.Item1.Equals("Strategy", StringComparison.OrdinalIgnoreCase))
						{
							strategyValue = value?.ToString();
						}

						else if (map.Value.Item1.Equals("Potential", StringComparison.OrdinalIgnoreCase))
							row["Potential"] = CleanNumber(value);

						else if (map.Value.Item1.Equals("Current Opportunity", StringComparison.OrdinalIgnoreCase))
							row["CurrentOpportunity"] = CleanNumber(value);

						else if (map.Value.Item1.Equals("Future Opportunity", StringComparison.OrdinalIgnoreCase))
							row["FutureOpportunity"] = CleanNumber(value);

						else if (map.Value.Item1.Equals("Market Share", StringComparison.OrdinalIgnoreCase))
							row["MarketShare"] = CleanNumber(value);

						else if (map.Value.Item1.Equals("At Risk", StringComparison.OrdinalIgnoreCase))
							row["AtRisk"] = CleanNumber(value);

						else if (map.Value.Item1.Equals("Risk Explanation", StringComparison.OrdinalIgnoreCase))
							row["RiskExplanation"] = value;
					}

					if (!string.IsNullOrWhiteSpace(customerId) &&
						!strategyInsertedCustomers.Contains(customerId))
					{
						row["Strategy"] = strategyValue;
						strategyInsertedCustomers.Add(customerId);
					}
					foreach (var col in monthGroup)
					{
						object value = wide.Rows[i][col.Key];

						switch (col.Value.Item1)
						{
							case "SalesForecast":
								row["SalesForecast"] = CleanNumber(value);
								break;

							case "SalesTarget":
								row["SalesTarget"] = CleanNumber(value);
								break;

							case "GppForecast":
								row["GPPForecast"] = CleanNumber(value);
								break;

							case "GppTarget":
								row["GPPTarget"] = CleanNumber(value);
								break;

							case "CallsForecast":
								row["CallsForecast"] = CleanNumber(value);
								break;

							case "CallsTarget":
								row["CallsTarget"] = CleanNumber(value);
								break;
						}
					}

					table.Rows.Add(row);
				}
			}

			string Normalize(string s)
			{
				if (string.IsNullOrWhiteSpace(s)) return "";
				s = s.Replace("\u00A0", " ");
				s = s.Trim();
				if (s.Length > 0 && s[0] == '\uFEFF')
					s = s.Substring(1);
				return s;
			}

			for (int i = 0; i < wide.Columns.Count; i++)
			{
				string col = Normalize(wide.Columns[i].ColumnName);

				var monthMatch = monthRegex.Match(col);
				if (!monthMatch.Success)
					continue;

				string month = monthMatch.Groups[1].Value.Substring(0, 3);

				if (!columnMap.ContainsKey(month))
					columnMap[month] = new Dictionary<string, int>();

				if (salesForecastRegex.IsMatch(col))
					columnMap[month]["SalesForecast"] = i;

				else if (salesTargetRegex.IsMatch(col))
					columnMap[month]["SalesTarget"] = i;

				else if (gppForecastRegex.IsMatch(col))
					columnMap[month]["GPPForecast"] = i;

				else if (gppTargetRegex.IsMatch(col))
					columnMap[month]["GPPTarget"] = i;

				else if (callsForecastRegex.IsMatch(col))
					columnMap[month]["CallsForecast"] = i;

				else if (callsTargetRegex.IsMatch(col))
					columnMap[month]["CallsTarget"] = i;
			}


			Func<string, string> normKey = s =>
			{
				if (string.IsNullOrEmpty(s)) return "";
				s = s.Replace("\u00A0", " ");
				s = s.Trim().ToLowerInvariant();
				return Regex.Replace(s, "\\s+", "");
			};

			var colIndex = new Dictionary<string, int>();

			for (int i = 0; i < wide.Columns.Count; i++)
			{
				var key = normKey(wide.Columns[i].ColumnName);
				if (!colIndex.ContainsKey(key))
					colIndex[key] = i;
			}

			int FindIndex(params string[] names)
			{
				foreach (var name in names)
				{
					var key = normKey(name);

					if (colIndex.ContainsKey(key))
						return colIndex[key];

					var found = colIndex.Keys.FirstOrDefault(k => k.Contains(key));
					if (found != null)
						return colIndex[found];
				}
				return -1;
			}

			int salespersonIdx = FindIndex("salesperson", "personnel");
			int customerIdx = FindIndex("customername", "customer");

			int strategyIdx = FindIndex("strategy");
			int potentialIdx = FindIndex("potential");
			int currentOppIdx = FindIndex("currentopportunity");
			int futureOppIdx = FindIndex("futureopportunity");
			int marketShareIdx = FindIndex("marketshare");
			int atRiskIdx = FindIndex("atrisk");
			int riskExpIdx = FindIndex("riskexplanation");

			foreach (DataRow wr in wide.Rows)
			{
				string salesperson = salespersonIdx >= 0 ? wr[salespersonIdx]?.ToString()?.Trim() : null;
				string customer = customerIdx >= 0 ? wr[customerIdx]?.ToString()?.Trim() : null;

				if (string.IsNullOrWhiteSpace(salesperson))
					continue;

				if (string.Equals(customer, "GRAND TOTALS", StringComparison.OrdinalIgnoreCase))
					break;

				foreach (var month in columnMap.Keys.OrderBy(m =>
						 DateTime.ParseExact(m, "MMM", CultureInfo.InvariantCulture).Month))
				{
					DataRow row = table.NewRow();

					int monthNumber = DateTime.ParseExact(month, "MMM", CultureInfo.InvariantCulture).Month;

					row["CustomerID"] = customer;
					row["SalespersonID"] = salesperson;
					row["Period"] = new DateTime(year, monthNumber, 1).ToString("M/d/yyyy");

					if (columnMap[month].ContainsKey("SalesForecast"))
						row["SalesForecast"] = CleanNumber(wr[columnMap[month]["SalesForecast"]]);

					if (columnMap[month].ContainsKey("SalesTarget"))
						row["SalesTarget"] = CleanNumber(wr[columnMap[month]["SalesTarget"]]);

					if (columnMap[month].ContainsKey("GPPForecast"))
						row["GPPForecast"] = CleanNumber(wr[columnMap[month]["GPPForecast"]]);

					if (columnMap[month].ContainsKey("GPPTarget"))
						row["GPPTarget"] = CleanNumber(wr[columnMap[month]["GPPTarget"]]);

					if (columnMap[month].ContainsKey("CallsForecast"))
						row["CallsForecast"] = CleanNumber(wr[columnMap[month]["CallsForecast"]]);

					if (columnMap[month].ContainsKey("CallsTarget"))
						row["CallsTarget"] = CleanNumber(wr[columnMap[month]["CallsTarget"]]);

					if (strategyIdx >= 0)
						row["Strategy"] = wr[strategyIdx];

					if (potentialIdx >= 0)
						row["Potential"] = CleanNumber(wr[potentialIdx]);

					if (currentOppIdx >= 0)
						row["CurrentOpportunity"] = CleanNumber(wr[currentOppIdx]);

					if (futureOppIdx >= 0)
						row["FutureOpportunity"] = CleanNumber(wr[futureOppIdx]);

					if (marketShareIdx >= 0)
						row["MarketShare"] = CleanNumber(wr[marketShareIdx]);

					if (atRiskIdx >= 0)
						row["AtRisk"] = CleanNumber(wr[atRiskIdx]);

					if (riskExpIdx >= 0)
						row["RiskExplanation"] = wr[riskExpIdx]?.ToString();

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

		private void SanitizeForecastTable(DataTable table)
		{
			if (table == null) return;

			for (int r = 1; r < table.Rows.Count; r++)
			{
				var row = table.Rows[r];

				string period = row[2]?.ToString()?.Trim();

				DateTime dt;

				if (!DateTime.TryParse(period, out dt))
				{
					row[2] = new DateTime(DateTime.Now.Year, 1, 1)
								.ToString("MM/dd/yyyy");
				}
				else
				{
					row[2] = dt.ToString("MM/dd/yyyy");
				}

				int[] numericCols = { 3, 4, 5, 6, 7, 8, 10, 11, 12, 13, 14 };

				foreach (int c in numericCols)
				{
					string s = row[c]?.ToString()?.Trim();

					if (string.IsNullOrWhiteSpace(s))
					{
						row[c] = "";
						continue;
					}

					decimal d;

					if (!decimal.TryParse(s, out d))
					{
						row[c] = "";
					}
				}
			}
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
						SanitizeForecastTable(table);
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
