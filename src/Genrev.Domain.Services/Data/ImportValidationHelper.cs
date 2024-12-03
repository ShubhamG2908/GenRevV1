using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;

using Dymeng.Validation;
using Dymeng.Data;


namespace Genrev.DomainServices.Data
{

    public enum ImportValidationError
    {
        ClientIDRequired,
        AccountTypeNameRequired,
        ActualCallsMustBeNumeric,
        ActualCallsMustBeZeroOrMore,
        CustomerTypeNameRequired,
        IndustryTypeNameRequired,
        CompanyNameRequired,
        CompanyCodeRequired,
        FiscalMonthEndOutOfRange,
        FiscalMonthEndRequired,
        LastNameRequired,
        FirstNameRequired,
        CustomerNameRequired,
        GeneralException,
        PersonnelIDRequired,
        PeriodRequired,
        CallsPerMonthMustBeNumeric,
        CallsPerMonthMustBeZeroOrMore,
        ActualSalesMustBeNumeric,
        ActualSalesMustBeZeroOrMore,
        ActualGPPMustBeNumeric
    }

    public class ImportValidationHelper
    {
        

        public List<ValidationError> ValidateCompaniesDataTable(DataTable table) {

            var errors = new List<ValidationError>();

            foreach (DataRow row in table.Rows) {

                string s = row.ToStringValue(0);    // clientID

                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.ClientIDRequired, ref errors);
                }

