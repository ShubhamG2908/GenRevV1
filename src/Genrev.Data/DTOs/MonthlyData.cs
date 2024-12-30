using System;

namespace Genrev.Data.DTOs
{
    class MonthlyData
    {
        public DateTime Period { get; set; }
        public decimal? SalesActual { get; set; }
        public decimal? SalesForecast { get; set; }
        public decimal? SalesTarget { get; set; }
        public decimal? Potential { get; set; }
        public decimal? CurrentOpportunity { get; set; }
        public decimal? FutureOpportunity { get; set; }
        public decimal? CostActual { get; set; }
        public decimal? CostForecast { get; set; }
        public decimal? CostTarget { get; set; }
        public double? CallsActual { get; set; }
        public double? CallsForecast { get; set; }
        public double? CallsTarget { get; set; }
    }
}
