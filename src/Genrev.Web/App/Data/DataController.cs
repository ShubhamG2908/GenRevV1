using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace Genrev.Web.App.Data
{
    public class DataController : Dymeng.Web.Mvc.DevExpress.ContentAreaController
    {

        public ActionResult Index()
        {
            return Management();
        }


        #region FORECAST LOCK
        public ActionResult ForecastLock()
        {
            var model = new Models.Forecast.ForecastLockVM();
            model.AvailableYears = _service.GetDefaultYears();
            model.SelectedYear = _service.DefaultYear;
            return GetView("ForecastLock", model);
        }

        [HttpGet]
        public ActionResult GetForecastLocks(int year)
        {
            return Json(_service.GetForecastLocksByYear(year),JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult AddForecastLock(int personnelID, int year)
        {
            var result = _service.AddForecastLock(personnelID, year);
            return Json(result);
        }

        [HttpPost]
        public ActionResult RemoveForecastLock(int personnelID, int year)
        {
            var result = _service.RemoveForecastLock(personnelID, year);
            return Json(result);
        }
        #endregion



        #region FORECAST



        public ActionResult Forecast(int? year, int? salespersonID)
        {

            var model = new Models.Forecast.ForecastVM();

            model.AvailableYears = _service.GetDefaultYears();
            model.SelectedYear = year.HasValue ? year.Value : _service.DefaultYear;

            model.AvailableSalespersons = CommonListItems.DataService.GetPersonnelCommonList();
            if (salespersonID.HasValue)
            {
                model.SelectedSalesperson = model.AvailableSalespersons.Where(x => x.ID == salespersonID.Value).Single();
            }
            else
            {
                model.SelectedSalesperson = model.AvailableSalespersons.Where(x => x.ID == AppService.Current.Person.PersonID).Single();
            }

            return GetView("Forecast", model);
        }


        public ActionResult ForecastPlanByYear(int year, int salespersonID)
        {

            var model = _service.GetForecastPlanyByYearModel(year, salespersonID);

            return PartialView("ForecastPlanByYearGrid", model);
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ForecastPlanByYearGridCallback()
        {

            int year = int.Parse(Request.Params["year"]);
            int salespersonID = int.Parse(Request.Params["salespersonID"]);

            var model = _service.GetForecastPlanyByYearModel(year, salespersonID);

            return PartialView("ForecastPlanByYearGrid", model);
        }


        public ActionResult ForecastDetailByYear(int year, int personnelID)
        {
            var model = _service.GetForecastDetailVM(personnelID, year);
            return PartialView("ForecastDetailByYearGrid", model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ForecastDetailByYearGridCallback()
        {
            int year = int.Parse(Request.Params["year"]);
            int personnelID = int.Parse(Request.Params["personnelID"]);
            var model = _service.GetForecastDetailVM(personnelID, year);
            return PartialView("ForecastDetailByYearGrid", model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult ForecastDetailByYearBatchEdit(DevExpress.Web.Mvc.MVCxGridViewBatchUpdateValues<Models.Forecast.ForecastGridItemVM, int> updateValues)
        {

            int year = int.Parse(Request.Params["year"]);
            int personnelID = int.Parse(Request.Params["personnelID"]);

            if (updateValues.Update.Count > 0)
            {
                _service.ForecastBatchUpdate(updateValues.Update, year, personnelID);
            }

            return TransferToAction("ForecastDetailByYearGridCallback");
        }




        #endregion



        #region IMPORTING

        [Authorize(Roles = "sysadmin")]
        public ActionResult Import()
        {
            return GetView("Import");
        }


        private string getCompiledValidationMessages(List<Dymeng.Validation.ValidationError> errors)
        {

            if (AppService.Current.Settings.DisplayDetailedErrors)
            {

            }
            else
            {

            }

            var messages = new List<string>();

            foreach (Dymeng.Validation.ValidationError err in errors)
            {

                string msg = err.Message;

                if (AppService.Current.Settings.DisplayDetailedErrors)
                {
                    if (err.Exception != null)
                    {
                        msg += "\r\n\r\n" + err.Exception.ToString();
                    }
                }

                messages.Add(msg);
            }

            return string.Join("<br />", messages.Distinct().ToArray());
        }


        [HttpGet]
        [Authorize(Roles = "sysadmin")]
        public FileResult GetImportTemplate(string templateName)
        {

            try
            {

                string filename = "GRImportTemplate_";

                switch (templateName)
                {
                    case "personnel":
                        filename += "Personnel.csv";
                        break;
                    case "accountTypes":
                        filename += "AccountTypes.csv";
                        break;
                    case "customerTypes":
                        filename += "CustomerTypes.csv";
                        break;
                    case "industryTypes":
                        filename += "Industries.csv";
                        break;
                    case "customers":
                        filename += "Customers.csv";
                        break;
                    case "monthlyData":
                        filename += "MonthlyData.csv";
                        break;
                    default:
                        throw new ArgumentOutOfRangeException("Type not registered");
                }

                var fullpath = Server.MapPath("~/App_Data/ImportTemplates/" + filename);

                byte[] b = System.IO.File.ReadAllBytes(fullpath);

                return File(b, System.Net.Mime.MediaTypeNames.Application.Octet, filename);

            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.ToString());
                throw new InvalidOperationException();
            }


        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "sysadmin")]
        public ActionResult UploadCompanies()
        {

            throw new NotImplementedException();

            //string[] errors;

            //DevExpress.Web.UploadedFile[] files = DevExpress.Web.Mvc.UploadControlExtension.GetUploadedFiles(
            //    "uploadCompanies",
            //    DataUploadValidation.Settings,
            //    out errors,
            //    (sender, e) => {
            //        var validationErrors = _service.ProcessFileImport(Domain.Data.ImportType.Companies, e.UploadedFile);
            //        e.UploadedFile.IsValid = validationErrors.Count == 0 ? true : false;
            //        if (validationErrors.Count > 0) {
            //            e.ErrorText = getCompiledValidationMessages(validationErrors);
            //        }
            //    });

            //return null;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "sysadmin")]
        public ActionResult UploadPersonnel()
        {

            string[] errors;

            DevExpress.Web.UploadedFile[] files = DevExpress.Web.Mvc.UploadControlExtension.GetUploadedFiles(
                "uploadPersonnel",
                DataUploadValidation.Settings,
                out errors,
                (sender, e) =>
                {
                    var validationErrors = _service.ProcessFileImport(Domain.Data.ImportType.Personnel, e.UploadedFile);
                    e.UploadedFile.IsValid = validationErrors.Count == 0 ? true : false;
                    if (validationErrors.Count > 0)
                    {
                        e.ErrorText = getCompiledValidationMessages(validationErrors);
                    }
                });

            return null;
        }



        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "sysadmin")]
        public ActionResult UploadAccountTypes()
        {

            string[] errors;

            DevExpress.Web.UploadedFile[] files = DevExpress.Web.Mvc.UploadControlExtension.GetUploadedFiles(
                "uploadAccountTypes",
                DataUploadValidation.Settings,
                out errors,
                (sender, e) =>
                {
                    var validationErrors = _service.ProcessFileImport(Domain.Data.ImportType.AccountTypes, e.UploadedFile);
                    e.UploadedFile.IsValid = validationErrors.Count == 0 ? true : false;
                    if (validationErrors.Count > 0)
                    {
                        e.ErrorText = getCompiledValidationMessages(validationErrors);
                    }
                });

            return null;
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "sysadmin")]
        public ActionResult UploadCustomerTypes()
        {

            string[] errors;

            DevExpress.Web.UploadedFile[] files = DevExpress.Web.Mvc.UploadControlExtension.GetUploadedFiles(
                "uploadCustomerTypes",
                DataUploadValidation.Settings,
                out errors,
                (sender, e) =>
                {
                    var validationErrors = _service.ProcessFileImport(Domain.Data.ImportType.CustomerTypes, e.UploadedFile);
                    e.UploadedFile.IsValid = validationErrors.Count == 0 ? true : false;
                    if (validationErrors.Count > 0)
                    {
                        e.ErrorText = getCompiledValidationMessages(validationErrors);
                    }
                });

            return null;
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "sysadmin")]
        public ActionResult UploadIndustryTypes()
        {

            string[] errors;

            DevExpress.Web.UploadedFile[] files = DevExpress.Web.Mvc.UploadControlExtension.GetUploadedFiles(
                "uploadIndustryTypes",
                DataUploadValidation.Settings,
                out errors,
                (sender, e) =>
                {
                    var validationErrors = _service.ProcessFileImport(Domain.Data.ImportType.IndustryTypes, e.UploadedFile);
                    e.UploadedFile.IsValid = validationErrors.Count == 0 ? true : false;
                    if (validationErrors.Count > 0)
                    {
                        e.ErrorText = getCompiledValidationMessages(validationErrors);
                    }
                });

            return null;
        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "sysadmin")]
        public ActionResult UploadCustomers()
        {

            string[] errors;

            DevExpress.Web.UploadedFile[] files = DevExpress.Web.Mvc.UploadControlExtension.GetUploadedFiles(
                "uploadCustomers",
                DataUploadValidation.Settings,
                out errors,
                (sender, e) =>
                {
                    var validationErrors = _service.ProcessFileImport(Domain.Data.ImportType.Customers, e.UploadedFile);
                    e.UploadedFile.IsValid = validationErrors.Count == 0 ? true : false;
                    if (validationErrors.Count > 0)
                    {
                        e.ErrorText = getCompiledValidationMessages(validationErrors);
                    }
                });

            return null;
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "sysadmin")]
        public ActionResult UploadMonthlyData()
        {

            string[] errors;

            DevExpress.Web.UploadedFile[] files = DevExpress.Web.Mvc.UploadControlExtension.GetUploadedFiles(
                "uploadMonthlyData",
                DataUploadValidation.Settings,
                out errors,
                (sender, e) =>
                {
                    var validationErrors = _service.ProcessFileImport(Domain.Data.ImportType.MonthlyData, e.UploadedFile);
                    e.UploadedFile.IsValid = validationErrors.Count == 0 ? true : false;
                    if (validationErrors.Count > 0)
                    {
                        e.ErrorText = getCompiledValidationMessages(validationErrors);
                    }
                });

            return null;
        }

        #endregion



        #region MANAGEMENT


        [Authorize(Roles = "sysadmin")]
        public ActionResult Management()
        {
            return GetView("Management");
        }

        [Authorize(Roles = "sysadmin")]
        public ActionResult ManagementMatrix(int? year)
        {

            var model = new Models.Management.MatrixVM();

            model.AvailableYears = _service.GetDefaultYears();
            model.SelectedYear = year.HasValue ? year.Value : _service.DefaultYear;
            model.MatrixGridItems = _service.GetMatrixGridItems(model.SelectedYear);

            model.CustomersList = CommonListItems.DataService.GetCustomerCommonList();
            //model.PersonnelList = CommonListItems.DataService.GetPersonnelCommonList();
            model.PersonnelList = CommonListItems.DataService.GetPersonnalCommonListAll();
            model.ProductsList = CommonListItems.DataService.GetProductCommonList();
            model.FiscalYearPeriodsList = CommonListItems.DataService.GetPeriodCommonList(_service.GetFiscalYearPeriods(model.SelectedYear));

            return PartialView("ManagementMatrix", model);

        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "sysadmin")]
        public ActionResult ManagementMatrixGridCallback()
        {

            int year = Int32.Parse(Request.Params["selectedYear"]);
            string viewModelString = Request.Params["viewMode"];

            var model = new Models.Management.MatrixVM();

            model.AvailableYears = _service.GetDefaultYears();
            model.SelectedYear = year;
            model.MatrixGridItems = _service.GetMatrixGridItems(model.SelectedYear);
            model.ViewMode = viewModelString == "true" ? Models.Management.MatrixVM.ViewModes.GroupedByMonth : Models.Management.MatrixVM.ViewModes.ListAll;

            model.CustomersList = CommonListItems.DataService.GetCustomerCommonList().OrderBy(x => x.Name).ToList();
            //model.PersonnelList = CommonListItems.DataService.GetPersonnelCommonList();
            model.PersonnelList = CommonListItems.DataService.GetPersonnalCommonListAll().OrderBy(x => x.DisplayName).ToList();
            model.ProductsList = CommonListItems.DataService.GetProductCommonList().OrderBy(x => x.SKU).ToList();
            model.FiscalYearPeriodsList = CommonListItems.DataService.GetPeriodCommonList(_service.GetFiscalYearPeriods(model.SelectedYear));

            return PartialView("ManagementMatrixByCostsGrid", model);

        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        [Authorize(Roles = "sysadmin")]
        public ActionResult ManagementMatrixBatchEdit(DevExpress.Web.Mvc.MVCxGridViewBatchUpdateValues<Models.Management.MatrixByCostGridItem, int> updateValues)
        {

            if (updateValues.DeleteKeys.Count > 0)
            {
                _service.MatrixGridBatchDelete(updateValues.DeleteKeys);
            }

            if (updateValues.Insert.Count > 0)
            {
                _service.MatrixGridBatchInsert(updateValues.Insert);
            }

            if (updateValues.Update.Count > 0)
            {
                _service.MatrixGridBatchUpdate(updateValues.Update);
            }


            return TransferToAction("ManagementMatrixGridCallback");

        }


        #endregion


        #region ACUAL MANAGEMENT
        [Authorize(Roles = "sysadmin")]
        public ActionResult Actual()
        {
            return GetView("Actual");
        }

        [Authorize(Roles = "sysadmin")]
        public ActionResult ActualMatrix(int? year)
        {
            return PartialView("ActualMatrix");
        }
        #endregion













        private DataService _service;

        public DataController()
        {
            _service = new DataService();
        }

    }
}