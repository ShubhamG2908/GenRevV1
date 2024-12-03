using System;
using System.Collections.Generic;
using Dymeng.Validation;
using System.Data;
using Dymeng.Data;
using Genrev.Domain.Data.Staging;

namespace Genrev.DomainServices.Data
{
    public class ImportStagingHelper
    {

        ImportValidationHelper validationHelper;
        Genrev.Data.GenrevContext context;


        public ImportStagingHelper(Genrev.Data.GenrevContext context) {
            this.context = context;
            validationHelper = new ImportValidationHelper();
        }




        public DataTable GetPersonnelStagingTable() {

            var table = new DataTable();

            table.Columns.Add("ID");
            table.Columns.Add("FirstName");
            table.Columns.Add("LastName");

            return table;
        }

        public DataTable GetCustomerTypeStagingTable() {

            var table = new DataTable();

            table.Columns.Add("ID");
            table.Columns.Add("Name");

            return table;

        }

        public DataTable GetAccountTypeStagingTable() {

            var table = new DataTable();

            table.Columns.Add("ID");
            table.Columns.Add("Name");
            table.Columns.Add("CallsPerMonthGoal");

            return table;

        }

        public DataTable GetIndustryTypeStagingTable() {

            var table = new DataTable();

            table.Columns.Add("ID");
            table.Columns.Add("Name");

            return table;

        }

