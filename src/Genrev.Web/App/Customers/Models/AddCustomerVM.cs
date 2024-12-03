using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Customers.Models
{
    public class AddCustomerVM
    {

        public List<CommonListItems.CustomerType> CustomerTypesList { get; set; }
        public List<CommonListItems.Industry> IndustriesList { get; set; }
        public List<CommonListItems.AccountType> AccountTypesList { get; set; }
    }
}