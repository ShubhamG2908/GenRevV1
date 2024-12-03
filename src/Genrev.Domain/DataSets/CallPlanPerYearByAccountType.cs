using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.DataSets
{
    public class CallPlanPerYearByAccountType
    {
        public int PersonnelID { get; set; }
        public int AccountTypeID { get; set; }
        public string AccountTypeName { get; set; }
        public int PlannedCalls { get; set; }
        public int? GoalCountPerYear { get; set; }
        public int AccountTypeCount { get; set; }
        
    }
}
