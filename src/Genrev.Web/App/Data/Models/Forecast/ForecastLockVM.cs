using System.Collections.Generic;

namespace Genrev.Web.App.Data.Models.Forecast
{
    public class ForecastLockVM
    {
        public int SelectedYear { get; set; }
        public IEnumerable<int> AvailableYears { get; set; }
    }
}