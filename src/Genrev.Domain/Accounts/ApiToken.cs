using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Accounts
{
    public class ApiToken
    {
        
        public int ID { get; set; }
        public int AccountID { get; set; }

        public string Value { get; set; }
        public DateTime GenerationDate { get; set; }
        public DateTime ExpirationDate { get; set; }

        public virtual Account Account { get; set; }
        
    }
}
