using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using WebMatrix.WebData;

namespace Genrev.Web.Infrastructure.Membership
{


    public class DymengMembershipProvider : ExtendedMembershipProvider
    {


        /*************************
        * 
        * FIELDS
        * 
        ************************/

        IMembershipRepository _membershipRepository;


        /*************************
        * 
        * PROPERTIES
        * 
        ************************/



        /*************************
        * 
        * CTOR/DTOR
        * 
        ************************/

        public DymengMembershipProvider() {
            _membershipRepository = new MembershipRepository();
        }

        public DymengMembershipProvider(IMembershipRepository membershipRepository) {
            _membershipRepository = membershipRepository;
        }

        /*************************
        * 
        * PUBLIC METHODS
        * 
        ************************/


        public override string CreateUserAndAccount(string userName, string password, bool requireConfirmation, IDictionary<string, object> values) {
            string email = values["Email"].ToString();
            _membershipRepository.CreateUserAndAccount(userName, password, email);
            return null;
        }

        public override bool ValidateUser(string username, string password) {
            var user = _membershipRepository.GetUser(username);
            if (user.GetPassword() == Domain.Services.Users.Helpers.HashPassword(password)) {
                return true;
            } else {
                return false;
            }
        }

        public override string GetPassword(string username, string answer) {
            return _membershipRepository.GetPassword(username);
        }

        public override MembershipUser GetUser(string username, bool userIsOnline) {
            return _membershipRepository.GetUser(username);
        }

        /*************************
        * 
        * PRIVATE METHODS
        * 
        ************************/

















        public override string ApplicationName {
            get
            {
                throw new NotImplementedException();
            }

            set
            {
                throw new NotImplementedException();
            }
        }

        public override bool EnablePasswordReset {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool EnablePasswordRetrieval {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MaxInvalidPasswordAttempts {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MinRequiredNonAlphanumericCharacters {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int MinRequiredPasswordLength {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override int PasswordAttemptWindow {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override MembershipPasswordFormat PasswordFormat {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override string PasswordStrengthRegularExpression {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool RequiresQuestionAndAnswer {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool RequiresUniqueEmail {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override bool ChangePassword(string username, string oldPassword, string newPassword) {
            throw new NotImplementedException();
        }

        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer) {
            throw new NotImplementedException();
        }

        public override bool ConfirmAccount(string accountConfirmationToken) {
            throw new NotImplementedException();
        }

        public override bool ConfirmAccount(string userName, string accountConfirmationToken) {
            throw new NotImplementedException();
        }

        public override string CreateAccount(string userName, string password, bool requireConfirmationToken) {
            throw new NotImplementedException();
        }

        public override MembershipUser CreateUser(string username, string password, string email, string passwordQuestion, string passwordAnswer, bool isApproved, object providerUserKey, out MembershipCreateStatus status) {
            throw new NotImplementedException();
        }
        
        public override bool DeleteAccount(string userName) {
            throw new NotImplementedException();
        }

        public override bool DeleteUser(string username, bool deleteAllRelatedData) {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords) {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords) {
            throw new NotImplementedException();
        }

        public override string GeneratePasswordResetToken(string userName, int tokenExpirationInMinutesFromNow) {
            throw new NotImplementedException();
        }

        public override ICollection<OAuthAccountData> GetAccountsForUser(string userName) {
            throw new NotImplementedException();
        }

        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords) {
            throw new NotImplementedException();
        }

        public override DateTime GetCreateDate(string userName) {
            throw new NotImplementedException();
        }

        public override DateTime GetLastPasswordFailureDate(string userName) {
            throw new NotImplementedException();
        }

        public override int GetNumberOfUsersOnline() {
            throw new NotImplementedException();
        }
        
        public override DateTime GetPasswordChangedDate(string userName) {
            throw new NotImplementedException();
        }

        public override int GetPasswordFailuresSinceLastSuccess(string userName) {
            throw new NotImplementedException();
        }
        
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline) {
            throw new NotImplementedException();
        }

        public override int GetUserIdFromPasswordResetToken(string token) {
            throw new NotImplementedException();
        }

        public override string GetUserNameByEmail(string email) {
            throw new NotImplementedException();
        }

        public override bool IsConfirmed(string userName) {
            throw new NotImplementedException();
        }

        public override string ResetPassword(string username, string answer) {
            throw new NotImplementedException();
        }

        public override bool ResetPasswordWithToken(string token, string newPassword) {
            throw new NotImplementedException();
        }

        public override bool UnlockUser(string userName) {
            throw new NotImplementedException();
        }

        public override void UpdateUser(MembershipUser user) {
            throw new NotImplementedException();
        }


    }
    
}