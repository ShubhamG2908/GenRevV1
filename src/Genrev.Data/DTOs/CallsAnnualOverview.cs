using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Data.DTOs
{
    class CallsAnnualOverview
    {
        public int TypeID { get; set; }
        public string TypeName { get; set; }
        public int NumberOfAccounts { get; set; }
        public int? CallGoal { get; set; }
        public int? CallPlan { get; set; }
        public decimal? SalesForecast { get; set; }
    }
}
