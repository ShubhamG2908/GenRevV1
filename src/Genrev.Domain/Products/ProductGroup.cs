using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Products
{
    public class ProductGroup
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int CompanyID { get; set; }
        public int? ParentID { get; set; }
        public string Name { get; set; }
        
        public virtual Companies.Company Company { get; set; }
        public virtual ProductGroup Parent { get; set; }

    }
}
