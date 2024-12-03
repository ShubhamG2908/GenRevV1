using System;
using System.Collections.Generic;
using System.Configuration;

namespace Genrev.DomainServices.Data
{
    public class YearProvider
    {

        public int GetDefaultYear()
        {
            return DateTime.UtcNow.Year;
        }

        public IEnumerable<int> GetDefaultYears()
        {
            var years = new List<int>();
            var startYear = 19;
            var endYear = 1;
            int.TryParse(ConfigurationManager.AppSettings["YearMin"], out startYear);
            int.TryParse(ConfigurationManager.AppSettings["YearMax"], out endYear);
            int min = GetDefaultYear() - startYear;
            var max = GetDefaultYear() + endYear;
            while (min <= max)
            {
                years.Add(min);
                min++;
            }
            years.Reverse();
            return years;
        }

    }
}
