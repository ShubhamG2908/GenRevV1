using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Customers.Models
{
    public class EditCustomerVM
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string PostalCode { get; set; }
        public string Country { get; set; }
        public string Phone { get; set; }

        public CommonListItems.CustomerType CustomerTypeCLI { get; set; }
        public CommonListItems.Industry IndustryCLI { get; set; }
        public CommonListItems.AccountType AccountTypeCLI { get; set; }
        
        public List<CommonListItems.CustomerType> CustomerTypesList { get; set; }
        public List<CommonListItems.Industry> IndustriesList { get; set; }
        public List<CommonListItems.AccountType> AccountTypesList { get; set; }

        

    }
}