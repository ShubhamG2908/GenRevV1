using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.MemberBar.Models
{
    public class ViewAsVM
    {

        public int PersonnelID { get; set; }
        public string Name { get; set; }

        public Domain.Companies.Person Person { get; set; }
        
    }
}