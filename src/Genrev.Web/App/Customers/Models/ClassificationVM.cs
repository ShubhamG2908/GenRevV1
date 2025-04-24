using System.Collections.Generic;

namespace Genrev.Web.App.Customers.Models
{
    public class ClassificationVM
    {

        public List<TypesListItemVM> Types { get; set; }
        public List<IndustryListItemVM> Industries { get; set; }
        public List<AccountTypeListItemVM> AccountTypes { get; set; }
        public List<AreaOfResponsibilityListItemVM> AreaOfResponsibilities { get; set; }
        
    }
}