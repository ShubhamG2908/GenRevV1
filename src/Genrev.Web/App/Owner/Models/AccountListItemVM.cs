using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Genrev.Domain.Accounts;

namespace Genrev.Web.App.Owner.Models
{
    public class AccountListItemVM
    {

        public int ID { get; set; }
        public AccountStatus StatusValue { get; set; }
        public string Status { get; set; }
        public string Email { get; set; }
        public string Company { get; set; }
        public string EditLink { get; set; }

    }
}