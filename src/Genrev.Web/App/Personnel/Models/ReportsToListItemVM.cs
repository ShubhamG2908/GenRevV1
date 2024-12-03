using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Personnel.Models
{
    public class ReportsToListItemVM
    {

        public int ID { get; set; }
        public bool ReportsTo { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        
    }
}