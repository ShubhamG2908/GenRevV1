using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Data.Staging
{
    public class MonthlyDataStaging
    {

        public int ID { get; set; }
        public DateTime Period { get; set; }
        public string CustomerClientID { get; set; }
        public string PersonClientID { get; set; }
        public string ProductClientID { get; set; }

        public decimal? SalesActual { get; set; }
        public decimal? SalesTarget { get; set; }
        public decimal? CostActual { get; set; }
        public decimal? CostTarget { get; set; }
        public decimal? CallsActual { get; set; }
        public decimal? CallsTarget { get; set; }
        public decimal? Potential { get; set; }
        public decimal? CurrentOpportunity { get; set; }
        public decimal? FutureOpportunity { get; set; }

    }
}
