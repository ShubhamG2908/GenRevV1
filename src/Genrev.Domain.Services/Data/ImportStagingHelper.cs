using Dymeng.Data;
using Dymeng.Validation;
using Genrev.Domain.Data.Staging;
using Genrev.Domain.DataSets;
using Serilog;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity.Core.Common.CommandTrees.ExpressionBuilder;
using System.Globalization;
using System.Linq;

namespace Genrev.DomainServices.Data
{
    public class ImportStagingHelper
    {

        ImportValidationHelper validationHelper;
        Genrev.Data.GenrevContext context;


		public ImportStagingHelper(Genrev.Data.GenrevContext context)
        {
            this.context = context;
            validationHelper = new ImportValidationHelper();
        }
        private static decimal? ParseNullableDecimal(object cell)
        {
            if (cell == null) return null;
            var s = (cell ?? string.Empty).ToString().Trim();
            if (string.IsNullOrWhiteSpace(s)) return null;

            decimal d;
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out d)) return d;

            return null;
        }

        private static double? ParseNullableDouble(object cell)
        {
            if (cell == null) return null;
            var s = (cell ?? string.Empty).ToString().Trim();
            if (string.IsNullOrWhiteSpace(s)) return null;

            double d;
            if (double.TryParse(s, NumberStyles.Any, CultureInfo.CurrentCulture, out d)) return d;

            return null;
        }

        public DataTable GetPersonnelStagingTable()
        {
            var table = new DataTable();

            table.Columns.Add("ID");
            table.Columns.Add("FirstName");
            table.Columns.Add("LastName");

            return table;
        }

        public DataTable GetCustomerTypeStagingTable()
        {

            var table = new DataTable();

            table.Columns.Add("ID");
            table.Columns.Add("Name");

            return table;

        }

        public DataTable GetAccountTypeStagingTable()
        {

            var table = new DataTable();

            table.Columns.Add("ID");
            table.Columns.Add("Name");
            table.Columns.Add("CallsPerMonthGoal");

            return table;

        }

        public DataTable GetIndustryTypeStagingTable()
        {

            var table = new DataTable();

            table.Columns.Add("ID");
            table.Columns.Add("Name");

            return table;

        }
        public DataTable GetAreaOfResponsibilityStagingTable()
        {
            var table = new DataTable();
            table.Columns.Add("ID");
            table.Columns.Add("Name");
            return table;
        }

        public DataTable GetCustomerStagingTable()
        {

            var table = new DataTable();

            table.Columns.Add("ID");
            table.Columns.Add("Name");
            table.Columns.Add("CustomerTypeID");
            table.Columns.Add("AccountTypeID");
            table.Columns.Add("IndustryTypeID");
            table.Columns.Add("Address1");
            table.Columns.Add("Address2");
            table.Columns.Add("City");
            table.Columns.Add("State");
            table.Columns.Add("Country");
            table.Columns.Add("Phone");
            table.Columns.Add("PostalCode");

            return table;

        }

        public DataTable GetMonthlyDataStagingTable()
        {

            var table = new DataTable();

            table.Columns.Add("CustomerID");
            table.Columns.Add("PersonnelID");
            table.Columns.Add("Period");
            table.Columns.Add("ActualSales");
            table.Columns.Add("ActualGPP");
            table.Columns.Add("ActualCalls");

            return table;
        }


        public bool CheckPersonnelStagingTable(DataTable table)
        {
            return table.Columns.Count == 4;
        }

        public bool CheckCustomerTypeStagingTable(DataTable table)
        {
            return table.Columns.Count == 2;
        }

        public bool CheckAccountTypeStagingTable(DataTable table)
        {
            return table.Columns.Count == 3;
        }

        public bool CheckIndustryTypeStagingTable(DataTable table)
        {
            return table.Columns.Count == 2;
        }
        public bool CheckAreaOfResponsibilityStagingTable(DataTable table)
        {
            return table.Columns.Count == 2;
        }

        public bool CheckCustomerStagingTable(DataTable table)
        {
            return table.Columns.Count == 12;
        }

        public bool CheckMonthlyDataStagingTable(DataTable table)
        {
            return table.Columns.Count == 6;
        }

        public List<ValidationError> ImportToPersonnelStaging(DataTable table)
        {

            /* Expected Fields
             * ClientID
             * FirstName
             * LastName
             */

            var errors = validationHelper.ValidatePersonnelDataTable(table);

            if (errors.Count > 0)
            {
                return errors;
            }

            var personnel = new List<PersonnelStaging>();

            try
            {
                foreach (DataRow row in table.Rows)
                {
                    var person = new PersonnelStaging();
                    person.ClientID = row.ToStringValue(0);
                    person.FirstName = row.ToStringValue(1);
                    person.LastName = row.ToStringValue(2);
                    personnel.Add(person);
                }

                context.StagedPersonnel.RemoveRange(context.StagedPersonnel);
                context.StagedPersonnel.AddRange(personnel);
                context.SaveChanges();

            }
            catch (Exception e)
            {
                validationHelper.AddGeneralExceptionError(e, ref errors);
            }

            return errors;
        }

        public List<ValidationError> ImportToAccountTypesStaging(DataTable table)
        {

            /* Expected Fields
             * ClientID
             * TypeName
             * CallsPerMonthGoal
             */

            var errors = validationHelper.ValidateAccountTypesDataTable(table);

            if (errors.Count > 0)
            {
                return errors;
            }

            var accountTypes = new List<AccountTypeStaging>();
            try
            {
				foreach (DataRow row in table.Rows)
                {
                    var accountType = new AccountTypeStaging();
                    accountType.ClientID = row.ToStringValue(0);
                    accountType.Name = row.ToStringValue(1);
                    accountType.CallsPerMonthGoal = row.ToDoubleOrNull(2);
                    accountTypes.Add(accountType);
                }

                context.StagedAccountTypes.RemoveRange(context.StagedAccountTypes);
                context.StagedAccountTypes.AddRange(accountTypes);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                validationHelper.AddGeneralExceptionError(e, ref errors);
            }
            return errors;
        }

        public List<ValidationError> ImportToCustomerTypesStaging(DataTable table)
        {

            /* Expected Fields
             * ClientID
             * TypeName
             */

            var errors = validationHelper.ValidateCustomerTypesDataTable(table);

            if (errors.Count > 0)
            {
                return errors;
            }

            var customerTypes = new List<CustomerTypeStaging>();

            try
            {

                foreach (DataRow row in table.Rows)
                {
                    var customerType = new CustomerTypeStaging();
                    customerType.ClientID = row.ToStringValue(0);
                    customerType.Name = row.ToStringValue(1);
                    customerTypes.Add(customerType);
                }

                context.StagedCustomerTypes.RemoveRange(context.StagedCustomerTypes);
                context.StagedCustomerTypes.AddRange(customerTypes);
                context.SaveChanges();

            }
            catch (Exception e)
            {
                validationHelper.AddGeneralExceptionError(e, ref errors);
            }

            return errors;
        }

        public List<ValidationError> ImportToIndustryTypesStaging(DataTable table)
        {

            /* Expected Fields
             * ClientID
             * TypeName
             */

            var errors = validationHelper.ValidateIndustryTypesDataTable(table);

            if (errors.Count > 0)
            {
                return errors;
            }

            var industryTypes = new List<IndustryTypeStaging>();

            try
            {

                foreach (DataRow row in table.Rows)
                {
                    var industryType = new IndustryTypeStaging();
                    industryType.ClientID = row.ToStringValue(0);
                    industryType.Name = row.ToStringValue(1);
                    industryTypes.Add(industryType);
                }

                context.StagedIndustryTypes.RemoveRange(context.StagedIndustryTypes);
                context.StagedIndustryTypes.AddRange(industryTypes);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                validationHelper.AddGeneralExceptionError(e, ref errors);
            }

            return errors;
        }

        public List<ValidationError> ImportToAreaOfResponsibilityStaging(DataTable table)
        {
            var errors = validationHelper.ValidateAreaOfResponsibilityDataTable(table);

            if (errors.Count > 0)
            {
                return errors;
            }

            var areaOfResponsibilities = new List<AreaOfResponsibilityStaging>();
            try
            {
                foreach (DataRow row in table.Rows)
                {
                    var areaOfResponsibility = new AreaOfResponsibilityStaging();
                    areaOfResponsibility.ClientID = row.ToStringValue(0);
                    areaOfResponsibility.Name = row.ToStringValue(1);
                    areaOfResponsibilities.Add(areaOfResponsibility);
                }

                context.StagedAreaOfResponsibilities.RemoveRange(context.StagedAreaOfResponsibilities);
                context.StagedAreaOfResponsibilities.AddRange(areaOfResponsibilities);
                context.SaveChanges();
            }
            catch (Exception e)
            {
                validationHelper.AddGeneralExceptionError(e, ref errors);
            }

            return errors;
        }

        public List<ValidationError> ImportToCustomersStaging(DataTable table)
        {

            /* Expected Fields
             * ClientID
             * Name
             * CustomertTypeClientID
             * AccountTypeClientID
             * IndustryTypeClientID
             * Address1
             * Address2
             * City
             * State
             * Country
             * Phone
             * PostalCode
             */

            var errors = validationHelper.ValidateCustomersDataTable(table);

            if (errors.Count > 0)
            {
                return errors;
            }

            var customers = new List<CustomerStaging>();

            try
            {

                foreach (DataRow row in table.Rows)
                {
                    var customer = new CustomerStaging();
                    customer.ClientID = row.ToStringValue(0);
                    customer.Name = row.ToStringValue(1);
                    customer.CustomerTypeClientID = row.ToStringValue(2);
                    customer.AccountTypeClientID = row.ToStringValue(3);
                    customer.IndustryTypeClientID = row.ToStringValue(4);
                    customer.Address1 = row.ToStringValue(5);
                    customer.Address2 = row.ToStringValue(6);
                    customer.City = row.ToStringValue(7);
                    customer.State = row.ToStringValue(8);
                    customer.Country = row.ToStringValue(9);
                    customer.Phone = row.ToStringValue(10);
                    customer.PostalCode = row.ToStringValue(11);
                    customers.Add(customer);
                }

                context.StagedCustomers.RemoveRange(context.StagedCustomers);
                context.StagedCustomers.AddRange(customers);
                context.SaveChanges();

            }
            catch (Exception e)
            {
                validationHelper.AddGeneralExceptionError(e, ref errors);
            }

            return errors;
        }

        public List<ValidationError> ImportToMonthlyDataStaging(DataTable table)
        {
            var errors = validationHelper.ValidateMonthlyDataTable(table);

            if (errors.Count > 0)
            {
                return errors;
            }
            Log.Debug("ImportToMonthlyDataStaging - date format: " + DateTime.Now);
            var data = new List<MonthlyDataStaging>();
            int rowIndex = 0;
            foreach (DataRow row in table.Rows)
            {
                // No need to skip first row because it will get only data rows not headers
                if (row.ToStringValue(0) == "CustomerID" || row.ToStringValue(0) == "SalespersonID")
                {
                    rowIndex++;
                    continue;
                }

                var d = new MonthlyDataStaging();
                d.CustomerClientID = row.ToStringValue(0);
                d.PersonClientID = row.ToStringValue(1);
                d.Period = ConvertToDateTimeMonthly((row.ToStringValue(2) ?? string.Empty).Trim());
                d.SalesActual = (decimal?)row.ToDoubleOrNull(3);
                d.CostActual = (decimal?)row.ToDoubleOrNull(4);
                d.CallsActual = (double?)row.ToDoubleOrNull(5);

                data.Add(d);
                rowIndex++;
            }

            context.StagedMonthlyData.RemoveRange(context.StagedMonthlyData);
            context.StagedMonthlyData.AddRange(data);
            context.SaveChanges();

            return errors;
        }

		public List<ValidationError> ImportToForecastDataStaging(DataTable table,int companyId)
		{

            var errors = new List<ValidationError>();
			var CustomerList = context.Customers.ToList();
			var PersonnelList = context.Personnel.ToList();
			//var customerDataList = context.CustomerData.ToList();
			var missingCustomers = new HashSet<string>();
			var missingSalespersons = new HashSet<string>();

			string lastCustomer = null;
			string lastSalesperson = null;
			Func<DataRow, string, int, object> getCell = (r, name, idx) =>
            {
                if (r.Table.Columns.Contains(name)) return r[name];
                if (r.Table.Columns.Count > idx) return r[idx];
                return null;
            };

            foreach (DataRow row in table.Rows)
            {
                var firstCust = (getCell(row, "CustomerID", 0) ?? getCell(row, "Customer", 0) ?? string.Empty).ToString();
                if (firstCust == "CustomerID" || firstCust == "SalespersonID")
                    continue;

				if (!string.IsNullOrWhiteSpace(firstCust) && firstCust.ToUpper().Contains("GRAND TOTAL"))
					continue;

				var d = new ForecastDataStaging();
				string customerValue = row.Table.Columns.Contains("Customer")
					? row["Customer"]?.ToString()
					: row.Table.Columns.Contains("CustomerID")
						? row["CustomerID"]?.ToString()
						: string.Empty;

				string salespersonValue = row.Table.Columns.Contains("Salesperson")
					? row["Salesperson"]?.ToString()
					: row.Table.Columns.Contains("SalespersonID")
						? row["SalespersonID"]?.ToString()
						: string.Empty;

				// If customer column empty, reuse previous customer
				DateTime testDate;

				if (!string.IsNullOrWhiteSpace(customerValue) &&
					!DateTime.TryParse(customerValue, out testDate))
				{
					lastCustomer = customerValue.Trim();
				}

				d.CustomerClientID = lastCustomer;

				if (!string.IsNullOrWhiteSpace(salespersonValue) &&
	            !DateTime.TryParse(salespersonValue, out testDate))
				{
					lastSalesperson = salespersonValue.Trim();
				}

				d.PersonClientID = lastSalesperson;

				d.Period = ConvertToDateTimeForecast((getCell(row, "Period", 2) ?? string.Empty).ToString().Trim());
                if (d.Period == DateTime.MinValue) continue;

                d.SalesForecast = ParseNullableDecimal(getCell(row, "SalesForecast", 3) ?? getCell(row, "Sales Forecast", 3));
                d.SalesTarget = ParseNullableDecimal(getCell(row, "SalesTarget", 4) ?? getCell(row, "Sales Target", 4));

                d.GPPForecast = ParseNullableDecimal(getCell(row, "GPPForecast", 5) ?? getCell(row, "GPP Forecast", 5));
                d.GPPTarget = ParseNullableDecimal(getCell(row, "GPPTarget", 6) ?? getCell(row, "GPP Target", 6));

                d.CallsForecast = ParseNullableDouble(getCell(row, "CallsForecast", 7) ?? getCell(row, "Calls Forecast", 7));
                d.CallsTarget = ParseNullableDouble(getCell(row, "CallsTarget", 8) ?? getCell(row, "Calls Target", 8));

                d.Strategy = (getCell(row, "Strategy", 9) ?? string.Empty).ToString();

                d.Potential = ParseNullableDecimal(getCell(row, "Potential", 10));
                d.CurrentOpportunity = ParseNullableDecimal(getCell(row, "CurrentOpportunity", 11) ?? getCell(row, "Current Opportunity", 11));
                d.FutureOpportunity = ParseNullableDecimal(getCell(row, "FutureOpportunity", 12) ?? getCell(row, "Future Opportunity", 12));

                d.MarketShare = ParseNullableDecimal(getCell(row, "MarketShare", 13) ?? getCell(row, "Market Share", 13));
                d.AtRisk = ParseNullableDecimal(getCell(row, "AtRisk", 14) ?? getCell(row, "At Risk", 14));

                d.RiskExplanation = (getCell(row, "RiskExplanation", 15) ?? getCell(row, "Risk Explanation", 15) ?? string.Empty).ToString();


                var singleCustomer = CustomerList.FirstOrDefault(w => w.Name == d.CustomerClientID && w.CompanyID == companyId);
				if (singleCustomer == null)
				{
					singleCustomer = CustomerList.FirstOrDefault(w => w.Name == d.CustomerClientID);

					if (singleCustomer == null)
					{
						if (!string.IsNullOrWhiteSpace(d.CustomerClientID) && !missingCustomers.Contains(d.CustomerClientID))
						{
							missingCustomers.Add(d.CustomerClientID);

							errors.Add(new ValidationError
							{
								Message = $"Customer '{d.CustomerClientID}' does not exist in the Company."
							});
						}
						else
						{
							errors.Add(new ValidationError
							{
								Message = $"Customer '{d.CustomerClientID}' does not exists in the Company."
							});
						}
					}
				}

				var singlePerson = PersonnelList.FirstOrDefault(w => w.CommonName == d.PersonClientID && w.CompanyID == companyId);
				if (singlePerson == null) 
				{
					var personClientTrim = d.PersonClientID?.Trim();

					singlePerson = PersonnelList.FirstOrDefault(w =>
						((w.CommonName ?? string.Empty).Trim()) == personClientTrim);

					if (!string.IsNullOrWhiteSpace(d.PersonClientID) && !missingSalespersons.Contains(d.PersonClientID))
					{
						missingSalespersons.Add(d.PersonClientID);

						errors.Add(new ValidationError
						{
							Message = $"Salesperson '{d.PersonClientID}' does not exist in the Company."
						});
					}
				}

				CustomerData data = null;
				if (singleCustomer != null && singleCustomer.ID > 0 && singlePerson != null && singlePerson.ID > 0)
				{
					data = context.CustomerData.FirstOrDefault(w =>
						w.CustomerID == singleCustomer.ID &&
						w.PersonnelID == singlePerson.ID &&
						System.Data.Entity.DbFunctions.TruncateTime(w.Period) == System.Data.Entity.DbFunctions.TruncateTime(d.Period));
				}
				if (singleCustomer != null && singleCustomer.ID > 0 && singlePerson != null && singlePerson.ID > 0)
                {
                    var personnelDownline = context.GetDownstreamCustomerIDs(singlePerson.ID,singlePerson.CompanyID).ToList();
                    if (!personnelDownline.Contains(singleCustomer.ID))
                    {
                        errors.Add(new ValidationError() { Message = singleCustomer.ClientID + " is not mapped with " + singlePerson.ClientID });
                        //return errors;
                        continue;
                    }

					bool isStrategyOnly = (getCell(row, "IsStrategyOnly", 16) ?? "").ToString() == "true";

					//	if (data != null && data.ID > 0)
					//	{
					//		if (isStrategyOnly)
					//		{
					//			// ✅ Only update strategy fields if incoming value is non-null/non-empty
					//			// This prevents a Jan import from blanking out values set by a prior March import

					//			if (!string.IsNullOrWhiteSpace(d.Strategy))
					//				data.Strategy = d.Strategy;

					//			if (d.Potential.HasValue)
					//				data.Potential = d.Potential;

					//			if (d.CurrentOpportunity.HasValue)
					//				data.CurrentOpportunity = d.CurrentOpportunity;

					//			if (d.FutureOpportunity.HasValue)
					//				data.FutureOpportunity = d.FutureOpportunity;

					//			if (d.MarketShare.HasValue)
					//				data.MarketShare = d.MarketShare;

					//			if (d.AtRisk.HasValue)
					//				data.AtRisk = d.AtRisk;

					//			if (!string.IsNullOrWhiteSpace(d.RiskExplanation))
					//				data.RiskExplanation = d.RiskExplanation;

					//			context.SaveChanges();
					//		}
					//		else
					//		{
					//			UpdateCustomerData(data, d, singleCustomer.ID, singlePerson.ID, singleCustomer.CompanyID);
					//		}
					//	}
					//	else
					//	{
					//		if (!isStrategyOnly)
					//		{
					//			InsertCustomerData(d, singleCustomer.ID, singlePerson.ID, singleCustomer.CompanyID);
					//		}
					//		else
					//		{
					//			// No existing Jan record — insert with strategy fields only, sales fields left null
					//			var strategyOnlyData = new ForecastDataStaging();
					//			strategyOnlyData.Period = d.Period;
					//			strategyOnlyData.Strategy = d.Strategy;
					//			strategyOnlyData.MarketShare = d.MarketShare;
					//			strategyOnlyData.AtRisk = d.AtRisk;
					//			strategyOnlyData.RiskExplanation = d.RiskExplanation;
					//                        strategyOnlyData.Potential = d.Potential;
					//                        strategyOnlyData.CurrentOpportunity = d.CurrentOpportunity;
					//                        strategyOnlyData.FutureOpportunity = d.FutureOpportunity;
					//                        // SalesForecast, SalesTarget, GPPForecast, GPPTarget, CallsForecast, CallsTarget
					//                        // are intentionally left null

					//                        InsertCustomerData(strategyOnlyData, singleCustomer.ID, singlePerson.ID, singleCustomer.CompanyID);
					//		}
					//	}
					//}
					//        }
					if (data != null && data.ID > 0)
					{
						// ✅ Always use UpdateCustomerData for existing records.
						// UpdateCustomerData null-guards ALL fields, so empty values
						// from any import will never overwrite existing DB values.
						UpdateCustomerData(data, d, singleCustomer.ID, singlePerson.ID, singleCustomer.CompanyID);
					}
					else
					{
						if (!isStrategyOnly)
						{
							// Normal month row — insert all fields as-is
							InsertCustomerData(d, singleCustomer.ID, singlePerson.ID, singleCustomer.CompanyID);
						}
						else
						{
							// Jan strategy-only row, no existing record —
							// insert strategy fields only, leave sales fields null
							var strategyOnlyData = new ForecastDataStaging();
							strategyOnlyData.Period = d.Period;
							strategyOnlyData.Strategy = d.Strategy;
							strategyOnlyData.Potential = d.Potential;
							strategyOnlyData.CurrentOpportunity = d.CurrentOpportunity;
							strategyOnlyData.FutureOpportunity = d.FutureOpportunity;
							strategyOnlyData.MarketShare = d.MarketShare;
							strategyOnlyData.AtRisk = d.AtRisk;
							strategyOnlyData.RiskExplanation = d.RiskExplanation;
							// SalesForecast, SalesTarget, GPPForecast, GPPTarget,
							// CallsForecast, CallsTarget intentionally left null

							InsertCustomerData(strategyOnlyData, singleCustomer.ID, singlePerson.ID, singleCustomer.CompanyID);
						}
					}
				}
			}
			if (errors.Any())
			{
				errors.Insert(0, new ValidationError
				{
					Message = "Import completed with warnings. Some customers or salespersons were not found."
				});
			}
			return errors;
		}
		private void UpdateCustomerData(CustomerData data, ForecastDataStaging obj, int customerId, int personnelId,int companyId)
        {
			data.CustomerID = customerId;
			data.PersonnelID = personnelId;
			data.CompanyID = companyId;
			data.Period = obj.Period;

			// Only update if incoming value is NOT NULL
			if (obj.SalesForecast.HasValue)
				data.SalesForecast = obj.SalesForecast;

			if (obj.SalesTarget.HasValue)
				data.SalesTarget = obj.SalesTarget;

			if (obj.GPPForecast.HasValue)
				data.CostForecast = CustomerData.GetCost(obj.SalesForecast, obj.GPPForecast);

			if (obj.GPPTarget.HasValue)
				data.CostTarget = CustomerData.GetCost(obj.SalesTarget, obj.GPPTarget);

			if (obj.CallsForecast.HasValue)
				data.CallsForecast = obj.CallsForecast;

			if (obj.CallsTarget.HasValue)
				data.CallsTarget = obj.CallsTarget;

			// Only update if incoming value is NOT NULL
			// This prevents overwriting existing DB values

			if (obj.Potential.HasValue)
				data.Potential = obj.Potential;

			if (obj.CurrentOpportunity.HasValue)
				data.CurrentOpportunity = obj.CurrentOpportunity;

			if (obj.FutureOpportunity.HasValue)
				data.FutureOpportunity = obj.FutureOpportunity;

			if (!string.IsNullOrWhiteSpace(obj.Strategy))
				data.Strategy = obj.Strategy;

			if (obj.MarketShare.HasValue)
				data.MarketShare = obj.MarketShare;

			if (obj.AtRisk.HasValue)
				data.AtRisk = obj.AtRisk;

			if (!string.IsNullOrWhiteSpace(obj.RiskExplanation))
				data.RiskExplanation = obj.RiskExplanation;

			context.SaveChanges();
		}
        private void InsertCustomerData(ForecastDataStaging obj, int customerId, int personnelId, int companyId)
        {
            CustomerData customerData = new CustomerData()
            {
                CustomerID = customerId,
                PersonnelID = personnelId,
				CompanyID = companyId,
				Period = obj.Period,
                SalesForecast = obj.SalesForecast,
                SalesTarget = obj.SalesTarget,
                CostForecast = CustomerData.GetCost(obj.SalesForecast, obj.GPPForecast),
                CostTarget = CustomerData.GetCost(obj.SalesTarget, obj.GPPTarget),
                CallsForecast = obj.CallsForecast,
                CallsTarget = obj.CallsTarget,
                Potential = obj.Potential,
                CurrentOpportunity = obj.CurrentOpportunity,
                FutureOpportunity = obj.FutureOpportunity,
                Strategy = obj.Strategy,
                MarketShare = obj.MarketShare,
                AtRisk = obj.AtRisk,
                RiskExplanation = obj.RiskExplanation,
            };
            context.CustomerData.Add(customerData);
            context.SaveChanges();
        }
        private static DateTime ConvertToDateTimeMonthly(string dateValue)
        {
            dateValue = dateValue.Trim();
            string[] formats;

#if DEBUG
            formats = new string[]
            {
            "MM/dd/yyyy HH:mm:ss",
            "MM/dd/yyyy hh:mm:ss tt",
            "yyyy/MM/dd HH:mm:ss",
            "MM/dd/yyyy",
             "M/d/yyyy h:mm:ss tt"
            };
#else
        formats = new string[]
            {            
            "MM/dd/yyyy HH:mm:ss",
            "MM-dd-yyyy HH:mm:ss",            
            "yyyy-MM-dd HH:mm:ss",            
            "M/d/yyyy h:mm:ss tt"
            };
#endif
            Log.Debug("dataValue: " + dateValue);
            DateTime parsedDate;
            if (DateTime.TryParseExact(dateValue, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return parsedDate;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
        private static DateTime ConvertToDateTimeForecast(string dateValue)
        {
            dateValue = dateValue.Trim();
            string[] formats;

#if DEBUG
            formats = new string[]
        {
            "MM/dd/yyyy HH:mm:ss",
            "MM/dd/yyyy hh:mm:ss tt",
            "yyyy/MM/dd HH:mm:ss",
            "MM/dd/yyyy",
        };
#else
        formats = new string[]
            {
            "MM/dd/yyyy HH:mm:ss",
            "MM-dd-yyyy HH:mm:ss",
            "dd/MM/yyyy HH:mm:ss",
            "MM/dd/yyyy HH:mm:ss",
            "MM-dd-yyyy HH:mm:ss",            
            "MM-dd-yyyy hh:mm:ss tt",            
            "yyyy-MM-dd HH:mm:ss",
            "MM-dd-yyyy",
            "MM/dd/yyyy",
            };
#endif

            Log.Debug("ConvertToDateTimeForecast - dataValue: " + dateValue);
            DateTime parsedDate;
            if (DateTime.TryParseExact(dateValue, formats, CultureInfo.InvariantCulture, DateTimeStyles.None, out parsedDate))
            {
                return parsedDate;
            }
            else
            {
                return DateTime.MinValue;
            }
        }
    }
}