        public DataTable GetCustomerStagingTable() {

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

        public DataTable GetMonthlyDataStagingTable() {

            var table = new DataTable();

            table.Columns.Add("CustomerID");
            table.Columns.Add("PersonnelID");
            table.Columns.Add("Period");
            table.Columns.Add("ActualSales");
            table.Columns.Add("ActualGPP");
            table.Columns.Add("ActualCalls");

            return table;
        }


        public bool CheckPersonnelStagingTable(DataTable table) {
            return table.Columns.Count == 4;
        }

        public bool CheckCustomerTypeStagingTable(DataTable table) {
            return table.Columns.Count == 2;
        }

        public bool CheckAccountTypeStagingTable(DataTable table) {
            return table.Columns.Count == 3;
        }

        public bool CheckIndustryTypeStagingTable(DataTable table) {
            return table.Columns.Count == 2;
        }

        public bool CheckCustomerStagingTable(DataTable table) {
            return table.Columns.Count == 12;
        }

        public bool CheckMonthlyDataStagingTable(DataTable table) {
            return table.Columns.Count == 6;
        }






        public List<ValidationError> ImportToPersonnelStaging(DataTable table) {

            /* Expected Fields
             * ClientID
             * FirstName
             * LastName
             */

            var errors = validationHelper.ValidatePersonnelDataTable(table);

            if (errors.Count > 0) {
                return errors;
            }

            var personnel = new List<PersonnelStaging>();

            try {
                foreach (DataRow row in table.Rows) {
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
            catch (Exception e) {
                validationHelper.AddGeneralExceptionError(e, ref errors);
            }

            return errors;
        }

        public List<ValidationError> ImportToAccountTypesStaging(DataTable table) {

            /* Expected Fields
             * ClientID
             * TypeName
             * CallsPerMonthGoal
             */

            var errors = validationHelper.ValidateAccountTypesDataTable(table);

            if (errors.Count > 0) {
                return errors;
            }

            var accountTypes = new List<AccountTypeStaging>();
            try {


                foreach (DataRow row in table.Rows) {
                    var accountType = new AccountTypeStaging();
                    accountType.ClientID = row.ToStringValue(0);
                    accountType.Name = row.ToStringValue(1);
                    accountType.CallsPerMonthGoal = row.ToIntOrNull(2);
                    accountTypes.Add(accountType);
                }

                context.StagedAccountTypes.RemoveRange(context.StagedAccountTypes);
                context.StagedAccountTypes.AddRange(accountTypes);
                context.SaveChanges();
            }
            catch (Exception e) {
                validationHelper.AddGeneralExceptionError(e, ref errors);
            }
            return errors;
        }

        public List<ValidationError> ImportToCustomerTypesStaging(DataTable table) {

            /* Expected Fields
             * ClientID
             * TypeName
             */

            var errors = validationHelper.ValidateCustomerTypesDataTable(table);

            if (errors.Count > 0) {
                return errors;
            }

            var customerTypes = new List<CustomerTypeStaging>();

            try {

                foreach (DataRow row in table.Rows) {
                    var customerType = new CustomerTypeStaging();
                    customerType.ClientID = row.ToStringValue(0);
                    customerType.Name = row.ToStringValue(1);
                    customerTypes.Add(customerType);
                }

                context.StagedCustomerTypes.RemoveRange(context.StagedCustomerTypes);
                context.StagedCustomerTypes.AddRange(customerTypes);
                context.SaveChanges();

            }
            catch (Exception e) {
                validationHelper.AddGeneralExceptionError(e, ref errors);
            }

            return errors;
        }

        public List<ValidationError> ImportToIndustryTypesStaging(DataTable table) {

            /* Expected Fields
             * ClientID
             * TypeName
             */

            var errors = validationHelper.ValidateIndustryTypesDataTable(table);

            if (errors.Count > 0) {
                return errors;
            }

            var industryTypes = new List<IndustryTypeStaging>();

            try {

                foreach (DataRow row in table.Rows) {
                    var industryType = new IndustryTypeStaging();
                    industryType.ClientID = row.ToStringValue(0);
                    industryType.Name = row.ToStringValue(1);
                    industryTypes.Add(industryType);
                }

                context.StagedIndustryTypes.RemoveRange(context.StagedIndustryTypes);
                context.StagedIndustryTypes.AddRange(industryTypes);
                context.SaveChanges();
            }
            catch (Exception e) {
                validationHelper.AddGeneralExceptionError(e, ref errors);
            }

            return errors;
        }

        public List<ValidationError> ImportToCustomersStaging(DataTable table) {

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

            if (errors.Count > 0) {
                return errors;
            }

            var customers = new List<CustomerStaging>();

            try {

                foreach (DataRow row in table.Rows) {
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
            catch (Exception e) {
                validationHelper.AddGeneralExceptionError(e, ref errors);
            }

            return errors;
        }

        public List<ValidationError> ImportToMonthlyDataStaging(DataTable table) {

            /* Expected Fields
             * CustomerClientID (required)
             * PersonnelClientID (required)
             * ProductClientID
             * SalesActual
             * SalesTarget
             * GPPActual
             * GPPTarget
             * CallsActual
             * CallsTarget
             * Potential
             * CurrentOpportunity
             * FutureOpportunity
             */


            // TODO: change this to the correct validation
            var errors = validationHelper.ValidateMonthlyDataTable(table);

            if (errors.Count > 0) {
                return errors;
            }

            var data = new List<MonthlyDataStaging>();

            foreach (DataRow row in table.Rows) {

                var d = new MonthlyDataStaging();

                d.CustomerClientID = row.ToStringValue(0);
                d.PersonClientID = row.ToStringValue(1);
                //d.ProductClientID = row.ToStringValue(2);
                d.Period = row.ToDateTime(2);

                d.SalesActual = (decimal?)row.ToDoubleOrNull(3);

                //d.SalesTarget = (decimal?)row.ToDoubleOrNull(4);
                d.CostActual = (decimal?)row.ToDoubleOrNull(4);

                //d.CostTarget = (decimal?)row.ToDoubleOrNull(6);
                d.CallsActual = (decimal?)row.ToDoubleOrNull(5);
                //d.CallsTarget = (decimal?)row.ToDoubleOrNull(8);
                //d.Potential = (decimal?)row.ToDoubleOrNull(9);
                //d.CurrentOpportunity = (decimal?)row.ToDoubleOrNull(10);
                //d.FutureOpportunity = (decimal?)row.ToDoubleOrNull(11);

                data.Add(d);
            }

            context.StagedMonthlyData.RemoveRange(context.StagedMonthlyData);
            context.StagedMonthlyData.AddRange(data);
            context.SaveChanges();

            return errors;
        }



    }
}
