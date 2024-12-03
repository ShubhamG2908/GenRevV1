using Genrev.Domain.Companies;
using System;

namespace Genrev.Domain.Data
{
    public class ForecastLock
    {
        public int ID { get; private set; }
        public DateTime DateCreated { get; private set; }
        public int PersonnelID { get; set; }
        public int Year { get; set; }
        public virtual Person Person { get; set; }

        public ForecastLock()
        {
            DateCreated = DateTime.Now;
        }
    }
}
