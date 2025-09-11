using System;
using System.Collections.Generic;
using System.Linq;
using Genrev.Web.App.Data.Models.Management;
using Genrev.Web.App.Data.Models.Forecast;
using Genrev.Domain.Data;
using System.Data.SqlClient;
using Genrev.DomainServices.Data;
using Genrev.Domain;
using Genrev.Domain.DataSets;
using System.Data.Entity.Migrations;
using Serilog;
using Genrev.Domain.Companies;

namespace Genrev.Web.App.Data
{
	public class DataService
	{
		private readonly string _connectionString;

		public DataService(string connectionString)
		{
			_connectionString = connectionString;
		}

		#region GENERAL

		public int DefaultYear { get { return _yearProvider.GetDefaultYear(); } }

		public IEnumerable<int> GetDefaultYears()
		{
			return _yearProvider.GetDefaultYears();
		}

		public List<DateTime> GetFiscalYearPeriods(int year)
		{
			return AppService.Current.Account.PrimaryCompany.GetFiscalYear(year).MonthlyPeriods;
		}

		#endregion


		#region FORECAST


		internal ForecastDetailVM GetForecastDetailVM(int personnelID, int year)
		{
			var fiscalYear = AppService.Current.Account.PrimaryCompany.GetFiscalYear(year);
			var model = new ForecastDetailVM
			{
				IsLocked = _context.ForecastLocks.Any(m => m.PersonnelID == personnelID && m.Year == year),
				ForecastListItems = _context.Database.SqlQuery<ForecastDTO>(
					"EXEC dbo.GetForecastData @FiscalYear, @PersonnelID",
					new SqlParameter("@FiscalYear", year),
					new SqlParameter("@PersonnelID", personnelID)).Select(m => m.ToForecastGridItemVM()).ToList()
			};
			return model;
		}


		internal ForecastPlanByYearVM GetForecastPlanyByYearModel(int year, int salespersonID)
		{

			var model = new ForecastPlanByYearVM();

			model.Items = new List<ForecastPlanByYearListItem>();

			var data = AppService.Current.DataContext.GetCallPlanPerYearByAccountTypeByPersonnel(salespersonID, year, DateTime.Now);

			foreach (var d in data)
			{

				model.Items.Add(new ForecastPlanByYearListItem()
				{
					AccountType = d.AccountTypeName,
					AccountTypeCount = d.AccountTypeCount,
					AccountTypeID = d.AccountTypeID,
					GoalCalls = d.GoalCountPerYear,
					PlannedCalls = d.PlannedCalls
				});
			}

			return model;

		}


		internal void ForecastBatchUpdate(List<ForecastGridItemVM> items, int year, int personnelID)
		{
			var fiscalYear = FiscalYear.GetByYear(year, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);
			var existing = items.Where(m => m.CustomerDataID.HasValue).ToList();

			foreach (var e in existing)
			{
				var customerData = _context.CustomerData.Find(e.CustomerDataID.Value);
				customerData.SalesForecast = e.SalesForecast;
				customerData.SalesTarget = e.SalesTarget;
				customerData.CostForecast = CustomerData.GetCost(e.SalesForecast, e.GPPForecast);
				customerData.CostTarget = CustomerData.GetCost(e.SalesTarget, e.GPPTarget);
				customerData.CallsForecast = e.CallsForecast;
				customerData.CallsTarget = e.CallsTarget;
				customerData.Potential = e.Potential;
				customerData.CurrentOpportunity = e.CurrentOpportunity;
				customerData.FutureOpportunity = e.FutureOpportunity;
				customerData.Strategy = e.Strategy;
				customerData.MarketShare = e.MarketShare;
				customerData.AtRisk = e.AtRisk;
				customerData.RiskExplanation = e.RiskExplanation;
				_context.SaveChanges();
			}

			var @new = items.Where(m => !m.CustomerDataID.HasValue).ToList();
			foreach (var e in @new)
			{
				var customerData = new CustomerData();
				customerData.CustomerID = e.CustomerID;
				customerData.PersonnelID = personnelID;
				customerData.Period = e.Period;
				Map(e, customerData);
				_context.CustomerData.Add(customerData);
			}
			_context.SaveChanges();
			AppCache.InvalidateAll();
		}

