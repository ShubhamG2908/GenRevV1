using System;
using System.Collections.Generic;

namespace Genrev.Domain.Companies
{
    public class AccountType
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int CompanyID { get; set; }
        public string Name { get; set; }
        public double? CallsPerMonthGoal { get; set; }

        public virtual Company Company { get; set; }
        public virtual ICollection<Customer> Customers { get; set; }

    }
}
