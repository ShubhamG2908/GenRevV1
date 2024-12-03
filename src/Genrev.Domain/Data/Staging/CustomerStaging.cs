using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Data.Staging
{
    public class CustomerStaging
    {

        public int ID { get; set; }

        public string ClientID { get; set; }
        public string Name { get; set; }
        public string CustomerTypeClientID { get; set; }
        public string AccountTypeClientID { get; set; }
        public string IndustryTypeClientID { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string PostalCode { get; set; }

    }
}
