using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using System.Dynamic;
using Newtonsoft.Json.Linq;
using Genrev.Data;

namespace Genrev.Web.App.Home.Charts
{
    public class CurrentYearGrossProfit
    {



        public static string GetGpdJSON(DateTime startDate, DateTime endDate) {

            var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, AppService.Current.ViewContext.PersonnelIDs);
            //var model = new GenrevContext().GetMonthlyDataByPersonnel(startDate, endDate, AppService.Current.ViewContext.PersonnelIDs).ToList();
                        
            var months = model.Select(x => x.Period.ToString("MMM"));

            dynamic gpActual = new ExpandoObject();
            gpActual.name = "Actual";
            gpActual.data = model.Select(x => x.GrossProfitDollars).ToList();
            

            dynamic gpForecast = new ExpandoObject();
            gpForecast.name = "Forecast";
            gpForecast.data = model.Select(x => x.GrossProfitDollarsForecast).ToList();

            var allSeries = new List<Object>() { gpActual, gpForecast };

            JObject chart = JObject.FromObject(new {
                categories = months,
                series = allSeries
            });

            return chart.ToString();

        }

        public static string GetGppJSON(DateTime startDate, DateTime endDate) {

            var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, AppService.Current.ViewContext.PersonnelIDs).ToList();

            var months = model.Select(x => x.Period.ToString("MMM"));

            dynamic gpActual = new ExpandoObject();
            gpActual.name = "Actual";
            gpActual.data = model.Select(x => x.GrossProfitPercent).ToList();

            dynamic gpForecast = new ExpandoObject();
            gpForecast.name = "Forecast";
            gpForecast.data = model.Select(x => x.GrossProfitPercentForecast).ToList();

            var allSeries = new List<Object>() { gpActual, gpForecast };

            JObject chart = JObject.FromObject(new {
                categories = months,
                series = allSeries
            });

            return chart.ToString();

        }

    }
}