using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Genrev.Domain.Accounts;
using Genrev.Domain.Products;

namespace Genrev.Domain.Companies
{
    public class Company
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int AccountID { get; set; }
        public int? ParentCompanyID { get; set; }
        public string FullName { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public Month FiscalYearEndMonth { get; set; }

        public virtual Account Account { get; set; }
        public virtual Company ParentCompany { get; set; }
        
        public virtual ICollection<Person> Personnel { get; set; }
        public virtual ICollection<Role> Roles { get; set; }
        public virtual ICollection<Industry> Industries { get; set; }
        public virtual ICollection<CustomerType> CustomerTypes { get; set; }
        public virtual ICollection<AccountType> AccountTypes { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }
        public virtual ICollection<ProductGroup> ProductGroups { get; set; }
        public virtual ICollection<Product> Products { get; set; }

        public FiscalYear GetFiscalYear(int refYear) {
            var fy = new FiscalYear();
            fy.EndDate = new DateTime(refYear, (int)FiscalYearEndMonth, 1);
            fy.StartDate = fy.EndDate.AddMonths(-11);
            return fy;
        }
        


    }
    
}