		private CustomerData Map(ForecastGridItemVM item, CustomerData customerData)
		{
			customerData.SalesForecast = item.SalesForecast;
			customerData.SalesTarget = item.SalesTarget;
			customerData.CostForecast = CustomerData.GetCost(item.SalesForecast, item.GPPForecast);
			customerData.CostTarget = CustomerData.GetCost(item.SalesTarget, item.GPPTarget);
			customerData.CallsForecast = item.CallsForecast;
			customerData.CallsTarget = item.CallsTarget;
			customerData.Potential = item.Potential;
			customerData.CurrentOpportunity = item.CurrentOpportunity;
			customerData.FutureOpportunity = item.FutureOpportunity;
			customerData.Strategy = item.Strategy;
			customerData.MarketShare = item.MarketShare;
			customerData.AtRisk = item.AtRisk;
			customerData.RiskExplanation = item.RiskExplanation;
			return customerData;
		}
		#endregion


		#region FORECAST LOCK
		public IEnumerable<object> GetForecastLocksByYear(int year,int companyId)
		{
			var items = _context.Database.SqlQuery<ForecastLockDTO>(@"
						SELECT 
							@Year AS Year,
							P.ID AS PersonnelID,
							P.PersonLastName AS PersonnelLastName,
							P.PersonFirstName AS PersonnelFirstName,
							CAST((
								SELECT CASE WHEN COUNT(*) > 0 THEN 1 ELSE 0 END
								FROM ForecastLocks AS FL
								WHERE FL.PersonnelID = P.ID AND FL.Year = @Year
							) AS bit) AS IsLocked
						FROM Personnel AS P
						WHERE P.CompanyID = @CompanyID
						ORDER BY P.PersonLastName, P.PersonFirstName
					", new System.Data.SqlClient.SqlParameter("@Year", year),
				   new System.Data.SqlClient.SqlParameter("@CompanyID", companyId))
				.ToList();
			return items;
		}


		public bool AddForecastLock(int personnelID, int year, int companyId)
		{
			// ensure personnel belongs to the company
			if (!_context.Personnel.Any(p => p.ID == personnelID && p.CompanyID == companyId))
			{
				return false;
			}

			if (_context.ForecastLocks.Any(m => m.PersonnelID == personnelID && m.Year == year))
			{
				return false;
			}
			var forecastLock = new ForecastLock
			{
				PersonnelID = personnelID,
				Year = year
			};
			_context.ForecastLocks.Add(forecastLock);
			_context.SaveChanges();
			return true;
		}


		public bool RemoveForecastLock(int personnelID, int year, int companyId)
		{
			// ensure personnel belongs to the company
			if (!_context.Personnel.Any(p => p.ID == personnelID && p.CompanyID == companyId))
			{
				return false;
			}

			var forecastLock = _context.ForecastLocks.SingleOrDefault(m => m.PersonnelID == personnelID && m.Year == year);
			if (forecastLock == null)
			{
				return false;
			}
			_context.ForecastLocks.Remove(forecastLock);
			_context.SaveChanges();
			return true;
		}
		#endregion


		#region MANAGEMENT


