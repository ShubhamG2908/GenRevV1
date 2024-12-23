namespace Genrev.Web.App.Data.Models.Forecast
{
    public class ForecastPlanByYearListItem
    {

        public int AccountTypeID { get; set; }
        public string AccountType { get; set; }
        public double PlannedCalls { get; set; }
        public double? GoalCalls { get; set; }
        public int AccountTypeCount { get; set; }
        

    }
}