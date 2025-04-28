using Newtonsoft.Json.Linq;

using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;

namespace Genrev.Web.App.Home.Data
{
    public class TopBottomMatrix
    {


        public static string GetTopBottomMatrixJSON(DateTime startDate, DateTime endDate)
        {
            var model = AppService.Current.DataContext.GetTopBottomMatrix(startDate, endDate, AppService.Current.ViewContext.PersonnelIDs);

            dynamic topCustomers = new ExpandoObject();
            dynamic topSalespersons = new ExpandoObject();

            topCustomers = model
                .Where(
                    x => x.Entity == "Customer"
                    && x.Factor == "Sales")
                .OrderByDescending(x => x.EntityValue)
                .ToList();

            topSalespersons = model
                .Where(
                    x => x.Entity == "Salesperson"
                    && x.Factor == "Sales")
                .OrderByDescending(x => x.EntityValue)
                .ToList();

            JObject jObj = JObject.FromObject(new
            {
                topCustomers = topCustomers,
                topSalespersons = topSalespersons,
            });

            return jObj.ToString();
        }
        public static List<SalesByCustomer> GetSalesByCustomer(DateTime startDate, DateTime endDate)
        {
            var model = AppService.Current.DataContext.GetTopBottomMatrix(startDate, endDate, AppService.Current.ViewContext.PersonnelIDs);
            return model
                .Where(
                    x => x.Entity == "Customer"
                    && x.Factor == "Sales")
                .OrderByDescending(x => x.EntityValue)
                .Select(s => new SalesByCustomer()
                {
                    CustomerID = s.EntityID,
                    CustomerName = s.EntityName,
                    SalesAmount = s.EntityValue ?? 0,
                    YTDForecast = s.YTDForecast ?? 0
                }).ToList();
        }
        public static List<SalesBySalesperson> GetSalesBySalesperson(DateTime startDate, DateTime endDate)
        {
            var model = AppService.Current.DataContext.GetTopBottomMatrix(startDate, endDate, AppService.Current.ViewContext.PersonnelIDs);
            return model
                .Where(
                   x => x.Entity == "Salesperson"
                    && x.Factor == "Sales")
                .OrderByDescending(x => x.EntityValue)
                .Select(s => new SalesBySalesperson()
                {
                    SalespersonID = s.EntityID,
                    SalespersonName = s.EntityName,
                    SalesAmount = s.EntityValue ?? 0,
                    YTDForecast = s.YTDForecast ?? 0
                }).ToList();
        }
    }
}