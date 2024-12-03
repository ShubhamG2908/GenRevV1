using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Web.Mvc;

namespace Genrev.Web.App.Home
{
    [Authorize]
    public class HomeController : Dymeng.Web.Mvc.DevExpress.ContentAreaController
    {

        public ActionResult Index() {
            return RedirectToAction("Dashboard");
        }

        public ActionResult Dashboard() {
            return GetView("Dashboard");
        }

        public ActionResult Search() {
            return GetView("Search");
        }

        

        [Route("Home/Dashboard/Data/CurrentYear/TopBottom")]
        public string DashDataCurrentYearTopBottom(DateTime currentDate) {

            Domain.Month fyEndingMonth = AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth;
            var fy = Domain.FiscalYear.GetCurrent(currentDate, fyEndingMonth);

            return Data.TopBottomMatrix.GetTopBottomMatrixJSON(fy.StartDate, fy.EndDate);
        }

        [Route("Home/Dashboard/ChartData/CurrentYear/SalesVsForecast")]
        public string DashChartDataCurrentYearSalesVsForecast(DateTime currentDate) {
            
            Domain.Month fyEndingMonth = AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth;
            var fy = Domain.FiscalYear.GetCurrent(currentDate, fyEndingMonth);
            
            return Charts.CurrentYearSalesVsForecast.GetJSON(fy.StartDate, fy.EndDate);
        }

        [Route("Home/Dashboard/ChartData/CurrentYear/GrossProfitDollars")]
        public string DashChartDataCurrentYearGrossProfitDollars(DateTime currentDate) {

            Domain.Month fyEndingMonth = AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth;
            var fy = Domain.FiscalYear.GetCurrent(currentDate, fyEndingMonth);

            return Charts.CurrentYearGrossProfit.GetGpdJSON(fy.StartDate, fy.EndDate);
            
        }

        [Route("Home/Dashboard/ChartData/CurrentYear/GrossProfitPercent")]
        public string DashChartDataCurrentYearGrossProfitPercent(DateTime currentDate) {

            Domain.Month fyEndingMonth = AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth;
            var fy = Domain.FiscalYear.GetCurrent(currentDate, fyEndingMonth);

            return Charts.CurrentYearGrossProfit.GetGppJSON(fy.StartDate, fy.EndDate);

        }

        [Route("Home/Dashboard/Data/CurrentYear/Projections")]
        public string DashDataCurrentYearProjections(DateTime currentDate) {

            return Data.CurrentYearProjections.GetProjectionsJSON(currentDate);

        }


    }
}