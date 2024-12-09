namespace Genrev.Web.App.Data.Models.Actual
{
    public class ActualGridItem
    {
        public int GridID { get; set; }
        public int ID { get; set; }

        public CommonListItems.Period Period { get; set; }

        public int CustomerID { get; set; }
        public int? PersonnelID { get; set; }
        public int? ProductID { get; set; }

        public decimal? SalesActual { get; set; }
        public decimal? SalesForecast { get; set; }
        public decimal? SalesTarget { get; set; }

        public decimal? Potential { get; set; }
        public decimal? CurrentOpportunity { get; set; }
        public decimal? FutureOpportunity { get; set; }

        public decimal? GPPActual { get; set; }
        public decimal? GPPForecast { get; set; }
        public decimal? GPPTarget { get; set; }

        public int? CallsActual { get; set; }
        public int? CallsForecast { get; set; }
        public int? CallsTarget { get; set; }
    }
}