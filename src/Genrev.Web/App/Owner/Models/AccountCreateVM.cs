using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Genrev.Domain.Accounts;

namespace Genrev.Web.App.Owner.Models
{
    
    public class AccountCreateVM {

        public AccountStatus AccountStatus { get; set; }
        
        public string MasterEmail { get; set; }
        public string CompanyFullName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactGender { get; set; }
       

    }
}