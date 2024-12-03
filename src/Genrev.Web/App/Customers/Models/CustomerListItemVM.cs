using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Genrev.Domain.Companies;

namespace Genrev.Web.App.Customers.Models
{
    public class CustomerListItemVM
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }
        public string CustomerType { get; set; }
        public string Industry { get; set; }
        public string AccountType { get; set; }

    }
}