using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Products
{
    public class Product
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int CompanyID { get; set; }
        public string SKU { get; set; }
        public string Description { get; set; }
        public int? ProductGroupID { get; set; }
        
        public virtual Companies.Company Company { get; set; }
        public virtual ProductGroup ProductGroup { get; set; }
        public virtual ICollection<Companies.Customer> Customers { get; set; }
        public virtual ICollection<DataSets.CustomerData> CustomerData { get; set; }
            
    }
}
