using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Data.Staging
{
    public class PersonnelStaging
    {
        public int ID { get; set; }

        public string ClientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
    }
}
