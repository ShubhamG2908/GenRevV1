using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dymeng.Collections;

using Genrev.Domain.Companies;
using Genrev.Domain.Users;
using System.Collections.ObjectModel;

namespace Genrev.Domain.Accounts
{

    // refs dbo.AccountStatuses
    public enum AccountStatus
    {
        Default = 0,
        Active = 1,
        Locked = 2,
        Inactive = 3
    }




    public class Account
    {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public AccountStatus Status { get; set; }
        public string Email { get; set; }

        public bool ApiEnabled { get; set; }
        public string ApiKey { get; set; }
        public string ApiPassword { get; set; }
        public bool AllowApiIpBypass { get; set; }

        public virtual EntityCollection<ActivityItem> Activity { get; protected set; }
        public virtual ICollection<Company> Companies { get; set; }
        public virtual ICollection<User> Users { get; set; }
        public virtual ICollection<ApiToken> ApiTokens { get; set; }
        public virtual ICollection<ApiIpWhitelistItem> ApiIpWhitelistItems { get; set; }

        

        public void AddActivity(ActivityItem item) {
            Activity = EntityCollectionHelper.AddItem(Activity, item);
        }

        public void AddActivity(AccountActivityType type) {
            var activity = new ActivityItem();
            activity.Account = this;
            activity.DateOfActivity = DateTime.UtcNow;
            activity.Type = type;
            Activity = EntityCollectionHelper.AddItem(Activity, activity);
        }

        public void AddActivity(AccountActivityType type, string note) {
            var activity = new ActivityItem();
            activity.Account = this;
            activity.DateOfActivity = DateTime.UtcNow;
            activity.Type = type;
            activity.Note = note;
            Activity = EntityCollectionHelper.AddItem(Activity, activity);
        }
        

        public Person SysAdminPerson {
            get
            {
                var q = from c in Companies
                        from p in c.Personnel
                        from r in p.Roles
                        where r.IsSysAdministrator == true
                        select p;

                return q.First();
            }
        }

        public Company PrimaryCompany { get
            {
                return Companies.Single();
            }
        }

        public User SysAdminUser {
            get
            {
                var q = from u in Users
                        where u.Email == this.Email
                        select u;
                return q.First();
            }
        }

        public string StatusText {
            get {
                switch (Status) {
                    case AccountStatus.Active: return "Active";
                    case AccountStatus.Default: return "Default";
                    case AccountStatus.Inactive: return "Inactive";
                    case AccountStatus.Locked: return "Locked";
                    default: throw new ArgumentOutOfRangeException("Account.Status not registered enum member");
                }   
            }
        }



    }
    
}
