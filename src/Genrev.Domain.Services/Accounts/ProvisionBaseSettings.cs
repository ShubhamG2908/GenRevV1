using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Services.Accounts
{
    public class ProvisionBaseSettings
    {

        // TODO: refactor for sproc provisioning
        public string AccountMasterEmail { get; set; }
        public string MasterCompanyFullName { get; set; }
        public string MasterCompanyName { get; set; }
        public string MasterCompanyCode { get; set; }
        public string MasterLoginFirstName { get; set; }
        public string MasterLoginLastName { get; set; }
        public string MasterLoginGender { get; set; }
        public string MasterLoginDisplayName { get; set; }
        public string MasterAccountPassword { get; set; }
        public Domain.Month FiscalYearEndingMonth { get; set; }
        
    }
}
