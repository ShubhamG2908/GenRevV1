using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Data.Staging
{
    public class CompanyStaging
    {
        public int ID { get; set; }

        public string ClientID { get; set; }
        public string ParentClientID { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public int FiscalMonthEnd { get; set; }
        
    }
}
