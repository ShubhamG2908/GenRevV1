using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Genrev.Domain.Products;

namespace Genrev.Domain.Companies
{
    public class Customer
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int CompanyID { get; set; }
        public string Name { get; set; }

        public string Address1 { get; set; }
        public string Address2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string PostalCode { get; set; }
        public string Phone { get; set; }

        public int? TypeID { get; set; }
        public int? IndustryID { get; set; }
        public int? AccountTypeID { get; set; }

        public virtual Company Company { get; set; }
        public virtual CustomerType Type { get; set; }
        public virtual Industry Industry { get; set; }
        public virtual AccountType AccountType { get; set; }
        
        public virtual ICollection<Person> Personnel { get; set; }
        public virtual ICollection<Product> Products { get; set; }
        public virtual ICollection<DataSets.CustomerData> Data { get; set; }

        
        public decimal? GetCustomerShare() {

            return GetCustomerShare(null);   
        }

        public decimal? GetCustomerShare(Person salesperson) {

            // customer share: next year forecast / next year potential

            var nextYear = DateTime.Now.AddYears(1).Year;
            var data = Data.Where(x => x.Period.Year == nextYear);

            if (salesperson != null) {
                data = data.Where(x => x.PersonnelID == salesperson.ID);
            }

            decimal? forecast = data.Sum(x => x.SalesForecast);
            decimal? potential = data.Sum(x => x.Potential);

            if (forecast == null || potential == null || potential == 0) {
                return null;
            } else {
                return forecast / potential;
            }

        }

    }
}
