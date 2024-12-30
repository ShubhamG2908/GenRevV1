namespace Genrev.Web.App.Personnel.Models
{
    public class AvailabilityListItem
    {

        public int ID { get; set; }
        public int PersonnelID { get; set; }
        public int Year { get; set; }
        public int Weekdays { get; set; }
        public int? Holidays { get; set; }
        public int? VacationDays { get; set; }
        public int? AdministrationDays { get; set; }
        public int? OtherDays { get; set; }
        public double? PlannedCalls { get; set; }

        public double? CallCommittment { get; set; }
        public int? DaysAvailable { get; set; }
        
    }
}