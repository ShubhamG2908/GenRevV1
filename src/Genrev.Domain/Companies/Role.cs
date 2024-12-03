using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dymeng.Collections;

namespace Genrev.Domain.Companies
{
    public class Role
    {


        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int CompanyID { get; set; }
        // these two are created on provision of a new company only
        public bool IsSysAdministrator { get; set; }
        public bool IsSysSalesPro { get; set; }
        
        public string Name { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        
        public virtual Company Company { get; set; }
        public virtual EntityCollection<Person> Personnel { get; protected set; }
        
    }
}
