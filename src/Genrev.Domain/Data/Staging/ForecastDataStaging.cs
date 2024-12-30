using System;

namespace Genrev.Domain.Data.Staging
{
    public class ForecastDataStaging
    {

        public int ID { get; set; }
        public DateTime Period { get; set; }
        public string CustomerClientID { get; set; }
        public string PersonClientID { get; set; }

        public decimal? SalesForecast { get; set; }
        public decimal? SalesTarget { get; set; }
        public decimal? GPPForecast { get; set; }
        public decimal? GPPTarget { get; set; }
        public double? CallsForecast { get; set; }
        public double? CallsTarget { get; set; }
        public decimal? Potential { get; set; }
        public decimal? CurrentOpportunity { get; set; }
        public decimal? FutureOpportunity { get; set; }
        public string Strategy { get; set; }
        public decimal? MarketShare { get; set; }
        public decimal? AtRisk { get; set; }
        public string RiskExplanation { get; set; }

    }
}
