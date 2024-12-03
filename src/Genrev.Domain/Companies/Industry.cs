using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Companies
{
    public class Industry
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int CompanyID { get; set; }
        public string Name { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }

    }
}
