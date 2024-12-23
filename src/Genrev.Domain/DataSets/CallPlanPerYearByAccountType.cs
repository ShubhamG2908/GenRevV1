namespace Genrev.Domain.DataSets
{
    public class CallPlanPerYearByAccountType
    {
        public int PersonnelID { get; set; }
        public int AccountTypeID { get; set; }
        public string AccountTypeName { get; set; }
        public double PlannedCalls { get; set; }
        public double? GoalCountPerYear { get; set; }
        public int AccountTypeCount { get; set; }
        
    }
}
