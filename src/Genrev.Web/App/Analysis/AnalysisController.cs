using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Dymeng.Web.Mvc;

namespace Genrev.Web.App.Analysis
{
    public class AnalysisController : Dymeng.Web.Mvc.DevExpress.ContentAreaController
    {

        

        public ActionResult Index() {
            return ActualVsForecast();
        }

        #region DRILLDOWN
        /***********************
         * 
         * DRILLDOWN
         * 
         * ********************/



        public ActionResult Drilldown(int? year) {

            var model = new Models.DrilldownVM();
            model.AvailableYears = _service.GetDefaultYears();
            model.SelectedYear = year.GetValueOrDefault(_service.DefaultYear);

            var fy = Domain.FiscalYear.GetByYear(
                model.SelectedYear, 
                AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            model.Items = _service.GetDrilldownListItems(fy.StartDate, fy.EndDate);

            return GetView("Drilldown", model);

            

        }


        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult DrilldownGridCallback() {

            int year = Int32.Parse(Request.Params["selectedYear"]);
            // devex sends these as "true,C" or "false,U", so we'll grab the first part
            // (someday should refactor this in the drilldown.js that calls this...
            bool showClassification = bool.Parse(Request.Params["showClassification"].Split(',')[0]);
            bool showSales = bool.Parse(Request.Params["showSales"].Split(',')[0]);
            bool showGPD = bool.Parse(Request.Params["showGPD"].Split(',')[0]);
            bool showGPP = bool.Parse(Request.Params["showGPP"].Split(',')[0]);
            bool showCalls = bool.Parse(Request.Params["showCalls"].Split(',')[0]);

            var model = new Models.DrilldownVM();            
            model.AvailableYears = _service.GetDefaultYears();
            model.SelectedYear = year;

            model.ShowColumnGroupClassification = showClassification;
            model.ShowColumnGroupSales = showSales;
            model.ShowColumnGroupGPD = showGPD;
            model.ShowColumnGroupGPP = showGPP;
            model.ShowColumnGroupCalls = showCalls;

            var fy = Domain.FiscalYear.GetByYear(year, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            model.Items = _service.GetDrilldownListItems(fy.StartDate, fy.EndDate);

            return PartialView("DrilldownGrid", model);
        }






        #endregion


        #region HISTORIC
        /***********************
         * 
         * HISTORIC
         * 
         * ********************/

        public ActionResult Historic() {

            var model = new Models.HistoricVM();
            
            model.PersonsFilterList.Insert(0, new CommonListItems.Person() { ID = -1, FirstName = "<All ", LastName = "Salespersons>" });
            model.IndustriesList.Insert(0, new CommonListItems.Industry() { ID = -1, Name = "<All Industries>" });
            model.CustomerTypesList.Insert(0, new CommonListItems.CustomerType() {  ID = -1, Name = "<All Customer Types>"});
            model.AccountTypesList.Insert(0, new CommonListItems.AccountType() { ID = -1, Name = "<All Account Types>" });
            model.ProductsList.Insert(0, new CommonListItems.Product() { ID = -1, SKU = "<All SKUs>" });
            model.CustomersList.Insert(0, new CommonListItems.Customer() { ID = -1, Name = "<All Customers>" });

            model.PersonsFilterList = model.PersonsFilterList.OrderBy(x => x.LastName).ToList();
            model.IndustriesList = model.IndustriesList.OrderBy(x => x.Name).ToList();
            model.CustomerTypesList = model.CustomerTypesList.OrderBy(x => x.Name).ToList();
            model.AccountTypesList = model.AccountTypesList.OrderBy(x => x.Name).ToList();
            model.ProductsList = model.ProductsList.OrderBy(x => x.SKU).ToList();
            model.CustomersList = model.CustomersList.OrderBy(x => x.Name).ToList();

            model.DefaultYearsToShow = model.YearsToShowList.Where(x => x.Number == 4).Single();
            model.DefaultPerson = model.PersonsFilterList.Where(x => x.ID == -1).Single();
            model.DefaultIndustry = model.IndustriesList.Where(x => x.ID == -1).Single();
            model.DefaultCustomerType = model.CustomerTypesList.Where(x => x.ID == -1).Single();
            model.DefaultAccountType = model.AccountTypesList.Where(x => x.ID == -1).Single();
            model.DefaultProduct = model.ProductsList.Where(x => x.ID == -1).Single();
            model.DefaultCustomer = model.CustomersList.Where(x => x.ID == -1).Single();

            return GetView("Historic", model);

        }



        [Route("Analysis/Historic/Page/Salesperson")]
        public ActionResult AnalysisHistoricPageSalesperson(int? salespersonID, int? yearsToShow) {
            return PartialView("HistoricSalespersonPage");
        }

        [Route("Analysis/Historic/Page/Industry")]
        public ActionResult AnalysisHistoricPageIndustry(int? industryID, int? yearsToShow) {
            return PartialView("HistoricIndustryPage");
        }

        [Route("Analysis/Historic/Page/CustomerType")]
        public ActionResult AnalysisHistoricPageCustomerType(int? customerTypeID, int? yearsToShow) {
            return PartialView("HistoricCustomerTypePage");
        }

        [Route("Analysis/Historic/Page/AccountType")]
        public ActionResult AnalysisHistoricPageAccountType(int? accountTypeID, int? yearsToShow) {
            return PartialView("HistoricAccountTypePage");
        }

        [Route("Analysis/Historic/Page/Product")]
        public ActionResult AnalysisHistoricPageProduct(int? productID, int? yearsToShow) {
            if (!AppService.Current.Settings.ProductFeatureEnabled)
            {
                return PartialView("FeatureDisabled");
            }
            return PartialView("HistoricProductPage");
        }

        [Route("Analysis/Historic/Page/Customer")]
        public ActionResult AnalysisHistoricPageCustomer(int? customerID, int? yearsToShow)
        {
            return PartialView("HistoricCustomerPage");
        }

        [Route("Analysis/Historic/ChartData/Sales")]
        public string AnalysisHistoricChartDataSales(int? salesperson, int? years) {
            
            if (salesperson == null || salesperson == -1) {
                salesperson = AppService.Current.ViewContext.PersonID;
            }
            
            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetCurrent(DateTime.Now, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            DateTime startDate = fy.EndDate.AddYears((years.Value) * -1).AddDays(1);
            DateTime endDate = fy.EndDate;
            
            return _service.Charts.GetHistoricSalesJSON(salesperson.Value, startDate, endDate);
        }

        [Route("Analysis/Historic/ChartData/GP")]
        public string AnalysisHistoricChartDataGP(int? salesperson, int? years) {

            if (salesperson == null || salesperson == -1) {
                salesperson = AppService.Current.ViewContext.PersonID;
            }
            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetCurrent(DateTime.Now, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            DateTime startDate = fy.EndDate.AddYears((years.Value) * -1).AddDays(1);
            DateTime endDate = fy.EndDate;

            return _service.Charts.GetHistoricGPJSON(salesperson.Value, startDate, endDate);
        }
        
        [Route("Analysis/Historic/ChartData/Industry/Sales")]
        public string AnalysisHistoricChartDataIndustrySales(int? industry, int? years) {
            
            if (industry == -1) {
                industry = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetCurrent(DateTime.Now, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            DateTime startDate = fy.EndDate.AddYears((years.Value) * -1).AddDays(1);
            DateTime endDate = fy.EndDate;

            return _service.Charts.GetHistoricSalesByIndustryJSON(industry, startDate, endDate);
            
        }

        [Route("Analysis/Historic/ChartData/Industry/GP")]
        public string AnalysisHistoricChartDataIndustryGP(int? industry, int? years) {

            if (industry == -1) {
                industry = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetCurrent(DateTime.Now, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            DateTime startDate = fy.EndDate.AddYears((years.Value) * -1).AddDays(1);
            DateTime endDate = fy.EndDate;

            return _service.Charts.GetHistoricGPByIndustryJSON(industry, startDate, endDate);
        }

        [Route("Analysis/Historic/ChartData/CustomerType/Sales")]
        public string AnalysisHistoricChartDataCustomerTypeSales(int? customerType, int? years) {

            if (customerType == -1) {
                customerType = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetCurrent(DateTime.Now, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            DateTime startDate = fy.EndDate.AddYears((years.Value) * -1).AddDays(1);
            DateTime endDate = fy.EndDate;


            return _service.Charts.GetHistoricSalesByCustomerTypeJSON(customerType, startDate, endDate);

        }

        [Route("Analysis/Historic/ChartData/CustomerType/GP")]
        public string AnalysisHistoricChartDataCustomerTypeGP(int? customerType, int? years) {

            if (customerType == -1) {
                customerType = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetCurrent(DateTime.Now, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            DateTime startDate = fy.EndDate.AddYears((years.Value) * -1).AddDays(1);
            DateTime endDate = fy.EndDate;

            return _service.Charts.GetHistoricGPByCustomerTypeJSON(customerType, startDate, endDate);
        }
        
        [Route("Analysis/Historic/ChartData/AccountType/Sales")]
        public string AnalysisHistoricChartDataAccountTypeSales(int? accountType, int? years) {

            if (accountType == -1) {
                accountType = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetCurrent(DateTime.Now, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            DateTime startDate = fy.EndDate.AddYears((years.Value) * -1).AddDays(1);
            DateTime endDate = fy.EndDate;


            return _service.Charts.GetHistoricSalesByAccountTypeJSON(accountType, startDate, endDate);

        }

        [Route("Analysis/Historic/ChartData/AccountType/GP")]
        public string AnalysisHistoricChartDataAccountTypeGP(int? accountType, int? years) {

            if (accountType == -1) {
                accountType = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetCurrent(DateTime.Now, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            DateTime startDate = fy.EndDate.AddYears((years.Value) * -1).AddDays(1);
            DateTime endDate = fy.EndDate;

            return _service.Charts.GetHistoricGPByAccountTypeJSON(accountType, startDate, endDate);
        }
        
        [Route("Analysis/Historic/ChartData/Product/Sales")]
        public string AnalysisHistoricChartDataProductSales(int? product, int? years) {

            if (product == -1) {
                product = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetCurrent(DateTime.Now, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            DateTime startDate = fy.EndDate.AddYears((years.Value) * -1).AddDays(1);
            DateTime endDate = fy.EndDate;


            return _service.Charts.GetHistoricSalesByProductJSON(product, startDate, endDate);

        }

        [Route("Analysis/Historic/ChartData/Product/GP")]
        public string AnalysisHistoricChartDataProductGP(int? product, int? years) {

            if (product == -1) {
                product = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetCurrent(DateTime.Now, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            DateTime startDate = fy.EndDate.AddYears((years.Value) * -1).AddDays(1);
            DateTime endDate = fy.EndDate;

            return _service.Charts.GetHistoricGPByProductJSON(product, startDate, endDate);
        }

        [Route("Analysis/Historic/ChartData/Customer/Sales")]
        public string AnalysisHistoricChartDataCustomerSales(int? customer, int? years)
        {

            if (customer == -1)
            {
                customer = null;
            }

            if (years == null)
            {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetCurrent(DateTime.Now, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            DateTime startDate = fy.EndDate.AddYears((years.Value) * -1).AddDays(1);
            DateTime endDate = fy.EndDate;


            return _service.Charts.GetHistoricSalesByCustomerJSON(customer, startDate, endDate);

        }

        [Route("Analysis/Historic/ChartData/Customer/GP")]
        public string AnalysisHistoricChartDataCustomerGP(int? customer, int? years)
        {

            if (customer == -1)
            {
                customer = null;
            }

            if (years == null)
            {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetCurrent(DateTime.Now, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            DateTime startDate = fy.EndDate.AddYears((years.Value) * -1).AddDays(1);
            DateTime endDate = fy.EndDate;

            return _service.Charts.GetHistoricGPByCustomerJSON(customer, startDate, endDate);
        }


        #endregion



        #region ACTUAL VS FORECAST
        /***********************
         * 
         * ACTUAL VS FORECAST
         * 
         * ********************/
        public ActionResult ActualVsForecast() {

            var model = new Models.ActualVsForecastVM();

            model.FiscalYearList = CommonListItems.DataService.GetFiscalYearList();
            model.PersonsFilterList.Insert(0, new CommonListItems.Person() { ID = -1, FirstName = "<All ", LastName = "Salespersons>" });
            model.IndustriesList.Insert(0, new CommonListItems.Industry() { ID = -1, Name = "<All Industries>" });
            model.CustomerTypesList.Insert(0, new CommonListItems.CustomerType() { ID = -1, Name = "<All Customer Types>" });
            model.AccountTypesList.Insert(0, new CommonListItems.AccountType() { ID = -1, Name = "<All Account Types>" });
            model.ProductsList.Insert(0, new CommonListItems.Product() { ID = -1, SKU = "<All SKUs>" });
            model.CustomersList.Insert(0, new CommonListItems.Customer() { ID = -1, Name = "<All Customers>" });

            model.PersonsFilterList = model.PersonsFilterList.OrderBy(x => x.LastName).ToList();
            model.IndustriesList = model.IndustriesList.OrderBy(x => x.Name).ToList();
            model.CustomerTypesList = model.CustomerTypesList.OrderBy(x => x.Name).ToList();
            model.AccountTypesList = model.AccountTypesList.OrderBy(x => x.Name).ToList();
            model.ProductsList = model.ProductsList.OrderBy(x => x.SKU).ToList();
            model.CustomersList = model.CustomersList.OrderBy(x => x.Name).ToList();

            model.DefaultFiscalYear = model.FiscalYearList?.Where(x => x.Number == model.FiscalYearList?.Max(y => y.Number)).SingleOrDefault();
            model.DefaultPerson = model.PersonsFilterList.Where(x => x.ID == -1).Single();
            model.DefaultIndustry = model.IndustriesList.Where(x => x.ID == -1).Single();
            model.DefaultCustomerType = model.CustomerTypesList.Where(x => x.ID == -1).Single();
            model.DefaultAccountType = model.AccountTypesList.Where(x => x.ID == -1).Single();
            model.DefaultProduct = model.ProductsList.Where(x => x.ID == -1).Single();
            model.DefaultCustomer = model.CustomersList.Where(x => x.ID == -1).Single();

            return GetView("ActualVsForecast", model);

        }



        [Route("Analysis/ActualVsForecast/Page/Salesperson")]
        public ActionResult AnalysisActualVsForecastPageSalesperson(int? salespersonID, int? yearsToShow) {
            return PartialView("ActualVsForecastSalespersonPage");
        }

        [Route("Analysis/ActualVsForecast/Page/Industry")]
        public ActionResult AnalysisActualVsForecastPageIndustry(int? industryID, int? yearsToShow) {
            return PartialView("ActualVsForecastIndustryPage");
        }

        [Route("Analysis/ActualVsForecast/Page/CustomerType")]
        public ActionResult AnalysisActualVsForecastPageCustomerType(int? customerTypeID, int? yearsToShow) {
            return PartialView("ActualVsForecastCustomerTypePage");
        }

        [Route("Analysis/ActualVsForecast/Page/AccountType")]
        public ActionResult AnalysisActualVsForecastPageAccountType(int? accountTypeID, int? yearsToShow) {
            return PartialView("ActualVsForecastAccountTypePage");
        }

        [Route("Analysis/ActualVsForecast/Page/Product")]
        public ActionResult AnalysisActualVsForecastPageProduct(int? productID, int? yearsToShow) {
            if (!AppService.Current.Settings.ProductFeatureEnabled)
            {
                return PartialView("FeatureDisabled");
            }
            return PartialView("ActualVsForecastProductPage");
        }

        [Route("Analysis/ActualVsForecast/Page/Customer")]
        public ActionResult AnalysisActualVsForecastPageCustomer(int? customerID, int? yearsToShow)
        {
            return PartialView("ActualVsForecastCustomerPage");
        }



        [Route("Analysis/ActualVsForecast/ChartData/Sales")]
        public string AnalysisActualVsForecastChartDataSales(int? salesperson, int? years) {
            
            if (salesperson == null || salesperson == -1) {
                salesperson = AppService.Current.ViewContext.PersonID;
            }
            
            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);
            
            return _service.Charts.GetActualVsForecastSalesJSON(salesperson.Value, fy.StartDate, fy.EndDate);
        }

        [Route("Analysis/ActualVsForecast/ChartData/GP")]
        public string AnalysisActualVsForecastChartDataGP(int? salesperson, int? years) {

            if (salesperson == null || salesperson == -1) {
                salesperson = AppService.Current.ViewContext.PersonID;
            }
            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetActualVsForecastGPJSON(salesperson.Value, fy.StartDate, fy.EndDate);
        }
        
        [Route("Analysis/ActualVsForecast/ChartData/Industry/Sales")]
        public string AnalysisActualVsForecastChartDataIndustrySales(int? industry, int? years) {

            if (industry == -1) {
                industry = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetActualVsForecastSalesByIndustryJSON(industry, fy.StartDate, fy.EndDate);

        }

        [Route("Analysis/ActualVsForecast/ChartData/Industry/GP")]
        public string AnalysisActualVsForecastChartDataIndustryGP(int? industry, int? years) {

            if (industry == -1) {
                industry = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetActualVsForecastGPByIndustryJSON(industry, fy.StartDate, fy.EndDate);
        }
        
        [Route("Analysis/ActualVsForecast/ChartData/CustomerType/Sales")]
        public string AnalysisActualVsForecastChartDataCustomerTypeSales(int? customerType, int? years) {

            if (customerType == -1) {
                customerType = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetActualVsForecastSalesByCustomerTypeJSON(customerType, fy.StartDate, fy.EndDate);

        }

        [Route("Analysis/ActualVsForecast/ChartData/CustomerType/GP")]
        public string AnalysisActualVsForecastChartDataCustomerTypeGP(int? customerType, int? years) {

            if (customerType == -1) {
                customerType = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetActualVsForecastGPByCustomerTypeJSON(customerType, fy.StartDate, fy.EndDate);
        }
        
        [Route("Analysis/ActualVsForecast/ChartData/AccountType/Sales")]
        public string AnalysisActualVsForecastChartDataAccountTypeSales(int? accountType, int? years) {

            if (accountType == -1) {
                accountType = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetActualVsForecastSalesByAccountTypeJSON(accountType, fy.StartDate, fy.EndDate);

        }

        [Route("Analysis/ActualVsForecast/ChartData/AccountType/GP")]
        public string AnalysisActualVsForecastChartDataAccountTypeGP(int? accountType, int? years) {

            if (accountType == -1) {
                accountType = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetActualVsForecastGPByAccountTypeJSON(accountType, fy.StartDate, fy.EndDate);
        }

        [Route("Analysis/ActualVsForecast/ChartData/Product/Sales")]
        public string AnalysisActualVsForecastChartDataProductSales(int? product, int? years) {

            if (product == -1) {
                product = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetActualVsForecastSalesByProductJSON(product, fy.StartDate, fy.EndDate);

        }

        [Route("Analysis/ActualVsForecast/ChartData/Product/GP")]
        public string AnalysisActualVsForecastChartDataProductGP(int? product, int? years) {

            if (product == -1) {
                product = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetActualVsForecastGPByProductJSON(product, fy.StartDate, fy.EndDate);

        }


        [Route("Analysis/ActualVsForecast/ChartData/Customer/Sales")]
        public string AnalysisActualVsForecastChartDataCustomerSales(int? customer, int? years)
        {

            if (customer == -1)
            {
                customer = null;
            }

            if (years == null)
            {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetActualVsForecastSalesByCustomerJSON(customer, fy.StartDate, fy.EndDate);

        }

        [Route("Analysis/ActualVsForecast/ChartData/Customer/GP")]
        public string AnalysisActualVsForecastChartDataCustomerGP(int? customer, int? years)
        {

            if (customer == -1)
            {
                customer = null;
            }

            if (years == null)
            {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetActualVsForecastGPByCustomerJSON(customer, fy.StartDate, fy.EndDate);

        }


        #endregion



        #region OPPORTUNITIES
        /***********************
         * 
         * OPPORTUNITIES
         * 
         * ********************/
        public ActionResult Opportunities() {

            var model = new Models.OpportunitiesVM();

            model.FiscalYearList = CommonListItems.DataService.GetFiscalYearList();
            model.PersonsFilterList.Insert(0, new CommonListItems.Person() { ID = -1, FirstName = "<All ", LastName = "Salespersons>" });
            //model.IndustriesList.Insert(0, new CommonListItems.Industry() { ID = -1, Name = "<All Industries>" });
            //model.CustomerTypesList.Insert(0, new CommonListItems.CustomerType() { ID = -1, Name = "<All Customer Types>" });
            //model.AccountTypesList.Insert(0, new CommonListItems.AccountType() { ID = -1, Name = "<All Account Types>" });

            model.DefaultFiscalYear = model.FiscalYearList?.Where(x => x.Number == model.FiscalYearList?.Max(y => y.Number)).SingleOrDefault();
            model.DefaultPerson = model.PersonsFilterList.Where(x => x.ID == -1).Single();
            //model.DefaultIndustry = model.IndustriesList.Where(x => x.ID == -1).Single();
            //model.DefaultCustomerType = model.CustomerTypesList.Where(x => x.ID == -1).Single();
            //model.DefaultAccountType = model.AccountTypesList.Where(x => x.ID == -1).Single();

            return GetView("Opportunities", model);
        }




        [Route("Analysis/Opportunities/Data/Salesperson")]
        public string OpportunitiesDataSalesperson(int? salespersonID, int? year) {

            if (!salespersonID.HasValue || salespersonID.Value == -1) {
                salespersonID = AppService.Current.ViewContext.PersonID;
            }

            if (!year.HasValue) {
                year = DateTime.Now.Year;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(year.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetOpportunitiesJSONBySalesperson(salespersonID.Value, fy.StartDate, fy.EndDate);


        }

        [Route("Analysis/Opportunities/Data/Industry")]
        public string OpportunitiesDataIndustry(int? salespersonID, int? year) {

            if (!salespersonID.HasValue || salespersonID.Value == -1) {
                salespersonID = AppService.Current.ViewContext.PersonID;
            }

            if (!year.HasValue) {
                year = DateTime.Now.Year;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(year.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetOpportunitiesJSONByIndustry(salespersonID.Value, fy.StartDate, fy.EndDate);

        }

        [Route("Analysis/Opportunities/Data/CustomerType")]
        public string OpportunitiesDataCustomerType(int? salespersonID, int? year) {

            if (!salespersonID.HasValue || salespersonID.Value == -1) {
                salespersonID = AppService.Current.ViewContext.PersonID;
            }

            if (!year.HasValue) {
                year = DateTime.Now.Year;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(year.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetOpportunitiesJSONByCustomerType(salespersonID.Value, fy.StartDate, fy.EndDate);

        }

        [Route("Analysis/Opportunities/Data/AccountType")]
        public string OpportunitiesDataAccountType(int? salespersonID, int? year) {

            if (!salespersonID.HasValue || salespersonID.Value == -1) {
                salespersonID = AppService.Current.ViewContext.PersonID;
            }

            if (!year.HasValue) {
                year = DateTime.Now.Year;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(year.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetOpportunitiesJSONByAccountType(salespersonID.Value, fy.StartDate, fy.EndDate);

        }

        [Route("Analysis/Opportunities/Data/Customer")]
        public string OpportunitiesDataCustomer(int? salespersonID, int? year)
        {

            if (!salespersonID.HasValue || salespersonID.Value == -1)
            {
                salespersonID = AppService.Current.ViewContext.PersonID;
            }

            if (!year.HasValue)
            {
                year = DateTime.Now.Year;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(year.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetOpportunitiesJSONByCustomer(salespersonID.Value, fy.StartDate, fy.EndDate);

        }

        #endregion



        #region SALES CALLS
        /***********************
         * 
         * SALES CALLS
         * 
         * ********************/
        public ActionResult SalesCalls() {

            var model = new Models.SalesCallsVM();

            model.FiscalYearList = CommonListItems.DataService.GetFiscalYearList();
            model.PersonsFilterList.Insert(0, new CommonListItems.Person() { ID = -1, FirstName = "<All ", LastName = "Salespersons>" });
            model.IndustriesList.Insert(0, new CommonListItems.Industry() { ID = -1, Name = "<All Industries>" });
            model.CustomerTypesList.Insert(0, new CommonListItems.CustomerType() { ID = -1, Name = "<All Customer Types>" });
            model.AccountTypesList.Insert(0, new CommonListItems.AccountType() { ID = -1, Name = "<All Account Types>" });
            model.ProductsList.Insert(0, new CommonListItems.Product() { ID = -1, SKU = "<All SKUs>" });
            model.CustomersList.Insert(0, new CommonListItems.Customer() { ID = -1, Name = "<All Customers>" });

            model.PersonsFilterList = model.PersonsFilterList.OrderBy(x => x.LastName).ToList();
            model.IndustriesList = model.IndustriesList.OrderBy(x => x.Name).ToList();
            model.CustomerTypesList = model.CustomerTypesList.OrderBy(x => x.Name).ToList();
            model.AccountTypesList = model.AccountTypesList.OrderBy(x => x.Name).ToList();
            model.ProductsList = model.ProductsList.OrderBy(x => x.SKU).ToList();
            model.CustomersList = model.CustomersList.OrderBy(x => x.Name).ToList();

            model.DefaultFiscalYear = model.FiscalYearList?.Where(x => x.Number == model.FiscalYearList?.Max(y => y.Number)).SingleOrDefault();
            model.DefaultPerson = model.PersonsFilterList.Where(x => x.ID == -1).Single();
            model.DefaultIndustry = model.IndustriesList.Where(x => x.ID == -1).Single();
            model.DefaultCustomerType = model.CustomerTypesList.Where(x => x.ID == -1).Single();
            model.DefaultAccountType = model.AccountTypesList.Where(x => x.ID == -1).Single();
            model.DefaultProduct = model.ProductsList.Where(x => x.ID == -1).Single();
            model.DefaultCustomer = model.CustomersList.Where(x => x.ID == -1).Single();

            return GetView("SalesCalls", model);
        }

        [AcceptVerbs(HttpVerbs.Get | HttpVerbs.Post)]
        public ActionResult AnalysisSalesCallsOverview(int? salespersonID, int? fiscalYear)
        {
            if (salespersonID == null || salespersonID == -1)
            {
                salespersonID = AppService.Current.ViewContext.PersonID;
            }

            if(fiscalYear == null || fiscalYear == -1)
            {
                fiscalYear = DateTime.Now.Year;
            }

            var model = new Models.CallPlanOverviewVM();
            model.Items = _service.Charts.GetCallPlanOverviewListItems(fiscalYear.Value, salespersonID.Value); //_serviceGetCallPlanOverviewListItems(fiscalYear, salespersonID);

            return PartialView("SalesCallPlanOverview", model);

        }




        [Route("Analysis/SalesCalls/Page/Salesperson")]
        public ActionResult AnalysisSalesCallsPageSalesperson(int? salespersonID, int? yearsToShow) {
            return PartialView("SalesCallsSalespersonPage");
        }

        [Route("Analysis/SalesCalls/Page/Industry")]
        public ActionResult AnalysisSalesCallsPageIndustry(int? industryID, int? yearsToShow) {
            return PartialView("SalesCallsIndustryPage");
        }

        [Route("Analysis/SalesCalls/Page/CustomerType")]
        public ActionResult AnalysisSalesCallsPageCustomerType(int? customerTypeID, int? yearsToShow) {
            return PartialView("SalesCallsCustomerTypePage");
        }

        [Route("Analysis/SalesCalls/Page/AccountType")]
        public ActionResult AnalysisSalesCallsPageAccountType(int? accountTypeID, int? yearsToShow) {
            return PartialView("SalesCallsAccountTypePage");
        }

        [Route("Analysis/SalesCalls/Page/Product")]
        public ActionResult AnalysisSalesCallsPageProduct(int? productID, int? yearsToShow) {
            if (!AppService.Current.Settings.ProductFeatureEnabled)
            {
                return PartialView("FeatureDisabled");
            }

            return PartialView("SalesCallsProductPage");
        }

        [Route("Analysis/SalesCalls/Page/Customer")]
        public ActionResult AnalysisSalesCallsPageCustomer(int? customerID, int? yearsToShow)
        {
            return PartialView("SalesCallsCustomerPage");
        }



        [Route("Analysis/SalesCalls/ChartData/Calls")]
        public string AnalysisSalesCallsChartDataCalls(int? salesperson, int? years) {

            if (salesperson == null || salesperson == -1) {
                salesperson = AppService.Current.ViewContext.PersonID;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetSalesCallsJSON(salesperson.Value, fy.StartDate, fy.EndDate);
        }
        
        [Route("Analysis/SalesCalls/ChartData/Industry/Calls")]
        public string AnalysisSalesCallsChartDataIndustryCalls(int? industry, int? years) {

            if (industry == -1) {
                industry = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetSalesCallsByIndustryJSON(industry, fy.StartDate, fy.EndDate);

        }
        
        [Route("Analysis/SalesCalls/ChartData/CustomerType/Calls")]
        public string AnalysisSalesCallsChartDataCustomerTypeCalls(int? customerType, int? years) {

            if (customerType == -1) {
                customerType = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetSalesCallsByCustomerTypeJSON(customerType, fy.StartDate, fy.EndDate);

        }
        
        [Route("Analysis/SalesCalls/ChartData/AccountType/Calls")]
        public string AnalysisSalesCallsChartDataAccountTypeCalls(int? accountType, int? years) {

            if (accountType == -1) {
                accountType = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetSalesCallsByAccountTypeJSON(accountType, fy.StartDate, fy.EndDate);

        }

        [Route("Analysis/SalesCalls/ChartData/Product/Calls")]
        public string AnalysisSalesCallsChartDateProductCalls(int? product, int? years) {

            if (product == -1) {
                product = null;
            }

            if (years == null) {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetSalesCallsByProductJSON(product, fy.StartDate, fy.EndDate);

        }

        [Route("Analysis/SalesCalls/ChartData/Customer/Calls")]
        public string AnalysisSalesCallsChartDataCustomerCalls(int? customer, int? years)
        {

            if (customer == -1)
            {
                customer = null;
            }

            if (years == null)
            {
                years = 3;
            }

            Domain.FiscalYear fy = Domain.FiscalYear.GetByYear(years.Value, AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth);

            return _service.Charts.GetSalesCallsByCustomerJSON(customer, fy.StartDate, fy.EndDate);

        }

        #endregion



        private AnalysisService _service;

        public AnalysisController() {
            _service = new AnalysisService();
        }



    }
}