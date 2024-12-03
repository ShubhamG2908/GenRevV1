namespace Genrev.Web.App.Data.Models.Forecast
{
    public class ForecastLockDTO
    {
        public int Year { get; set; }
        public int PersonnelID { get; set; }
        public string PersonnelLastName { get; set; }
        public string PersonnelFirstName { get; set; }
        public bool IsLocked { get; set; }
    }
}