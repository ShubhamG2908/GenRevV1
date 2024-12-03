using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Analysis.Models
{
    public class DrilldownListItem
    {
        
        public int ID { get; set; }

        public DateTime Period { get; set; }

        public string CompanyName { get; set; }
        public string Industry { get; set; }
        public string CustomerType { get; set; }
        public string AccountType { get; set; }
        public string ProductSKU { get; set; }

        public string SalesProfessional { get; set; }

        public int? IndustryID { get; set; }
        public int? CustomerTypeID { get; set; }
        public int? AccountTypeID { get; set; }

        public decimal? SalesActual { get; set; }
        public decimal? SalesForecast { get; set; }
        public decimal? SalesDifference { get; set; }
        public decimal? SalesVariance { get; set; }

        public decimal? GPDActual { get; set; }
        public decimal? GPDForecast { get; set; }
        public decimal? GPDDifference { get; set; }
        public decimal? GPDVariance { get; set; }

        public decimal? GPPActual { get; set; }
        public decimal? GPPForecast { get; set; }
        public decimal? GPPDifference { get; set; }
        public decimal? GPPVariance { get; set; }

        public int? CallsActual { get; set; }
        public int? CallsForecast { get; set; }
        public int? CallsDifference { get; set; }        


        public static DrilldownListItem FromCustomerDrilldownModel(Domain.DataSets.CustomerDrilldown data) {


            var model = new DrilldownListItem();

            model.ID = data.ID;
            model.Period = data.Period;
            model.CompanyName = data.CustomerName;
            model.SalesProfessional = data.PersonFirstName + " " + data.PersonLastName;

            if (AppService.Current.Settings.ProductFeatureEnabled)
            {
                model.ProductSKU = data.ProductSKU;
            }

            model.Industry = data.IndustryName;
            model.CustomerType = data.CustomerTypeName;
            model.AccountType = data.AccountTypeName;

            model.IndustryID = data.IndustryID;
            model.CustomerTypeID = data.CustomerTypeID;
            model.AccountTypeID = data.AccountTypeID;
            
            model.CallsActual = data.CallsActual;
            model.CallsDifference = data.CallsDifference;
            model.CallsForecast = data.CallsForecast;
            
            model.GPDActual = data.GrossProfitDollars;
            model.GPDDifference = data.GrossProfitDollarsDifference;
            model.GPDForecast = data.GrossProfitDollarsForecast;
            model.GPDVariance = data.GrossProfitDollarsVariance;

            model.GPPActual = data.GrossProfitPercent.HasValue ? data.GrossProfitPercent / 100 : null;
            model.GPPDifference = data.GrossProfitPercentDifference;
            model.GPPForecast = data.GrossProfitPercentForecast.HasValue ? data.GrossProfitPercentForecast / 100 : null;
            model.GPPVariance = data.GrossProfitPercentVariance;
            
            model.SalesActual = data.SalesActual;
            model.SalesDifference = data.SalesDifference;
            model.SalesForecast = data.SalesForecast;
            model.SalesVariance = data.SalesVariance;
            
            return model;
        }
        
    }
}