                s = row.ToStringValue(3);   // name
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.CompanyNameRequired, ref errors);
                }

                s = row.ToStringValue(4);   // code
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.CompanyCodeRequired, ref errors);
                }

                s = row.ToStringValue(5);   //fiscal month end
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.FiscalMonthEndRequired, ref errors);
                }

                int i;
                bool parsed = int.TryParse(s, out i);
                if (!parsed || i < 1 || i > 12) {  
                    addError(ImportValidationError.FiscalMonthEndOutOfRange, ref errors);
                }

            }

            return errors;            
        }

        public List<ValidationError> ValidatePersonnelDataTable(DataTable table) {

            var errors = new List<ValidationError>(); 

            foreach (DataRow row in table.Rows) {

                var s = row.ToStringValue(0);   // clientid

                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.ClientIDRequired, ref errors);
                }

                s = row.ToStringValue(1);   // first name
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.FirstNameRequired, ref errors);
                }
                
                s = row.ToStringValue(2);   // last name
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.LastNameRequired, ref errors);
                }
            }

            return errors;
        }

        public List<ValidationError> ValidateAccountTypesDataTable(DataTable table) {

            var errors = new List<ValidationError>();

            foreach (DataRow row in table.Rows) {

                string s = row.ToStringValue(0);    // clientID
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.ClientIDRequired, ref errors);
                }

                s = row.ToStringValue(1);   // name
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.AccountTypeNameRequired, ref errors);
                }

                s = row.ToStringValue(2);   // calls per month goal
                if (!string.IsNullOrWhiteSpace(s)) {
                    int i;
                    bool parsed = int.TryParse(s, out i);
                    if (!parsed) {
                        addError(ImportValidationError.CallsPerMonthMustBeNumeric, ref errors);
                    } else {
                        if (i < 0) {
                            addError(ImportValidationError.CallsPerMonthMustBeZeroOrMore, ref errors);
                        }
                    }
                }
            }
            
            return errors;
        }

        public List<ValidationError> ValidateCustomerTypesDataTable(DataTable table) {

            var errors = new List<ValidationError>();

            foreach (DataRow row in table.Rows) {

                string s = row.ToStringValue(0);    // clientID
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.ClientIDRequired, ref errors);
                }

                s = row.ToStringValue(1);   // Name
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.CustomerTypeNameRequired, ref errors);
                }
                
            }

            return errors;
        }

        public List<ValidationError> ValidateIndustryTypesDataTable(DataTable table) {

            var errors = new List<ValidationError>();

            foreach (DataRow row in table.Rows) {

                string s = row.ToStringValue(0);    // clientID
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.ClientIDRequired, ref errors);
                }

                s = row.ToStringValue(1);   // Name
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.IndustryTypeNameRequired, ref errors);
                }

            }

            return errors;
        }

        public List<ValidationError> ValidateCustomersDataTable(DataTable table) {

            var errors = new List<ValidationError>();

            foreach (DataRow row in table.Rows) {

                string s = row.ToStringValue(0);    // clientID
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.ClientIDRequired, ref errors);
                }

                s = row.ToStringValue(1);   // name
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.CustomerNameRequired, ref errors);
                }
            }

            return errors;
        }
        
        public List<ValidationError> ValidateMonthlyDataTable(DataTable table) {

            var errors = new List<ValidationError>();

            foreach (DataRow row in table.Rows) {

                string s = row.ToStringValue(0);    // clientID
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.ClientIDRequired, ref errors);
                }

                s = row.ToStringValue(1); // personID
                if (string.IsNullOrWhiteSpace(s)) {
                    addError(ImportValidationError.PersonnelIDRequired, ref errors);
                }

                s = row.ToStringValue(2);   // period
                DateTime parsedDateTime;
                bool dateParsed = DateTime.TryParse(s, out parsedDateTime);
                if (!dateParsed) {
                    addError(ImportValidationError.PeriodRequired, ref errors);
                }

                s = row.ToStringValue(3);   // actual sales
                if (!string.IsNullOrWhiteSpace(s)) {
                    decimal d;
                    bool parsed = decimal.TryParse(s, out d);
                    if (!parsed) {
                        addError(ImportValidationError.ActualSalesMustBeNumeric, ref errors);
                    } else {
                        if (d < 0) {
                            addError(ImportValidationError.ActualSalesMustBeZeroOrMore, ref errors);
                        }
                    }
                }

                s = row.ToStringValue(4);   // actual gpp
                if (!string.IsNullOrWhiteSpace(s)) {
                    decimal d;
                    bool parsed = decimal.TryParse(s, out d);
                    if (!parsed) {
                        addError(ImportValidationError.ActualGPPMustBeNumeric, ref errors);
                    } 
                }

                s = row.ToStringValue(5);   // actual calls
                if (!string.IsNullOrWhiteSpace(s)) {
                    int i;
                    bool parsed = int.TryParse(s, out i);
                    if (!parsed) {
                        addError(ImportValidationError.ActualCallsMustBeNumeric, ref errors);
                    } else {
                        if (i < 0) {
                            addError(ImportValidationError.ActualCallsMustBeZeroOrMore, ref errors);
                        }
                    }
                }


            }

            return errors;
        }


        private void addError(ImportValidationError errorType, ref List<ValidationError> targetList) {

            var error = new ValidationError();

            switch (errorType) {
                case ImportValidationError.ClientIDRequired:
                    error.Message = "ID is required.";
                    break;

                case ImportValidationError.CompanyNameRequired:
                    error.Message = "Company Name is required.";
                    break;
                    
                case ImportValidationError.CompanyCodeRequired:
                    error.Message = "Company Code is required.";
                    break;
                case ImportValidationError.FiscalMonthEndOutOfRange:
                case ImportValidationError.FiscalMonthEndRequired:
                    error.Message = "Fiscal Month End must be a number between 1 and 12.";
                    break;

                case ImportValidationError.FirstNameRequired:
                case ImportValidationError.LastNameRequired:
                    error.Message = "First and Last names are required.";
                    break;

                case ImportValidationError.CustomerNameRequired:
                    error.Message = "Customer Name is required.";
                    break;

                case ImportValidationError.CallsPerMonthMustBeNumeric:
                case ImportValidationError.CallsPerMonthMustBeZeroOrMore:
                    error.Message = "Calls per Month Goal must be numeric and must blank, zero or more.";
                    break;
                case ImportValidationError.AccountTypeNameRequired:
                case ImportValidationError.CustomerTypeNameRequired:
                case ImportValidationError.IndustryTypeNameRequired:
                    error.Message = "Name is required.";
                    break;

                case ImportValidationError.PeriodRequired:
                    error.Message = "Period is required and must be a valid date.";
                    break;
                case ImportValidationError.PersonnelIDRequired:
                    error.Message = "Personnel ID is required.";
                    break;
                case ImportValidationError.ActualCallsMustBeNumeric:
                case ImportValidationError.ActualCallsMustBeZeroOrMore:
                    error.Message = "Actual Calls must be numeric and must be blank, zero or more.";
                    break;
                case ImportValidationError.ActualSalesMustBeNumeric:
                case ImportValidationError.ActualSalesMustBeZeroOrMore:
                    error.Message = "Actual Sales must be numeric and must be blank, zero or more.";
                    break;
                case ImportValidationError.ActualGPPMustBeNumeric:
                    error.Message = "Actual GPP must be numeric.";
                    break;

                default:
                    throw new ArgumentOutOfRangeException("The specificed error type is not registered");
            }

            error.ID = (int)errorType;
            
            if (targetList == null) {
                targetList = new List<ValidationError>();
            }

            targetList.Add(error);
        }
        
        internal void AddGeneralExceptionError(Exception e, ref List<ValidationError> errors) {

            var error = new ValidationError();

            error.Exception = e;
            error.ID = (int)ImportValidationError.GeneralException;
            error.Message = "Unknown exception.  Please verify all data integrity and contact your administrator if this problem persists.";

            errors.Add(error);
        }
    }
}
