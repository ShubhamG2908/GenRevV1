using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Data;
using System.Data.SqlClient;
using Dymeng.Data.SqlClient;
using Genrev.Data;

namespace Genrev.Web.Infrastructure.Membership
{

    public interface IMembershipRepository
    {
        MembershipUser GetUser(string username);
        string GetPassword(string username);
        int CreateUserAndAccount(string username, string password, string email);
    }


    public class MembershipRepository : IMembershipRepository
    {


        /*************************
        * 
        * FIELDS
        * 
        ************************/
        const string DEFAULT_PROVIDER_NAME = "DymengMembershipProvider";

        private GenrevContext dataContext;
        private string membershipProviderName;
        
        /*************************
        * 
        * CONSTRUCTOR
        * 
        ************************/

        public MembershipRepository() {
            membershipProviderName = DEFAULT_PROVIDER_NAME;
            dataContext = new GenrevContext();
        }
        

        /*************************
        * 
        * PUBLIC METHODS
        * 
        ************************/

        public int CreateUserAndAccount(string username, string password, string email) {
            throw new NotImplementedException();
            //return userService.CreateWebUserAndAccount(username, password, email);
        }

        public string GetPassword(string username) {
            return dataContext.Users.Where(x => x.Email == username).FirstOrDefault()?.MembershipDetail.Password;
        }


        public MembershipUser GetUser(string username) {


            var entity = dataContext.Users.Where(x => x.Email == username).FirstOrDefault()?.MembershipDetail;

            if (entity == null) {
                return new MembershipUser(
                    providerName: membershipProviderName,
                    name: "",
                    providerUserKey: null,
                    email: "",
                    passwordQuestion: "",
                    comment: "",
                    isApproved: false,
                    isLockedOut: true,
                    creationDate: DateTime.UtcNow,
                    lastLoginDate: DateTime.UtcNow,
                    lastActivityDate: DateTime.UtcNow,
                    lastPasswordChangedDate: DateTime.UtcNow,
                    lastLockoutDate: DateTime.UtcNow
                    );
            } else {
                return new MembershipUser(
                    providerName: membershipProviderName,
                    name: entity.User.Email,
                    providerUserKey: null,
                    email: entity.User.Email,
                    passwordQuestion: entity.PasswordQuestion,
                    comment: "",
                    isApproved: entity.IsApproved,
                    isLockedOut: entity.IsLockedOut,
                    creationDate: entity.CreationDate,
                    lastLoginDate: entity.LastLoginDate,
                    lastActivityDate: entity.LastActivityDate,
                    lastPasswordChangedDate: entity.LastPasswordChangeDate,
                    lastLockoutDate: entity.LastLockoutDate
                    );
            }

        }

        /*************************
        * 
        * PRIVATE METHODS
        * 
        ************************/

        

    }
}