		public List<MatrixByCostGridItem> GetMatrixGridItems(int year, bool isDailyManagement = false)
		{
			var context = AppService.Current.DataContext;
			var items = new List<MatrixByCostGridItem>();

			var company = AppService.Current.Account.PrimaryCompany;
			var fiscalYear = company.GetFiscalYear(year);

			var groupedData = from d in context.CustomerData
							  join c in context.Customers on d.CustomerID equals c.ID
							  where c.CompanyID == company.ID
								  && d.Period >= fiscalYear.StartDate
								  && d.Period <= fiscalYear.EndDate
							  group d by new { PeriodGroup = isDailyManagement ? d.Period.Day : d.Period.Month, d.CustomerID, d.PersonnelID, d.ProductID } into g
							  orderby g.Key.PeriodGroup, g.Key.CustomerID, g.Key.PersonnelID, g.Key.ProductID
							  select new
							  {
								  ID = g.FirstOrDefault().ID,
								  GridID = g.FirstOrDefault().ID,
								  Period = g.FirstOrDefault().Period,
								  CustomerID = g.FirstOrDefault().CustomerID,
								  PersonnelID = g.FirstOrDefault().PersonnelID,
								  ProductID = g.FirstOrDefault().ProductID,
								  SalesActualSum = g.Sum(x => x.SalesActual),
								  SalesForecastSum = g.Sum(x => x.SalesForecast),
								  CostActualSum = g.Sum(x => x.CostActual),
								  CostForecastSum = g.Sum(x => x.CostForecast),
								  CostTargetSum = g.Sum(x => x.CostTarget),
								  CallsActualSum = g.Sum(x => x.CallsActual),
								  CallsForecastSum = g.Sum(x => x.CallsForecast),
								  PotentialSum = g.Sum(x => x.Potential),
								  CurrentOpportunitySum = g.Sum(x => x.CurrentOpportunity),
								  FutureOpportunitySum = g.Sum(x => x.FutureOpportunity),
								  CallsTargetSum = g.Sum(x => x.CallsTarget),
								  SalesTargetSum = g.Sum(x => x.SalesTarget)
							  };

			var data = groupedData.ToList();

			foreach (var d in data)
			{
				var item = new MatrixByCostGridItem
				{
					ID = d.ID,
					GridID = d.GridID,
					CustomerID = d.CustomerID,
					PersonnelID = d.PersonnelID,
					Period = new CommonListItems.Period()
					{
						Date = isDailyManagement
					? new DateTime(d.Period.Year, d.Period.Month, d.Period.Day)
					: new DateTime(d.Period.Year, d.Period.Month, 1),
					},
					SalesActual = d.SalesActualSum,
					SalesForecast = d.SalesForecastSum,
					GPPActual = CustomerData.GetGPP(d.SalesActualSum, d.CostActualSum),
					GPPForecast = CustomerData.GetGPP(d.SalesForecastSum, d.CostForecastSum),
					CallsActual = d.CallsActualSum,
					CallsForecast = d.CallsForecastSum,
					CostTarget = d.CostTargetSum,
					Potential = d.PotentialSum,
					CurrentOpportunity = d.CurrentOpportunitySum,
					FutureOpportunity = d.FutureOpportunitySum
				};
				if (AppService.Current.Settings.ProductFeatureEnabled)
				{
					item.ProductID = d.ProductID;
				}
				if (AppService.Current.Settings.TargetFeatureEnabled)
				{
					item.CallsTarget = d.CallsTargetSum;
					item.SalesTarget = d.SalesTargetSum;
					item.GPPTarget = CustomerData.GetGPP(d.SalesTargetSum, d.CostTargetSum);
				}

				items.Add(item);
			}

			return items;
		}
		public List<MatrixByCostGridItem> GetMatrixGridItemsDaily(int year)
		{

			var context = AppService.Current.DataContext;
			var items = new List<MatrixByCostGridItem>();

			var company = AppService.Current.Account.PrimaryCompany;
			var fiscalYear = company.GetFiscalYear(year);

			var q = from d in context.CustomerData
					join c in context.Customers on d.CustomerID equals c.ID
					where c.CompanyID == company.ID
						&& d.Period >= fiscalYear.StartDate
						&& d.Period <= fiscalYear.EndDate
						&& (d.SalesActual == null || d.SalesActual != 0)
						&& (d.CostActual == null || d.CostActual != 0)
						&& (d.CallsActual == null || d.CallsActual != 0)
					orderby d.Period, d.CustomerID, d.PersonnelID, d.ProductID
					select d;

			var data = q.ToList();

			foreach (var d in data)
			{

				var item = new MatrixByCostGridItem();
				item.ID = d.ID;
				item.CallsActual = d.CallsActual;
				item.CallsForecast = d.CallsForecast;

				item.GPPActual = CustomerData.GetGPP(d.SalesActual, d.CostActual);
				item.GPPForecast = CustomerData.GetGPP(d.SalesForecast, d.CostForecast);

				item.CustomerID = d.CustomerID;
				item.GridID = d.ID;
				item.Period = new CommonListItems.Period() { Date = d.Period };
				item.PersonnelID = d.PersonnelID;

				if (AppService.Current.Settings.ProductFeatureEnabled)
				{
					item.ProductID = d.ProductID;
				}

				item.SalesActual = d.SalesActual;
				item.SalesForecast = d.SalesForecast;
				item.Potential = d.Potential;
				item.CurrentOpportunity = d.CurrentOpportunity;
				item.FutureOpportunity = d.FutureOpportunity;

				if (AppService.Current.Settings.TargetFeatureEnabled)
				{
					item.CallsTarget = d.CallsTarget;
					item.GPPTarget = CustomerData.GetGPP(d.SalesTarget, d.CostTarget);
					item.SalesTarget = d.SalesTarget;
				}

				items.Add(item);
			}

			return items;

		}

		internal void MatrixGridBatchDelete(List<int> deleteKeys, bool isFromDailyData = false)
		{
			var context = AppService.Current.DataContext;

			foreach (int id in deleteKeys)
			{
				var d = context.CustomerData.Find(id);

				if (d == null)
				{
					continue;
				}

				if (isFromDailyData)
				{
					d.SalesActual = 0;
					d.CostActual = 0;
					d.CallsActual = 0;
				}
				else
				{
					try
					{
						context.CustomerData.Attach(d);
					}
					catch (Exception e)
					{
						System.Diagnostics.Debug.WriteLine(e.ToString());
						d = context.CustomerData.Find(id);
					}

					context.CustomerData.Remove(d);
				}
			}

			context.SaveChanges();
			AppCache.InvalidateAll();
		}

