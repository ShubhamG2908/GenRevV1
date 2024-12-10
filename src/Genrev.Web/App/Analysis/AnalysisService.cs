using System;
using System.Collections.Generic;
using System.Linq;
using System.Dynamic;
using Newtonsoft.Json.Linq;
using Genrev.DomainServices.Data;
using Genrev.Web.App.Analysis.Models;
using static Genrev.Web.App.CommonListItems;
using Genrev.Domain.DataSets;
using Genrev.Web.App.Products.Models;

namespace Genrev.Web.App.Analysis
{
    public class AnalysisService
    {

        public ChartsService Charts { get; set; } = new ChartsService();
        private readonly YearProvider _yearProvider = new YearProvider();

        #region DRILLDOWN
        /**********************
         * 
         * DRILLDOWN
         * 
         * *******************/


        public int DefaultYear { get { return _yearProvider.GetDefaultYear(); } }

        public IEnumerable<int> GetDefaultYears()
        {
            return _yearProvider.GetDefaultYears();
        }

        public List<Models.DrilldownListItem> GetDrilldownListItems(DateTime startDate, DateTime endDate)
        {


            var cacheKey = getDrilldownDataCacheKey(startDate, endDate);

            object cacheObject = AppCache.GetItem(cacheKey);

            if (cacheObject != null)
            {
                return cacheObject as List<Models.DrilldownListItem>;
            }


            var context = AppService.Current.DataContext;

            context.Database.Log = (s) => System.Diagnostics.Debug.WriteLine(s);

            int[] ids = AppService.Current.ViewContext.PersonnelIDs;

            var data = context.CustomerDrilldowns
                .Where(x => x.Period >= startDate && x.Period <= endDate && ids.ToList().Contains((int)x.PersonnelID))
                .GroupBy(x => new
                {
                    x.CalendarMonth,
                    x.CalendarYear,
                    x.CustomerID,
                    x.CustomerName,
                    x.PersonnelID,
                    x.PersonFirstName,
                    x.PersonLastName,
                    x.ProductSKU,
                    x.IndustryName,
                    x.CustomerTypeName,
                    x.AccountTypeName,
                    x.IndustryID,
                    x.CustomerTypeID,
                    x.AccountTypeID
                }).Select(y => new CustomerDrilldownVM()
                {
                    CalendarMonth = y.Key.CalendarMonth,
                    CalendarYear = y.Key.CalendarYear,
                    CustomerName = y.Key.CustomerName,
                    PersonFirstName = y.Key.PersonFirstName,
                    PersonLastName = y.Key.PersonLastName,
                    ProductSKU = y.Key.ProductSKU,
                    IndustryName = y.Key.IndustryName,
                    CustomerTypeName = y.Key.CustomerTypeName,
                    AccountTypeName = y.Key.AccountTypeName,
                    IndustryID = y.Key.IndustryID,
                    CustomerTypeID = y.Key.CustomerTypeID,
                    AccountTypeID = y.Key.AccountTypeID,
                    PersonnelID = y.Key.PersonnelID,
                    CallsActual = y.Sum(x => x.CallsActual),
                    CallsForecast = y.Sum(x => x.CallsForecast),
                    SalesActual = y.Sum(x => x.SalesActual),
                    SalesForecast = y.Sum(x => x.SalesForecast),
                    CostActual = y.Sum(x => x.CostActual),
                    CostForecast = y.Sum(x => x.CostForecast)
                }).ToList();

            var items = new List<Models.DrilldownListItem>();

            foreach (var d in data)
            {
                items.Add(Models.DrilldownListItem.FromCustomerDrilldownModel(new CustomerDrilldown(d)));
            }

            AppCache.AddItem(cacheKey, items);

            return items;
        }


        private string getDrilldownDataCacheKey(DateTime startDate, DateTime endDate)
        {

            string key =
                "DrilldownData_" +
                AppService.Current.ViewContext.PersonID.ToString() +
                "_" + startDate.ToString("yyyyMMdd") +
                "_" + endDate.ToString("yyyyMMdd");

            return key;
        }


        #endregion


        public class ChartsService
        {


            #region HISTORIC

            /**********************
             * 
             * HISTORIC
             * 
             * *******************/

            internal string GetHistoricSalesJSON(int salesperson, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(salesperson);

                var model = AppService.Current.DataContext.GetHistoricSales(startDate, endDate, spIDs);

                var years = model.Select(x => x.FiscalYear);

                dynamic salesSeries = new ExpandoObject();
                salesSeries.name = "Sales Dollars";
                salesSeries.data = model.Select(x => x.SalesActual).ToList();

                var allseries = new List<Object>() { salesSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = years,
                    series = allseries
                });

                return chart.ToString();

            }

            internal string GetHistoricGPJSON(int salesperson, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(salesperson);

                var model = AppService.Current.DataContext.GetHistoricSales(startDate, endDate, spIDs);

                var years = model.Select(x => x.FiscalYear);

                dynamic gpdSeries = new ExpandoObject();
                gpdSeries.name = "Gross Profit Dollars";
                gpdSeries.data = model.Select(x => x.GrossProfitDollars).ToList();
                gpdSeries.yAxis = 0;

                dynamic gppSeries = new ExpandoObject();
                gppSeries.name = "Gross Profit Percent";
                gppSeries.data = model.Select(x => x.GrossProfitPercent).ToList();
                gppSeries.yAxis = 1;

                var allseries = new List<Object>() { gpdSeries, gppSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = years,
                    series = allseries
                });

                return chart.ToString();

            }

