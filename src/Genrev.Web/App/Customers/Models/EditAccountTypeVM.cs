using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Customers.Models
{
    public class EditAccountTypeVM
    {

        public int ID { get; set; }
        public string Name { get; set; }
        public int? CallsPerMonthGoal { get; set; }
    }
}