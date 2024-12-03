using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Genrev.Domain.Users
{
    public class WebMembership
    {


        public int ID { get; set; }
        public DateTime DateCreated { get; set; }

        public string Password { get; set; }
        public string PasswordQuestion { get; set; }
        public string PasswordAnswer { get; set; }
        public bool IsApproved { get; set; }
        public bool IsLockedOut { get; set; }
        public DateTime LastActivityDate { get; set; }
        public DateTime LastLoginDate { get; set; }
        public DateTime LastPasswordChangeDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime LastLockoutDate { get; set; }
        public int FailedPasswordAttempts { get; set; }
        public DateTime FailedPasswordWindowStart { get; set; }
        public int FailedPasswordAnswerAttemptCount { get; set; }
        public DateTime FailedPasswordAnswerAttemptWindowStart { get; set; }        

        public virtual User User { get; set; }


    }
}
