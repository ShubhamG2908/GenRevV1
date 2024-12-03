using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Services.Accounts
{
    public class Provision
    {

        public static bool ProvisionAccount(ProvisionBaseSettings settings) {

            var context = new Genrev.Data.GenrevContext();

            // run the sql sproc and get the results
            Genrev.Data.DTOs.AccountProvisionResults results = context.ProvisionAccount(
                settings.AccountMasterEmail, settings.MasterCompanyFullName, settings.MasterCompanyName, settings.MasterCompanyCode,
                (int)settings.FiscalYearEndingMonth, settings.MasterLoginFirstName, settings.MasterLoginLastName,
                settings.MasterLoginDisplayName, settings.MasterLoginGender
                );

            if (results == null) {
                throw new InvalidOperationException("Unable to provision account, see sproc logs for details");
            }

            // create a user for this account
            var user = new Domain.Users.User();
            var membership = new Domain.Users.WebMembership();

            user.AccountID = results.AccountID;
            user.DisplayName = settings.MasterLoginDisplayName;
            user.Email = settings.AccountMasterEmail;
            user.MembershipDetail = membership;
            user.PersonnelID = results.PersonnelID;

            membership.ID = user.ID;
            membership.IsApproved = true;
            membership.IsLockedOut = false;
            membership.Password = Users.Helpers.HashPassword(settings.MasterAccountPassword);
            membership.User = user;

            try {

                context.Users.Add(user);
                //context.Database.Log = s => System.Diagnostics.Debug.WriteLine(s);
                context.SaveChanges();

                return true;

            } catch (Exception e) {

                System.Diagnostics.Debug.WriteLine("EXCEPTION======================");
                System.Diagnostics.Debug.WriteLine(e.ToString());

                // TODO: parameterize this
                context.Database.ExecuteSqlCommand("DELETE FROM dbo.Accounts WHERE AccountEmail = '" + settings.AccountMasterEmail + "'");

                return false;
            }

        }

    }
}
