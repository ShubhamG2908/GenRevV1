using Genrev.Web.App.Customers;
using Genrev.Web.App.Home.Data;
using Genrev.Web.App.Services;

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Web.Mvc;

namespace Genrev.Web.App.Home
{
    [Authorize]
    public class HomeController : Dymeng.Web.Mvc.DevExpress.ContentAreaController
    {
        private readonly CRMService _crmService;

        public HomeController()
        {
            string connectionString = ConfigurationManager.ConnectionStrings["GenrevContext"].ConnectionString;
            _crmService = new CRMService(connectionString);
        }
        public ActionResult Index()
        {

            return RedirectToAction("Dashboard");
        }

        public ActionResult Dashboard()
        {
            //var results = _crmService.GetCRMRecords();
            //return View(results);
            var model = new DashboardViewModel
            {
                CRMRecords = _crmService.GetCRMRecords(),
                SalesByCustomer = GetSalesByCustomer(),
                SalesBySalesperson = GetSalesBySalesperson()
            };

            return View(model);
        }

        public ActionResult Search()
        {
            return GetView("Search");
        }



        [Route("Home/Dashboard/Data/CurrentYear/TopBottom")]
        public string DashDataCurrentYearTopBottom(DateTime currentDate)
        {

            Domain.Month fyEndingMonth = AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth;
            var fy = Domain.FiscalYear.GetCurrent(currentDate, fyEndingMonth);

            return Data.TopBottomMatrix.GetTopBottomMatrixJSON(fy.StartDate, fy.EndDate);
        }

        [Route("Home/Dashboard/ChartData/CurrentYear/SalesVsForecast")]
        public string DashChartDataCurrentYearSalesVsForecast(DateTime currentDate)
        {

            Domain.Month fyEndingMonth = AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth;
            var fy = Domain.FiscalYear.GetCurrent(currentDate, fyEndingMonth);

            return Charts.CurrentYearSalesVsForecast.GetJSON(fy.StartDate, fy.EndDate);
        }

        [Route("Home/Dashboard/ChartData/CurrentYear/GrossProfitDollars")]
        public string DashChartDataCurrentYearGrossProfitDollars(DateTime currentDate)
        {

            Domain.Month fyEndingMonth = AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth;
            var fy = Domain.FiscalYear.GetCurrent(currentDate, fyEndingMonth);

            return Charts.CurrentYearGrossProfit.GetGpdJSON(fy.StartDate, fy.EndDate);

        }

        [Route("Home/Dashboard/ChartData/CurrentYear/GrossProfitPercent")]
        public string DashChartDataCurrentYearGrossProfitPercent(DateTime currentDate)
        {

            Domain.Month fyEndingMonth = AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth;
            var fy = Domain.FiscalYear.GetCurrent(currentDate, fyEndingMonth);

            return Charts.CurrentYearGrossProfit.GetGppJSON(fy.StartDate, fy.EndDate);

        }

        [Route("Home/Dashboard/Data/CurrentYear/Projections")]
        public string DashDataCurrentYearProjections(DateTime currentDate)
        {

            return Data.CurrentYearProjections.GetProjectionsJSON(currentDate);

        }
        public ActionResult SalesByCustomerGridCallback()
        {
            var model = GetSalesByCustomer();
            return PartialView("_SalesByCustomerGridPartial", model);
        }

        public ActionResult SalesBySalespersonGridCallback()
        {
            var model = GetSalesBySalesperson();
            return PartialView("_SalesBySalespersonGridPartial", model);
        }
        private List<SalesByCustomer> GetSalesByCustomer()
        {
            Domain.Month fyEndingMonth = AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth;
            var fy = Domain.FiscalYear.GetCurrent(DateTime.Now, fyEndingMonth);

            return Data.TopBottomMatrix.GetSalesByCustomer(fy.StartDate, fy.EndDate);
        }

        private List<SalesBySalesperson> GetSalesBySalesperson()
        {
            Domain.Month fyEndingMonth = AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth;
            var fy = Domain.FiscalYear.GetCurrent(DateTime.Now, fyEndingMonth);

            return Data.TopBottomMatrix.GetSalesBySalesperson(fy.StartDate, fy.EndDate);
        }
    }
}