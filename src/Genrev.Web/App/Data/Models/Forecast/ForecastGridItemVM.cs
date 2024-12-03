using Genrev.Domain.DataSets;
using System;

namespace Genrev.Web.App.Data.Models.Forecast
{
    public abstract class BaseForecastDTO
    {
        public DateTime Period { get; set; }
        public int CustomerID { get; set; }
        public string CustomerName { get; set; }
        public int? CustomerDataID { get; set; }
        public decimal? SalesForecast { get; set; }
        public decimal? SalesTarget { get; set; }
        public int? CallsForecast { get; set; }
        public int? CallsTarget { get; set; }
        public decimal? Potential { get; set; }
        public decimal? CurrentOpportunity { get; set; }
        public decimal? FutureOpportunity { get; set; }
        public string Strategy { get; set; }
    }

    public class ForecastDTO : BaseForecastDTO
    {
        public decimal? CostForecast { get; set; }
        public decimal? CostTarget { get; set; }

        public ForecastGridItemVM ToForecastGridItemVM()
        {
            return new ForecastGridItemVM
            {
                Period = Period,
                CustomerID = CustomerID,
                CustomerName = CustomerName,
                CustomerDataID = CustomerDataID,
                SalesForecast = SalesForecast,
                SalesTarget = SalesTarget,
                GPPForecast = CustomerData.GetGPP(SalesForecast, CostForecast),
                GPPTarget = CustomerData.GetGPP(SalesTarget, CostTarget),
                CallsForecast = CallsForecast,
                CallsTarget = CallsTarget,
                Potential = Potential,
                CurrentOpportunity = CurrentOpportunity,
                FutureOpportunity = FutureOpportunity,
                Strategy = Strategy
            };
        }
    }

    public class ForecastGridItemVM : BaseForecastDTO
    {
        public string Key { get { return Period.ToShortDateString() + "_" + CustomerID.ToString() + "_" + (CustomerDataID.ToString() ?? Guid.NewGuid().ToString()); } }
        public decimal? GPPForecast { get; set; }
        public decimal? GPPTarget { get; set; }

    }
}