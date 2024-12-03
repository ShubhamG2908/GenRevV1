using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Dynamic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Home.Data
{
    public class TopBottomMatrix
    {


        public static string GetTopBottomMatrixJSON(DateTime startDate, DateTime endDate) {

            var model = AppService.Current.DataContext.GetTopBottomMatrix(startDate, endDate, AppService.Current.ViewContext.PersonnelIDs);

            dynamic topCustomers = new ExpandoObject();
            dynamic bottomCustomers = new ExpandoObject();
            dynamic topSalespersons = new ExpandoObject();
            dynamic bottomSalespersons = new ExpandoObject();

            topCustomers = model
                .Where(
                    x => x.Entity == "Customer"
                    && x.Factor == "Sales"
                    && x.Mode == "Top")
                .OrderByDescending(x => x.EntityValue)
                .ToList();

            bottomCustomers = model
                .Where(
                    x => x.Entity == "Customer"
                    && x.Factor == "Sales"
                    && x.Mode == "Bottom")
                .OrderBy(x => x.EntityValue)
                .ToList();

            topSalespersons = model
                .Where(
                    x => x.Entity == "Salesperson"
                    && x.Factor == "Sales"
                    && x.Mode == "Top")
                .OrderByDescending(x => x.EntityValue)
                .ToList();

            bottomSalespersons = model
                .Where(
                    x => x.Entity == "Salesperson"
                    && x.Factor == "Sales"
                    && x.Mode == "Bottom")
                .OrderBy(x => x.EntityValue)
                .ToList();

            JObject jObj = JObject.FromObject(new {
                topCustomers = topCustomers,
                bottomCustomers = bottomCustomers,
                topSalespersons = topSalespersons,
                bottomSalespersons = bottomSalespersons
            });

            return jObj.ToString();
        }

    }
}