using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Accounts
{

    // refs dbo.AccountActivityCodes
    public enum AccountActivityType
    {
        Created = 0,
        Locked = 1,
        Deactivated = 2,
        Unlocked = 3,
        Reactivated = 4,
        Deprovisioned = 5
    }

    public class ActivityItem
    {


        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int AccountID { get; set; }
        public AccountActivityType Type { get; set; }
        public DateTime DateOfActivity { get; set; }
        public string Note { get; set; }
        
        public virtual Account Account { get; set; }

    }
}
