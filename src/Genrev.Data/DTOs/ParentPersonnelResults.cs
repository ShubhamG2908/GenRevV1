using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Data.DTOs
{
    public class ParentPersonnelResults
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }
        public int CompanyID { get; set; }
        public string PersonFirstName { get; set; }
        public string PersonLastName { get; set; }
        public string PersonGender { get; set; }

    }
}