		internal void MatrixGridBatchInsert(List<MatrixByCostGridItem> insert)
		{

			var context = AppService.Current.DataContext;

			foreach (var item in insert)
			{

				var data = new Domain.DataSets.CustomerData();

				data.CallsActual = item.CallsActual;
				data.CallsForecast = item.CallsForecast;
				data.CallsTarget = item.CallsTarget;

				data.CostActual = item.SalesActual * (1 - item.GPPActual / 100);
				data.CostForecast = item.SalesForecast * (1 - item.GPPForecast / 100);

				data.CustomerID = item.CustomerID;
				data.Period = item.Period.Date;
				data.PersonnelID = item.PersonnelID;

				if (AppService.Current.Settings.ProductFeatureEnabled)
				{
					data.ProductID = item.ProductID;
				}

				data.SalesActual = item.SalesActual;
				data.SalesForecast = item.SalesForecast;
				data.SalesTarget = item.SalesTarget;
				data.Potential = item.Potential;
				data.CurrentOpportunity = item.CurrentOpportunity;
				data.FutureOpportunity = item.FutureOpportunity;

				if (AppService.Current.Settings.TargetFeatureEnabled)
				{
					data.CallsTarget = item.CallsTarget;
					data.CostTarget = CustomerData.GetCost(item.SalesTarget, item.GPPTarget);
					data.SalesTarget = item.SalesTarget;
				}

				context.CustomerData.Add(data);

			}

			context.SaveChanges();
			AppCache.InvalidateAll();

		}

		internal void MatrixGridBatchUpdate(List<MatrixByCostGridItem> update)
		{

			var context = AppService.Current.DataContext;

			foreach (var item in update)
			{

				var data = context.CustomerData.Find(item.GridID);

				if (item.Period != null)
					data.Period = item.Period.Date;
				data.CallsActual = item.CallsActual;
				data.CallsForecast = item.CallsForecast;

				data.CostActual = CustomerData.GetCost(item.SalesActual, item.GPPActual);
				data.CostForecast = CustomerData.GetCost(item.SalesForecast, item.GPPForecast);

				data.CustomerID = item.CustomerID;
				//data.Period = item.Period;    // Period does not changed due to DevEx Grid's setup
				data.PersonnelID = item.PersonnelID;

				if (AppService.Current.Settings.ProductFeatureEnabled)
				{
					data.ProductID = item.ProductID;
				}

				data.SalesActual = item.SalesActual;
				data.SalesForecast = item.SalesForecast;
				data.Potential = item.Potential;
				data.CurrentOpportunity = item.CurrentOpportunity;
				data.FutureOpportunity = item.FutureOpportunity;

				if (AppService.Current.Settings.TargetFeatureEnabled)
				{
					data.CallsTarget = item.CallsTarget;
					data.CostTarget = CustomerData.GetCost(item.SalesTarget, item.GPPTarget);
					data.SalesTarget = item.SalesTarget;
				}

			}

			context.SaveChanges();
			AppCache.InvalidateAll();
		}


		#endregion


		#region IMPORTS


