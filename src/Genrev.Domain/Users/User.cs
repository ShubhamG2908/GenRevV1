using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Users
{
    public class User {

        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public int AccountID { get; set; }
        public int? PersonnelID { get; set; }
        public string Email { get; set; }
        public string DisplayName { get; set; }

        public virtual Accounts.Account Account { get; set; }
        //public virtual Companies.Person Person { get; set; }
        public virtual ICollection<WebLoginHistory> LoginHistory { get; set; }
        public virtual WebMembership MembershipDetail { get; set; }
        public virtual ICollection<WebRole> Roles { get; set; }
        public virtual ICollection<WebUserOption> Options { get; set; }

        
    }
}
