using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Customers.Models
{
    public class ClassificationVM
    {

        public List<TypesListItemVM> Types { get; set; }
        public List<IndustryListItemVM> Industries { get; set; }
        public List<AccountTypeListItemVM> AccountTypes { get; set; }
        
    }
}