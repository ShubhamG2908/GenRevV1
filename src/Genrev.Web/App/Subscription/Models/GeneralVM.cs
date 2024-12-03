using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

using Genrev.Domain.Accounts;

namespace Genrev.Web.App.Subscription.Models
{
    
    public class GeneralVM {

        public AccountStatus AccountStatus { get; set; }
        
        public string MasterEmail { get; set; }
        public string CompanyFullName { get; set; }
        public string CompanyName { get; set; }
        public string CompanyCode { get; set; }
        public string ContactFirstName { get; set; }
        public string ContactLastName { get; set; }
        public string ContactGender { get; set; }

        public string AccountStatusText {
            get
            {
                switch (AccountStatus) {
                    case AccountStatus.Active:
                        return "Active";
                    case AccountStatus.Inactive:
                        return "Inactive";
                    case AccountStatus.Locked:
                        return "Locked";
                    case AccountStatus.Default:
                        return "Default";
                    default: return "Unknown";
                }
            }
        }


        public static GeneralVM MapFromDomain(Domain.Accounts.Account account) {

            var vm = new GeneralVM();

            var company = account.PrimaryCompany;
            var sysadmin = account.SysAdminPerson;

            vm.AccountStatus = account.Status;
            vm.CompanyCode = company.Code;
            vm.CompanyName = company.Name;
            vm.CompanyFullName = company.FullName;
            vm.ContactFirstName = sysadmin.FirstName;
            vm.ContactLastName = sysadmin.LastName;
            vm.ContactGender = sysadmin.Gender;
            vm.MasterEmail = account.Email;

            return vm;

        }

    }
}