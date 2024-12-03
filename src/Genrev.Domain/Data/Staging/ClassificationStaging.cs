using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Data.Staging
{
    
    public class AccountTypeStaging
    {
        public int ID { get; set; }
        public string ClientID { get; set; }
        public string Name { get; set; }
        public int? CallsPerMonthGoal { get; set; }
    }


    public class CustomerTypeStaging
    {
        public int ID { get; set; }
        public string ClientID { get; set; }
        public string Name { get; set; }
    }

    public class IndustryTypeStaging
    {
        public int ID { get; set; }
        public string ClientID { get; set; }
        public string Name { get; set; }
    }

}
