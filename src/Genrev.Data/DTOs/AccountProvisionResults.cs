using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Data.DTOs
{

    /// <summary>
    /// Results map from dbo.ProvisionAccount
    /// </summary>
    public class AccountProvisionResults
    {
        public int AccountID { get; set; }
        public int CompanyID { get; set; }
        public int PersonnelID { get; set; }
    }
}
