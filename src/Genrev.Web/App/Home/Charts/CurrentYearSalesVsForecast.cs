using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Genrev.Web.App.Home.Charts
{
    public class CurrentYearSalesVsForecast
    {
        public static string GetJSON(DateTime startDate, DateTime endDate)
        {
            var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, AppService.Current.ViewContext.PersonnelIDs).ToList();
            var months = model.Select(x => x.Period.ToString("MMM"));

            dynamic ytd = new ExpandoObject();
            ytd.sales = model.Sum(x => x.SalesActual);

            dynamic totals = new ExpandoObject();
            totals.ytd = ytd;
            totals.forecast = model.Sum(x => x.SalesForecast);

            dynamic salesSeries = Analysis.AnalysisService.ChartsService.GetActualDisplay();
            salesSeries.data = model.Select(x => x.SalesActual).ToList();

            dynamic forecastSeries = Analysis.AnalysisService.ChartsService.GetForecastDisplay();
            forecastSeries.data = model.Select(x => x.SalesForecast).ToList();

            dynamic mtdForecastSeries = Analysis.AnalysisService.ChartsService.GetMTDDisplay();
            mtdForecastSeries.data = getMTDForecastSeries(model);

            dynamic targetSeries = Analysis.AnalysisService.ChartsService.GetTargetDisplay();
            targetSeries.data = model.Select(x => x.SalesTarget);

            var allSeries = new List<Object>() { salesSeries, forecastSeries, mtdForecastSeries };
            if (AppService.Current.Settings.TargetFeatureEnabled)
            {
                allSeries.Add(targetSeries);
                totals.target = model.Sum(x => x.SalesTarget);
            }

            JObject chart = JObject.FromObject(new
            {
                categories = months,
                series = allSeries,
                mtdForecastSeries,
                totals = totals
            });

            return chart.ToString();
        }

        private static List<decimal?> getMTDForecastSeries(List<Domain.DataSets.MonthlyData> monthData)
        {
            var list = new List<decimal?>();
            var refDate = DateTime.Now;

            foreach (var data in monthData)
            {
                list.Add(data.MonthToDateSalesForecast(refDate));
            }
            return list;
        }
    }
}