		public List<Dymeng.Validation.ValidationError> ProcessFileImport(ImportType importType, DevExpress.Web.UploadedFile file)
		{
			var errors = new List<Dymeng.Validation.ValidationError>();

			string path = AppService.Current.Settings.FileUploadDirectory;
			string fileName = System.IO.Path.GetRandomFileName().Replace(".", "") + ".csv";
			string fullPath = path + "\\" + fileName;
			fullPath = System.IO.Path.Combine(System.Web.Hosting.HostingEnvironment.MapPath("~"), fullPath);

			file.SaveAs(fullPath);

			var csvImportHelper = new CsvImportHelper(fullPath, importType, AppService.Current.DataContext);
			var importHelper = new ImportCommitHelper(AppService.Current.DataContext);

			try
			{
				errors = csvImportHelper.ImportToStaging();

				if (importType != ImportType.ForecastData)
				{
					try
					{
						importHelper.UpsertStagingToLive(importType, AppService.Current.Account.ID, true);
					}
					catch (SqlException sqlEx)
					{
						var err = new Dymeng.Validation.ValidationError();
						err.ID = -1;

						// Unique constraint violation (SQL Server error codes)
						if (sqlEx.Number == 2601 || sqlEx.Number == 2627)
						{
							string dupInfo = string.Empty;
							var marker = "The duplicate key value is";
							var idx = sqlEx.Message.IndexOf(marker, StringComparison.OrdinalIgnoreCase);
							if (idx >= 0)
							{
								var start = idx + marker.Length;
								var end = sqlEx.Message.IndexOf('.', start);
								if (end < 0) end = sqlEx.Message.Length;
								dupInfo = sqlEx.Message.Substring(start, end - start).Trim();
							}

							err.Message = "Import failed: one or more records already exist with the same Company and Account Type. Account Type must be unique per Company."
										+ (string.IsNullOrWhiteSpace(dupInfo) ? string.Empty : $" Duplicate key: {dupInfo}.");
						}
						// Custom RAISERROR from stored procedure (default 50000 or custom via sp_addmessage)
						else if (sqlEx.Number == 50000 || (sqlEx.Number >= 50001 && sqlEx.Number < 60000))
						{
							// Directly use the message thrown from the stored procedure
							err.Message = sqlEx.Message;
						}
						else
						{
							err.Message = "We're sorry, we ran into a database error while processing the import.";
							if (AppService.Current.Settings.DisplayDetailedErrors)
							{
								err.Message += "\r\n\r\n" + sqlEx.ToString();
							}
						}

						errors.Add(err);
						return errors;
					}
				}

				AppCache.InvalidateAll();
			}
			catch (FormatException e)
			{
				var err = new Dymeng.Validation.ValidationError();
				err.ID = -1;
				err.Message = "There seems to be an issue with the file format. Please verify the CSV format and try again. Contact your administrator if the problem persists.";
				Log.Debug("ProcessFileImport: " + e.Message);
				if (AppService.Current.Settings.DisplayDetailedErrors)
				{
					err.Message += "\r\n\r\n" + e.ToString();
				}

				errors.Add(err);
			}
			catch (Exception e)
			{
				System.Diagnostics.Debug.WriteLine(e.ToString());

				var err = new Dymeng.Validation.ValidationError();
				err.ID = -1;
				err.Message = "We're sorry, we ran into an issue with this request. Our development team has been notified.";
				if (AppService.Current.Settings.DisplayDetailedErrors)
				{
					err.Message += "\r\n\r\n" + e.ToString();
				}

				errors.Add(err);
			}

			return errors;
		}


		public int UpdateDefaultUser(int personnelId, int contextID)
		{
			try
			{
				var query = @"UPDATE Personnel
                      SET SelectedUser = @ContextId
                      WHERE ID = @PersonnelId";

				var result = _context.Database.ExecuteSqlCommand(
					query,
					new SqlParameter("@ContextId", contextID),
					new SqlParameter("@PersonnelId", personnelId)
				);

				return result;
			}
			catch (Exception ex)
			{
				Log.Error("Error updating default user: {Error}", ex.Message);
				return 0;
			}

		}

		public int? GetSelectedUserId(int personnelId)
		{
			try
			{
				var query = @"SELECT SelectedUser 
                      FROM Personnel 
                      WHERE ID = @PersonnelId";

				var selectedUser = _context.Database.SqlQuery<int?>(
					query,
					new SqlParameter("@PersonnelId", personnelId)
				).FirstOrDefault();

				return selectedUser;
			}
			catch (Exception ex)
			{
				Log.Error("Error retrieving SelectedUser: {Error}", ex.Message);
				return null;
			}
		}

		public string GetSelectedUserFullName(int personnelId)
		{
			try
			{
				return _context.Database.SqlQuery<string>(
                    @"SELECT 
						(
							SELECT 
								TRIM(pTemp.PersonFirstName + ' ' + pTemp.PersonLastName)
							FROM Personnel pTemp
							WHERE pTemp.ID = 
								CASE 
									WHEN p1.SelectedUser IS NULL THEN 1 
									ELSE p1.SelectedUser 
								END
						) AS SelectedPerson
					FROM Personnel p1
					WHERE p1.ID = @PersonnelId",
					new SqlParameter("@PersonnelId", personnelId)
				).FirstOrDefault();
			}
			catch (Exception ex)
			{
				Log.Error("Error fetching SelectedUser full name: {Error}", ex.Message);
				return null;
			}
		}



		#endregion

		private Genrev.Data.GenrevContext _context;
		private readonly YearProvider _yearProvider;

		public DataService() : this((Genrev.Data.GenrevContext)null) { }

		public DataService(Genrev.Data.GenrevContext context)
		{
			_context = context ?? new Genrev.Data.GenrevContext();
			_yearProvider = new YearProvider();
		}
	}
}