            internal string GetHistoricSalesByIndustryJSON(int? industry, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                int[] indIDs = industry.HasValue ? new int[] { industry.Value } : null;

                var model = AppService.Current.DataContext.GetHistoricSales(startDate, endDate, spIDs, indIDs);

                var years = model.Select(x => x.FiscalYear);

                dynamic salesSeries = new ExpandoObject();
                salesSeries.name = "Sales Dollars";
                salesSeries.data = model.Select(x => x.SalesActual).ToList();

                var allseries = new List<Object>() { salesSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = years,
                    series = allseries
                });

                return chart.ToString();

            }

            internal string GetHistoricGPByIndustryJSON(int? industry, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                int[] indIDs = industry.HasValue ? new int[] { industry.Value } : null;

                var model = AppService.Current.DataContext.GetHistoricSales(startDate, endDate, spIDs, indIDs);

                var years = model.Select(x => x.FiscalYear);

                dynamic gpdSeries = new ExpandoObject();
                gpdSeries.name = "Gross Profit Dollars";
                gpdSeries.data = model.Select(x => x.GrossProfitDollars).ToList();
                gpdSeries.yAxis = 0;

                dynamic gppSeries = new ExpandoObject();
                gppSeries.name = "Gross Profit Percent";
                gppSeries.data = model.Select(x => x.GrossProfitPercent).ToList();
                gppSeries.yAxis = 1;

                var allseries = new List<Object>() { gpdSeries, gppSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = years,
                    series = allseries
                });

                return chart.ToString();

            }

            internal string GetHistoricSalesByCustomerTypeJSON(int? customerType, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                int[] ctIDs = customerType.HasValue ? new int[] { customerType.Value } : null;

                var model = AppService.Current.DataContext.GetHistoricSales(startDate, endDate, spIDs, null, ctIDs);

                var years = model.Select(x => x.FiscalYear);

                dynamic salesSeries = new ExpandoObject();
                salesSeries.name = "Sales Dollars";
                salesSeries.data = model.Select(x => x.SalesActual).ToList();

                var allseries = new List<Object>() { salesSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = years,
                    series = allseries
                });

                return chart.ToString();
            }

            internal string GetHistoricGPByCustomerTypeJSON(int? customerType, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                int[] ctIDs = customerType.HasValue ? new int[] { customerType.Value } : null;

                var model = AppService.Current.DataContext.GetHistoricSales(startDate, endDate, spIDs, null, ctIDs);

                var years = model.Select(x => x.FiscalYear);

                dynamic gpdSeries = new ExpandoObject();
                gpdSeries.name = "Gross Profit Dollars";
                gpdSeries.data = model.Select(x => x.GrossProfitDollars).ToList();
                gpdSeries.yAxis = 0;

                dynamic gppSeries = new ExpandoObject();
                gppSeries.name = "Gross Profit Percent";
                gppSeries.data = model.Select(x => x.GrossProfitPercent).ToList();
                gppSeries.yAxis = 1;

                var allseries = new List<Object>() { gpdSeries, gppSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = years,
                    series = allseries
                });

                return chart.ToString();

            }



            internal string GetHistoricSalesByAccountTypeJSON(int? accountType, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                int[] atIDs = accountType.HasValue ? new int[] { accountType.Value } : null;

                var model = AppService.Current.DataContext.GetHistoricSales(startDate, endDate, spIDs, null, null, atIDs);

                var years = model.Select(x => x.FiscalYear);

                dynamic salesSeries = new ExpandoObject();
                salesSeries.name = "Sales Dollars";
                salesSeries.data = model.Select(x => x.SalesActual).ToList();

                var allseries = new List<Object>() { salesSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = years,
                    series = allseries
                });

                return chart.ToString();
            }

            internal string GetHistoricGPByAccountTypeJSON(int? accountType, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                int[] atIDs = accountType.HasValue ? new int[] { accountType.Value } : null;

                var model = AppService.Current.DataContext.GetHistoricSales(startDate, endDate, spIDs, null, null, atIDs);

                var years = model.Select(x => x.FiscalYear);

                dynamic gpdSeries = new ExpandoObject();
                gpdSeries.name = "Gross Profit Dollars";
                gpdSeries.data = model.Select(x => x.GrossProfitDollars).ToList();
                gpdSeries.yAxis = 0;

                dynamic gppSeries = new ExpandoObject();
                gppSeries.name = "Gross Profit Percent";
                gppSeries.data = model.Select(x => x.GrossProfitPercent).ToList();
                gppSeries.yAxis = 1;

                var allseries = new List<Object>() { gpdSeries, gppSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = years,
                    series = allseries
                });

                return chart.ToString();

            }









            internal string GetHistoricSalesByProductJSON(int? product, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                int[] pIDs = product.HasValue ? new int[] { product.Value } : null;

                var model = AppService.Current.DataContext.GetHistoricSales(startDate, endDate, spIDs, null, null, null, pIDs);

                var years = model.Select(x => x.FiscalYear);

                dynamic salesSeries = new ExpandoObject();
                salesSeries.name = "Sales Dollars";
                salesSeries.data = model.Select(x => x.SalesActual).ToList();

                var allseries = new List<Object>() { salesSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = years,
                    series = allseries
                });

                return chart.ToString();
            }

            internal string GetHistoricGPByProductJSON(int? product, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                int[] pIDs = product.HasValue ? new int[] { product.Value } : null;

                var model = AppService.Current.DataContext.GetHistoricSales(startDate, endDate, spIDs, null, null, null, pIDs);

                var years = model.Select(x => x.FiscalYear);

                dynamic gpdSeries = new ExpandoObject();
                gpdSeries.name = "Gross Profit Dollars";
                gpdSeries.data = model.Select(x => x.GrossProfitDollars).ToList();
                gpdSeries.yAxis = 0;

                dynamic gppSeries = new ExpandoObject();
                gppSeries.name = "Gross Profit Percent";
                gppSeries.data = model.Select(x => x.GrossProfitPercent).ToList();
                gppSeries.yAxis = 1;

                var allseries = new List<Object>() { gpdSeries, gppSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = years,
                    series = allseries
                });

                return chart.ToString();

            }



            internal string GetHistoricSalesByCustomerJSON(int? customer, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                int[] cIDs = customer.HasValue ? new int[] { customer.Value } : null;

                var model = AppService.Current.DataContext.GetHistoricSales(startDate, endDate, spIDs, null, null, null, null, cIDs);

                var years = model.Select(x => x.FiscalYear);

                dynamic salesSeries = new ExpandoObject();
                salesSeries.name = "Sales Dollars";
                salesSeries.data = model.Select(x => x.SalesActual).ToList();

                var allseries = new List<Object>() { salesSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = years,
                    series = allseries
                });

                return chart.ToString();
            }

            internal string GetHistoricGPByCustomerJSON(int? customer, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                int[] cIDs = customer.HasValue ? new int[] { customer.Value } : null;

                var model = AppService.Current.DataContext.GetHistoricSales(startDate, endDate, spIDs, null, null, null, null, cIDs);

                var years = model.Select(x => x.FiscalYear);

                dynamic gpdSeries = new ExpandoObject();
                gpdSeries.name = "Gross Profit Dollars";
                gpdSeries.data = model.Select(x => x.GrossProfitDollars).ToList();
                gpdSeries.yAxis = 0;

                dynamic gppSeries = new ExpandoObject();
                gppSeries.name = "Gross Profit Percent";
                gppSeries.data = model.Select(x => x.GrossProfitPercent).ToList();
                gppSeries.yAxis = 1;

                var allseries = new List<Object>() { gpdSeries, gppSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = years,
                    series = allseries
                });

                return chart.ToString();

            }



            #endregion



            #region ACTUAL VS FORECAST

            /**********************
             * 
             * ACTUAL VS FORECAST
             * 
             * *******************/

            internal string GetActualVsForecastSalesJSON(int salesperson, DateTime startDate, DateTime endDate)
            {

                System.Diagnostics.Debug.WriteLine("SALESPERSON: " + salesperson.ToString());

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(salesperson);

                System.Diagnostics.Debug.WriteLine("SALESPERSON IDs: " + string.Join(",", spIDs).ToString());

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, spIDs);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.sales = model.Sum(x => x.SalesActual);
                totals.forecast = model.Sum(x => x.SalesForecast);

                dynamic salesSeries = GetActualDisplay();
                salesSeries.data = model.Select(x => x.SalesActual).ToList();

                dynamic forecastSeries = GetForecastDisplay();
                forecastSeries.data = model.Select(x => x.SalesForecast).ToList();

                dynamic targetSeries = GetTargetDisplay();
                targetSeries.data = model.Select(x => x.SalesTarget);

                dynamic mtdSeries = GetMTDDisplay();
                mtdSeries.data = getMTDForecastSeries(model);

                var allseries = new List<Object>() { salesSeries, forecastSeries, mtdSeries };
                if (AppService.Current.Settings.TargetFeatureEnabled)
                {
                    allseries.Add(targetSeries);
                    totals.target = model.Sum(x => x.SalesTarget);
                }

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
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

            private static List<decimal?> getMTDForecastCallsSeries(List<Domain.DataSets.MonthlyData> monthData)
            {
                var list = new List<decimal?>();

                var refDate = DateTime.Now;

                foreach (var data in monthData)
                {
                    list.Add(data.MonthToDateCallsForecast(refDate));
                }

                return list;
            }

            internal string GetActualVsForecastGPJSON(int salesperson, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(salesperson);

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, spIDs);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.gpd = model.Sum(x => x.GrossProfitDollars);
                totals.gpdForecast = model.Sum(x => x.GrossProfitDollarsForecast);

                dynamic gpdSeries = new ExpandoObject();
                gpdSeries.name = "Actual GPD ($)";
                gpdSeries.data = model.Select(x => x.GrossProfitDollars).ToList();
                gpdSeries.yAxis = 0;

                dynamic gpdfSeries = new ExpandoObject();
                gpdfSeries.name = "Forecasted GPD ($)";
                gpdfSeries.data = model.Select(x => x.GrossProfitDollarsForecast).ToList();
                gpdfSeries.yAxis = 0;

                dynamic gppSeries = new ExpandoObject();
                gppSeries.name = "Actual GPP (%)";
                gppSeries.data = model.Select(x => x.GrossProfitPercent).ToList();
                gppSeries.yAxis = 1;

                dynamic gppfSeries = new ExpandoObject();
                gppfSeries.name = "Forecasted GPP (%)";
                gppfSeries.data = model.Select(x => x.GrossProfitPercentForecast).ToList();
                gppfSeries.yAxis = 1;

                var allseries = new List<Object>() { gpdSeries, gpdfSeries, gppSeries, gppfSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();

            }

            internal string GetActualVsForecastSalesByIndustryJSON(int? industry, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                int[] indIDs = industry.HasValue ? new int[] { industry.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, spIDs, indIDs);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.sales = model.Sum(x => x.SalesActual);
                totals.forecast = model.Sum(x => x.SalesForecast);

                dynamic salesSeries = GetActualDisplay();
                salesSeries.data = model.Select(x => x.SalesActual).ToList();

                dynamic forecastSeries = GetForecastDisplay();
                forecastSeries.data = model.Select(x => x.SalesForecast).ToList();

                dynamic targetSeries = GetTargetDisplay();
                targetSeries.data = model.Select(x => x.SalesTarget);

                dynamic mtdSeries = GetMTDDisplay();
                mtdSeries.data = getMTDForecastSeries(model);

                var allseries = new List<Object>() { salesSeries, forecastSeries, mtdSeries };
                if (AppService.Current.Settings.TargetFeatureEnabled)
                {
                    allseries.Add(targetSeries);
                    totals.target = model.Sum(x => x.SalesTarget);
                }

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();

            }

            internal string GetActualVsForecastGPByIndustryJSON(int? industry, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                int[] indIDs = industry.HasValue ? new int[] { industry.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, spIDs, indIDs);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.gpd = model.Sum(x => x.GrossProfitDollars);
                totals.gpdForecast = model.Sum(x => x.GrossProfitDollarsForecast);

                dynamic gpdSeries = new ExpandoObject();
                gpdSeries.name = "Actual GPD ($)";
                gpdSeries.data = model.Select(x => x.GrossProfitDollars).ToList();
                gpdSeries.yAxis = 0;

                dynamic gpdfSeries = new ExpandoObject();
                gpdfSeries.name = "Forecasted GPD ($)";
                gpdfSeries.data = model.Select(x => x.GrossProfitDollarsForecast).ToList();
                gpdfSeries.yAxis = 0;

                dynamic gppSeries = new ExpandoObject();
                gppSeries.name = "Actual GPP (%)";
                gppSeries.data = model.Select(x => x.GrossProfitPercent).ToList();
                gppSeries.yAxis = 1;

                dynamic gppfSeries = new ExpandoObject();
                gppfSeries.name = "Forecasted GPP (%)";
                gppfSeries.data = model.Select(x => x.GrossProfitPercentForecast).ToList();
                gppfSeries.yAxis = 1;

                var allseries = new List<Object>() { gpdSeries, gpdfSeries, gppSeries, gppfSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();

            }

            internal string GetActualVsForecastSalesByCustomerTypeJSON(int? customerType, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                int[] ctIDs = customerType.HasValue ? new int[] { customerType.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, spIDs, null, ctIDs);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.sales = model.Sum(x => x.SalesActual);
                totals.forecast = model.Sum(x => x.SalesForecast);

                dynamic salesSeries = GetActualDisplay();
                salesSeries.data = model.Select(x => x.SalesActual).ToList();

                dynamic forecastSeries = GetForecastDisplay();
                forecastSeries.data = model.Select(x => x.SalesForecast).ToList();

                dynamic targetSeries = GetTargetDisplay();
                targetSeries.data = model.Select(x => x.SalesTarget);

                dynamic mtdSeries = GetMTDDisplay();
                mtdSeries.data = getMTDForecastSeries(model);

                var allseries = new List<Object>() { salesSeries, forecastSeries, mtdSeries };
                if (AppService.Current.Settings.TargetFeatureEnabled)
                {
                    allseries.Add(targetSeries);
                    totals.target = model.Sum(x => x.SalesTarget);
                }

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();
            }

            internal string GetActualVsForecastGPByCustomerTypeJSON(int? customerType, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                int[] ctIDs = customerType.HasValue ? new int[] { customerType.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, spIDs, null, ctIDs);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.gpd = model.Sum(x => x.GrossProfitDollars);
                totals.gpdForecast = model.Sum(x => x.GrossProfitDollarsForecast);

                dynamic gpdSeries = new ExpandoObject();
                gpdSeries.name = "Actual GPD ($)";
                gpdSeries.data = model.Select(x => x.GrossProfitDollars).ToList();
                gpdSeries.yAxis = 0;

                dynamic gpdfSeries = new ExpandoObject();
                gpdfSeries.name = "Forecasted GPD ($)";
                gpdfSeries.data = model.Select(x => x.GrossProfitDollarsForecast).ToList();
                gpdfSeries.yAxis = 0;

                dynamic gppSeries = new ExpandoObject();
                gppSeries.name = "Actual GPP (%)";
                gppSeries.data = model.Select(x => x.GrossProfitPercent).ToList();
                gppSeries.yAxis = 1;

                dynamic gppfSeries = new ExpandoObject();
                gppfSeries.name = "Forecasted GPP (%)";
                gppfSeries.data = model.Select(x => x.GrossProfitPercentForecast).ToList();
                gppfSeries.yAxis = 1;

                var allseries = new List<Object>() { gpdSeries, gpdfSeries, gppSeries, gppfSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();

            }

            internal string GetActualVsForecastSalesByAccountTypeJSON(int? accountType, DateTime startDate, DateTime endDate)
            {

                var fp = new Domain.DataSets.FilterParams();
                fp.PersonnelIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                fp.AccountTypeIDs = accountType.HasValue ? new int[] { accountType.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, fp);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.sales = model.Sum(x => x.SalesActual);
                totals.forecast = model.Sum(x => x.SalesForecast);

                dynamic salesSeries = GetActualDisplay();
                salesSeries.data = model.Select(x => x.SalesActual).ToList();

                dynamic forecastSeries = GetForecastDisplay();
                forecastSeries.data = model.Select(x => x.SalesForecast).ToList();

                dynamic targetSeries = GetTargetDisplay();
                targetSeries.data = model.Select(x => x.SalesTarget);

                dynamic mtdSeries = GetMTDDisplay();
                mtdSeries.data = getMTDForecastSeries(model);

                var allseries = new List<Object>() { salesSeries, forecastSeries, mtdSeries };
                if (AppService.Current.Settings.TargetFeatureEnabled)
                {
                    allseries.Add(targetSeries);
                    totals.target = model.Sum(x => x.SalesTarget);
                }

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();
            }

            internal string GetActualVsForecastGPByAccountTypeJSON(int? accountType, DateTime startDate, DateTime endDate)
            {

                var fp = new Domain.DataSets.FilterParams();
                fp.PersonnelIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                fp.AccountTypeIDs = accountType.HasValue ? new int[] { accountType.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, fp);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.gpd = model.Sum(x => x.GrossProfitDollars);
                totals.gpdForecast = model.Sum(x => x.GrossProfitDollarsForecast);

                dynamic gpdSeries = new ExpandoObject();
                gpdSeries.name = "Actual GPD ($)";
                gpdSeries.data = model.Select(x => x.GrossProfitDollars).ToList();
                gpdSeries.yAxis = 0;

                dynamic gpdfSeries = new ExpandoObject();
                gpdfSeries.name = "Forecasted GPD ($)";
                gpdfSeries.data = model.Select(x => x.GrossProfitDollarsForecast).ToList();
                gpdfSeries.yAxis = 0;

                dynamic gppSeries = new ExpandoObject();
                gppSeries.name = "Actual GPP (%)";
                gppSeries.data = model.Select(x => x.GrossProfitPercent).ToList();
                gppSeries.yAxis = 1;

                dynamic gppfSeries = new ExpandoObject();
                gppfSeries.name = "Forecasted GPP (%)";
                gppfSeries.data = model.Select(x => x.GrossProfitPercentForecast).ToList();
                gppfSeries.yAxis = 1;

                var allseries = new List<Object>() { gpdSeries, gpdfSeries, gppSeries, gppfSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();

            }

            internal string GetActualVsForecastSalesByProductJSON(int? product, DateTime startDate, DateTime endDate)
            {

                var fp = new Domain.DataSets.FilterParams();
                fp.PersonnelIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                fp.ProductIDs = product.HasValue ? new int[] { product.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, fp);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.sales = model.Sum(x => x.SalesActual);
                totals.forecast = model.Sum(x => x.SalesForecast);

                dynamic salesSeries = GetActualDisplay();
                salesSeries.data = model.Select(x => x.SalesActual).ToList();

                dynamic forecastSeries = GetForecastDisplay();
                forecastSeries.data = model.Select(x => x.SalesForecast).ToList();

                dynamic targetSeries = GetTargetDisplay();
                targetSeries.data = model.Select(x => x.SalesTarget);

                dynamic mtdSeries = GetMTDDisplay();
                mtdSeries.data = getMTDForecastSeries(model);

                var allseries = new List<Object>() { salesSeries, forecastSeries, mtdSeries };
                if (AppService.Current.Settings.TargetFeatureEnabled)
                {
                    allseries.Add(targetSeries);
                    totals.target = model.Sum(x => x.SalesTarget);
                }

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();
            }

            internal string GetActualVsForecastGPByProductJSON(int? product, DateTime startDate, DateTime endDate)
            {

                var fp = new Domain.DataSets.FilterParams();
                fp.PersonnelIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                fp.ProductIDs = product.HasValue ? new int[] { product.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, fp);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.gpd = model.Sum(x => x.GrossProfitDollars);
                totals.gpdForecast = model.Sum(x => x.GrossProfitDollarsForecast);

                dynamic gpdSeries = new ExpandoObject();
                gpdSeries.name = "Actual GPD ($)";
                gpdSeries.data = model.Select(x => x.GrossProfitDollars).ToList();
                gpdSeries.yAxis = 0;

                dynamic gpdfSeries = new ExpandoObject();
                gpdfSeries.name = "Forecasted GPD ($)";
                gpdfSeries.data = model.Select(x => x.GrossProfitDollarsForecast).ToList();
                gpdfSeries.yAxis = 0;

                dynamic gppSeries = new ExpandoObject();
                gppSeries.name = "Actual GPP (%)";
                gppSeries.data = model.Select(x => x.GrossProfitPercent).ToList();
                gppSeries.yAxis = 1;

                dynamic gppfSeries = new ExpandoObject();
                gppfSeries.name = "Forecasted GPP (%)";
                gppfSeries.data = model.Select(x => x.GrossProfitPercentForecast).ToList();
                gppfSeries.yAxis = 1;

                var allseries = new List<Object>() { gpdSeries, gpdfSeries, gppSeries, gppfSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();

            }


            internal string GetActualVsForecastSalesByCustomerJSON(int? customer, DateTime startDate, DateTime endDate)
            {

                var fp = new Domain.DataSets.FilterParams();
                fp.PersonnelIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                fp.CustomerIDs = customer.HasValue ? new int[] { customer.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, fp);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.sales = model.Sum(x => x.SalesActual);
                totals.forecast = model.Sum(x => x.SalesForecast);

                dynamic salesSeries = GetActualDisplay();
                salesSeries.data = model.Select(x => x.SalesActual).ToList();

                dynamic forecastSeries = GetForecastDisplay();
                forecastSeries.data = model.Select(x => x.SalesForecast).ToList();

                dynamic targetSeries = GetTargetDisplay();
                targetSeries.data = model.Select(x => x.SalesTarget);

                dynamic mtdSeries = GetMTDDisplay();
                mtdSeries.data = getMTDForecastSeries(model);

                var allseries = new List<Object>() { salesSeries, forecastSeries, mtdSeries };
                if (AppService.Current.Settings.TargetFeatureEnabled)
                {
                    allseries.Add(targetSeries);
                    totals.target = model.Sum(x => x.SalesTarget);
                }

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();
            }

            internal string GetActualVsForecastGPByCustomerJSON(int? customer, DateTime startDate, DateTime endDate)
            {

                var fp = new Domain.DataSets.FilterParams();
                fp.PersonnelIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                fp.CustomerIDs = customer.HasValue ? new int[] { customer.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, fp);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.gpd = model.Sum(x => x.GrossProfitDollars);
                totals.gpdForecast = model.Sum(x => x.GrossProfitDollarsForecast);

                dynamic gpdSeries = new ExpandoObject();
                gpdSeries.name = "Actual GPD ($)";
                gpdSeries.data = model.Select(x => x.GrossProfitDollars).ToList();
                gpdSeries.yAxis = 0;

                dynamic gpdfSeries = new ExpandoObject();
                gpdfSeries.name = "Forecasted GPD ($)";
                gpdfSeries.data = model.Select(x => x.GrossProfitDollarsForecast).ToList();
                gpdfSeries.yAxis = 0;

                dynamic gppSeries = new ExpandoObject();
                gppSeries.name = "Actual GPP (%)";
                gppSeries.data = model.Select(x => x.GrossProfitPercent).ToList();
                gppSeries.yAxis = 1;

                dynamic gppfSeries = new ExpandoObject();
                gppfSeries.name = "Forecasted GPP (%)";
                gppfSeries.data = model.Select(x => x.GrossProfitPercentForecast).ToList();
                gppfSeries.yAxis = 1;

                var allseries = new List<Object>() { gpdSeries, gpdfSeries, gppSeries, gppfSeries };

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();

            }

            #endregion



            #region OPPORTUNITIES

            /**********************
             * 
             * OPPORTUNITIES
             * 
             * *******************/



            internal string GetOpportunitiesJSONBySalesperson(int salespersonID, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(salespersonID);

                var data = AppService.Current.DataContext.GetOpportunitiesAggregateByPersonnel(
                    startDate, endDate, spIDs);

                var potential = filterOpportunityAggregateBase(data, "potential");
                var current = filterOpportunityAggregateBase(data, "current");
                var future = filterOpportunityAggregateBase(data, "future");

                dynamic model = new ExpandoObject();

                model.potential = potential.Select(x => new { name = x.GroupEntityName, y = x.Potential }).ToList();
                model.current = current.Select(x => new { name = x.GroupEntityName, y = x.CurrentOpportunity }).ToList();
                model.future = future.Select(x => new { name = x.GroupEntityName, y = x.FutureOpportunity }).ToList();

                JObject o = JObject.FromObject(model);

                return o.ToString();
            }

            internal string GetOpportunitiesJSONByIndustry(int salespersonID, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(salespersonID);

                var data = AppService.Current.DataContext.GetOpportunitiesAggregateByIndustry(
                    startDate, endDate, spIDs);

                var potential = filterOpportunityAggregateBase(data, "potential");
                var current = filterOpportunityAggregateBase(data, "current");
                var future = filterOpportunityAggregateBase(data, "future");

                dynamic model = new ExpandoObject();

                model.potential = potential.Select(x => new { name = x.GroupEntityName, y = x.Potential }).ToList();
                model.current = current.Select(x => new { name = x.GroupEntityName, y = x.CurrentOpportunity }).ToList();
                model.future = future.Select(x => new { name = x.GroupEntityName, y = x.FutureOpportunity }).ToList();

                JObject o = JObject.FromObject(model);

                return o.ToString();
            }

            internal string GetOpportunitiesJSONByCustomerType(int salespersonID, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(salespersonID);

                var data = AppService.Current.DataContext.GetOpportunitiesAggregateByCustomerType(
                    startDate, endDate, spIDs);

                var potential = filterOpportunityAggregateBase(data, "potential");
                var current = filterOpportunityAggregateBase(data, "current");
                var future = filterOpportunityAggregateBase(data, "future");

                dynamic model = new ExpandoObject();

                model.potential = potential.Select(x => new { name = x.GroupEntityName, y = x.Potential }).ToList();
                model.current = current.Select(x => new { name = x.GroupEntityName, y = x.CurrentOpportunity }).ToList();
                model.future = future.Select(x => new { name = x.GroupEntityName, y = x.FutureOpportunity }).ToList();

                JObject o = JObject.FromObject(model);

                return o.ToString();
            }

            internal string GetOpportunitiesJSONByAccountType(int salespersonID, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(salespersonID);

                var data = AppService.Current.DataContext.GetOpportunitiesAggregateByAccountType(
                    startDate, endDate, spIDs);

                var potential = filterOpportunityAggregateBase(data, "potential");
                var current = filterOpportunityAggregateBase(data, "current");
                var future = filterOpportunityAggregateBase(data, "future");

                dynamic model = new ExpandoObject();

                model.potential = potential.Select(x => new { name = x.GroupEntityName, y = x.Potential }).ToList();
                model.current = current.Select(x => new { name = x.GroupEntityName, y = x.CurrentOpportunity }).ToList();
                model.future = future.Select(x => new { name = x.GroupEntityName, y = x.FutureOpportunity }).ToList();

                JObject o = JObject.FromObject(model);

                return o.ToString();
            }

            internal string GetOpportunitiesJSONByCustomer(int salespersonID, DateTime startDate, DateTime endDate)
            {

                int[] spIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(salespersonID);

                var data = AppService.Current.DataContext.GetOpportunitiesAggregateByCustomer(
                    startDate, endDate, spIDs);

                var potential = filterOpportunityAggregateBase(data, "potential");
                var current = filterOpportunityAggregateBase(data, "current");
                var future = filterOpportunityAggregateBase(data, "future");

                dynamic model = new ExpandoObject();

                model.potential = potential.Select(x => new { name = x.GroupEntityName, y = x.Potential }).ToList();
                model.current = current.Select(x => new { name = x.GroupEntityName, y = x.CurrentOpportunity }).ToList();
                model.future = future.Select(x => new { name = x.GroupEntityName, y = x.FutureOpportunity }).ToList();

                JObject o = JObject.FromObject(model);

                return o.ToString();
            }

            private List<Domain.DataSets.OpportunitiesAggregate> filterOpportunityAggregateBase(
                List<Domain.DataSets.OpportunitiesAggregate> data,
                string targetType)
            {

                int maxCount = 15;

                IEnumerable<Domain.DataSets.OpportunitiesAggregate> tempList;

                switch (targetType)
                {

                    case "potential":
                        tempList = data.Where(x => x.Potential > 0);
                        tempList = tempList.OrderByDescending(x => x.Potential);
                        return Domain.DataSets.OpportunitiesAggregate.TruncateList(tempList, maxCount, true);

                    case "current":
                        tempList = data.Where(x => x.Potential > 0);
                        tempList = tempList.OrderByDescending(x => x.Potential);
                        return Domain.DataSets.OpportunitiesAggregate.TruncateList(tempList, maxCount, true);

                    case "future":
                        tempList = data.Where(x => x.Potential > 0);
                        tempList = tempList.OrderByDescending(x => x.Potential);
                        return Domain.DataSets.OpportunitiesAggregate.TruncateList(tempList, maxCount, true);

                    default:
                        throw new ArgumentException("targetType not recognized");
                }

            }




            #endregion


            #region SALES CALLS

            /**********************
             * 
             * ACTUAL SALES CALLS
             * 
             * *******************/

            public List<Models.CallPlanOverviewListItem> GetCallPlanOverviewListItems(int fiscalYear, int personID)
            {
                var data = AppService.Current.DataContext.GetCallPlanOverviewByPersonnel(fiscalYear, personID);
                var items = new List<Models.CallPlanOverviewListItem>();

                foreach (var d in data.Items)
                {
                    items.Add(Models.CallPlanOverviewListItem.FromCallPlanOverviewModel(d));
                }

                return items;
            }

            internal string GetSalesCallsJSON(int salesperson, DateTime startDate, DateTime endDate)
            {

                var fp = new Domain.DataSets.FilterParams();
                fp.PersonnelIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(salesperson);

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, fp);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.calls = model.Sum(x => x.CallsActual);
                totals.forecast = model.Sum(x => x.CallsForecast);

                dynamic actualSeries = GetActualDisplay();
                actualSeries.data = model.Select(x => x.CallsActual).ToList();

                dynamic forecastSeries = GetForecastDisplay();
                forecastSeries.name = "Total Monthly Forecast";
                forecastSeries.data = model.Select(x => x.CallsForecast).ToList();

                dynamic targetSeries = GetTargetDisplay();
                targetSeries.data = model.Select(x => x.CallsTarget).ToList();

                dynamic mtdSeries = GetMTDDisplay();
                mtdSeries.data = getMTDForecastCallsSeries(model);

                var allseries = new List<Object>() { actualSeries, forecastSeries, mtdSeries };

                if (AppService.Current.Settings.TargetFeatureEnabled)
                {
                    allseries.Add(targetSeries);
                    totals.target = model.Sum(x => x.CallsTarget);
                }

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();

            }

            internal string GetSalesCallsByIndustryJSON(int? industry, DateTime startDate, DateTime endDate)
            {

                var fp = new Domain.DataSets.FilterParams();
                fp.PersonnelIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                fp.IndustryIDs = industry.HasValue ? new int[] { industry.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, fp);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.calls = model.Sum(x => x.CallsActual);
                totals.forecast = model.Sum(x => x.CallsForecast);

                dynamic actualSeries = GetActualDisplay();
                actualSeries.data = model.Select(x => x.CallsActual).ToList();

                dynamic forecastSeries = GetForecastDisplay();
                forecastSeries.name = "Total Monthly Forecast";
                forecastSeries.data = model.Select(x => x.CallsForecast).ToList();

                dynamic targetSeries = GetTargetDisplay();
                targetSeries.data = model.Select(x => x.CallsTarget).ToList();

                dynamic mtdSeries = GetMTDDisplay();
                mtdSeries.data = getMTDForecastCallsSeries(model);

                var allseries = new List<Object>() { actualSeries, forecastSeries, mtdSeries };
                if (AppService.Current.Settings.TargetFeatureEnabled)
                {
                    allseries.Add(targetSeries);
                    totals.target = model.Sum(x => x.CallsTarget);
                }

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();

            }

            internal string GetSalesCallsByCustomerTypeJSON(int? customerType, DateTime startDate, DateTime endDate)
            {

                var fp = new Domain.DataSets.FilterParams();
                fp.PersonnelIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                fp.CustomerTypeIDs = customerType.HasValue ? new int[] { customerType.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, fp);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.calls = model.Sum(x => x.CallsActual);
                totals.forecast = model.Sum(x => x.CallsForecast);

                dynamic actualSeries = GetActualDisplay();
                actualSeries.data = model.Select(x => x.CallsActual).ToList();

                dynamic forecastSeries = GetForecastDisplay();
                forecastSeries.name = "Total Monthly Forecast";
                forecastSeries.data = model.Select(x => x.CallsForecast).ToList();

                dynamic targetSeries = GetTargetDisplay();
                targetSeries.data = model.Select(x => x.CallsTarget).ToList();

                dynamic mtdSeries = GetMTDDisplay();
                mtdSeries.data = getMTDForecastCallsSeries(model);

                var allseries = new List<Object>() { actualSeries, forecastSeries, mtdSeries };
                if (AppService.Current.Settings.TargetFeatureEnabled)
                {
                    allseries.Add(targetSeries);
                    totals.target = model.Sum(x => x.CallsTarget);
                }

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();
            }

            internal string GetSalesCallsByAccountTypeJSON(int? accountType, DateTime startDate, DateTime endDate)
            {

                var fp = new Domain.DataSets.FilterParams();
                fp.PersonnelIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                fp.AccountTypeIDs = accountType.HasValue ? new int[] { accountType.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, fp);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.calls = model.Sum(x => x.CallsActual);
                totals.forecast = model.Sum(x => x.CallsForecast);

                dynamic actualSeries = GetActualDisplay();
                actualSeries.data = model.Select(x => x.CallsActual).ToList();

                dynamic forecastSeries = GetForecastDisplay();
                forecastSeries.name = "Total Monthly Forecast";
                forecastSeries.data = model.Select(x => x.CallsForecast).ToList();

                dynamic targetSeries = GetTargetDisplay();
                targetSeries.data = model.Select(x => x.CallsTarget).ToList();

                dynamic mtdSeries = GetMTDDisplay();
                mtdSeries.data = getMTDForecastCallsSeries(model);

                var allseries = new List<Object>() { actualSeries, forecastSeries, mtdSeries };
                if (AppService.Current.Settings.TargetFeatureEnabled)
                {
                    allseries.Add(targetSeries);
                    totals.target = model.Sum(x => x.CallsTarget);
                }

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();
            }

            internal string GetSalesCallsByProductJSON(int? productID, DateTime startDate, DateTime endDate)
            {

                var fp = new Domain.DataSets.FilterParams();
                fp.PersonnelIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                fp.ProductIDs = productID.HasValue ? new int[] { productID.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, fp);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.calls = model.Sum(x => x.CallsActual);
                totals.forecast = model.Sum(x => x.CallsForecast);

                dynamic actualSeries = GetActualDisplay();
                actualSeries.data = model.Select(x => x.CallsActual).ToList();

                dynamic forecastSeries = GetForecastDisplay();
                forecastSeries.name = "Total Monthly Forecast";
                forecastSeries.data = model.Select(x => x.CallsForecast).ToList();

                dynamic targetSeries = GetTargetDisplay();
                targetSeries.data = model.Select(x => x.CallsTarget).ToList();

                dynamic mtdSeries = GetMTDDisplay();
                mtdSeries.data = getMTDForecastCallsSeries(model);

                var allseries = new List<Object>() { actualSeries, forecastSeries, mtdSeries };
                if (AppService.Current.Settings.TargetFeatureEnabled)
                {
                    allseries.Add(targetSeries);
                    totals.target = model.Sum(x => x.CallsTarget);
                }

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();
            }

            internal string GetSalesCallsByCustomerJSON(int? customerID, DateTime startDate, DateTime endDate)
            {

                var fp = new Domain.DataSets.FilterParams();
                fp.PersonnelIDs = AppService.Current.DataContext.GetDownstreamPersonnelIDs(AppService.Current.ViewContext.PersonID);
                fp.CustomerIDs = customerID.HasValue ? new int[] { customerID.Value } : null;

                var model = AppService.Current.DataContext.GetMonthlyData(startDate, endDate, fp);

                var months = model.Select(x => x.Period.ToString("MMM"));

                dynamic totals = new ExpandoObject();
                totals.calls = model.Sum(x => x.CallsActual);
                totals.forecast = model.Sum(x => x.CallsForecast);

                dynamic actualSeries = GetActualDisplay();
                actualSeries.data = model.Select(x => x.CallsActual).ToList();

                dynamic forecastSeries = GetForecastDisplay();
                forecastSeries.name = "Total Monthly Forecast";
                forecastSeries.data = model.Select(x => x.CallsForecast).ToList();

                dynamic targetSeries = GetTargetDisplay();
                targetSeries.data = model.Select(x => x.CallsTarget).ToList();

                dynamic mtdSeries = GetMTDDisplay();
                mtdSeries.data = getMTDForecastCallsSeries(model);

                var allseries = new List<Object>() { actualSeries, forecastSeries, mtdSeries };
                if (AppService.Current.Settings.TargetFeatureEnabled)
                {
                    allseries.Add(targetSeries);
                    totals.target = model.Sum(x => x.CallsTarget);
                }

                JObject chart = JObject.FromObject(new
                {
                    categories = months,
                    series = allseries,
                    totals = totals
                });

                return chart.ToString();
            }



            #endregion


            #region CHART STYLES
            public static ExpandoObject GetActualDisplay()
            {
                dynamic actualSeries = new ExpandoObject();
                actualSeries.name = "Actual";
                actualSeries.grouping = false;
                actualSeries.pointPadding = .25;
                actualSeries.pointPlacement = -.2;

                return actualSeries;
            }

            public static ExpandoObject GetForecastDisplay()
            {
                dynamic forecastSeries = new ExpandoObject();
                forecastSeries.name = "Forecast";
                forecastSeries.grouping = false;
                forecastSeries.pointPadding = .25;
                forecastSeries.pointPlacement = 0.2;

                return forecastSeries;
            }

            public static ExpandoObject GetTargetDisplay()
            {
                dynamic forecastSeries = new ExpandoObject();
                forecastSeries.name = "Target";
                forecastSeries.type = "line";

                return forecastSeries;
            }

            public static ExpandoObject GetMTDDisplay()
            {
                dynamic mtdSeries = new ExpandoObject();
                mtdSeries.name = "MTDForecast";
                mtdSeries.grouping = false;
                mtdSeries.color = "rgba(0, 0, 0, 0.50)";
                mtdSeries.showInLegend = false;
                mtdSeries.pointPadding = .4;
                mtdSeries.pointPlacement = -.2;

                return mtdSeries;
            }
            #endregion

        }

    }
}