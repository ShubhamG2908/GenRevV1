namespace Genrev.Data.DTOs
{
    class CallsAnnualOverview
    {
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public int NumberOfAccounts { get; set; }
        public double? CallGoal { get; set; }
        public double? CallPlan { get; set; }
        public decimal? SalesForecast { get; set; }
    }
}
