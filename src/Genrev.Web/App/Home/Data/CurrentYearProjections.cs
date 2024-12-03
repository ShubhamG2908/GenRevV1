using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Genrev.Data;
using Genrev.Domain.DataSets;
using Genrev.Domain;

using System.Dynamic;
using Newtonsoft.Json.Linq;

namespace Genrev.Web.App.Home.Data
{
    public class CurrentYearProjections
    {

        public static string GetProjectionsJSON(DateTime refDate) {

            var context = new GenrevContext();

            Month fyEndingMonth = AppService.Current.Account.PrimaryCompany.FiscalYearEndMonth;
            FiscalYear fy = FiscalYear.GetCurrent(refDate, fyEndingMonth);
            List<MonthlyData> yearData = context.GetMonthlyData(fy.StartDate, fy.EndDate, AppService.Current.ViewContext.PersonnelIDs).ToList();

            fy.MonthlyDataByPersonnel = yearData;
            
            var refFirstDayOfMonth = new DateTime(refDate.Year, refDate.Month, 1);
            MonthlyData monthData = yearData.Where(x => x.Period == refFirstDayOfMonth).Single();

            dynamic projections = new ExpandoObject();

            projections.saleseomProjection = MonthlyData.Projections.MonthEndSales(monthData.SalesActual, refDate);
            projections.saleseoyProjection = MonthlyData.Projections.YearEndSales(yearData.Sum(x => x.SalesActual), refDate, fy);
            projections.saleseomForecast = monthData.SalesForecast;
            projections.saleseoyForecast = yearData.Sum(x => x.SalesForecast);
            projections.saleseomDiff = projections.saleseomProjection - projections.saleseomForecast;
            projections.saleseoyDiff = projections.saleseoyProjection - projections.saleseoyForecast;

            projections.gpdeomProjection = MonthlyData.Projections.MonthEndGPD(monthData.GrossProfitDollars, refDate);
            projections.gpdeoyProjection = MonthlyData.Projections.YearEndGPD(yearData.Sum(x => x.GrossProfitDollars), refDate, fy);
            projections.gpdeomForecast = monthData.GrossProfitDollarsForecast;
            projections.gpdeoyForecast = yearData.Sum(x => x.GrossProfitDollarsForecast);
            projections.gpdeomDiff = projections.gpdeomProjection - projections.gpdeomForecast;
            projections.gpdeoyDiff = projections.gpdeoyProjection - projections.gpdeoyForecast;

            projections.gppeomProjection = MonthlyData.Projections.MonthEndGPP(monthData.SalesActual, monthData.GrossProfitDollars, refDate);
            projections.gppeoyProjection = MonthlyData.Projections.YearEndGPP(yearData.Sum(x => x.SalesActual), yearData.Sum(x => x.GrossProfitDollars), refDate, fy);
            projections.gppeomForecast = monthData.GrossProfitPercentForecast;
            //projections.gppeoyForecast = fy.GetAccurateYearEndGPPForecast(refDate);
            projections.gppeoyForecast = yearData.Average(x => x.GrossProfitPercent);
            projections.gppeomDiff = projections.gppeomProjection - projections.gppeomForecast;
            projections.gppeoyDiff = projections.gppeoyProjection - projections.gppeoyForecast;

            if (AppService.Current.Settings.TargetFeatureEnabled)
            {
                projections.saleseomTarget = monthData.SalesTarget;
                projections.saleseoyTarget = yearData.Sum(x => x.SalesTarget);
                projections.saleseomTargetDiff = projections.saleseomProjection - projections.saleseomTarget;
                projections.saleseoyTargetDiff = projections.saleseoyProjection - projections.saleseoyTarget;

                projections.gpdeomTarget = monthData.GrossProfitDollarsTarget;
                projections.gpdeoyTarget = yearData.Sum(x => x.GrossProfitDollarsTarget);
                projections.gpdeomTargetDiff = projections.gpdeomProjection - projections.gpdeomTarget;
                projections.gpdeoyTargetDiff = projections.gpdeoyProjection - projections.gpdeoyTarget;

                projections.gppeomTarget = monthData.GrossProfitPercentTarget;
                projections.gppeoyTarget = projections.saleseoyTarget == 0 ? 0 : (projections.gpdeoyTarget / projections.saleseoyTarget) * 100;
                projections.gppeomTargetDiff = projections.gppeomProjection - projections.gppeomTarget;
                projections.gppeoyTargetDiff = projections.gppeoyProjection - projections.gppeoyTarget;
            }

            return JObject.FromObject(projections).ToString();

        }
        
    }
}