using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Genrev.Web.App.Owner.Models
{
    public class AccountListVM
    {
        public List<AccountListItemVM> Items { get; set; } = new List<AccountListItemVM>();
    }
}