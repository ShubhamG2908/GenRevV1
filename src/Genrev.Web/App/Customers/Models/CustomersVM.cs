using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Customers.Models
{
    public class CustomersVM
    {

        public List<CustomerListItemVM> CustomerListItems { get; set; }
        public List<CommonListItems.CustomerType> CustomerTypes { get; set; }
        public List<CommonListItems.Industry> IndustryTypes { get; set; }

    }
}