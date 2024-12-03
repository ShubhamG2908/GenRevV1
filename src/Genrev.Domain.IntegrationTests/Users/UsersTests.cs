using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Genrev.Data;
using Genrev.Domain.Users;

namespace Genrev.Domain.IntegrationTests.Users
{
    [TestClass]
    public class UsersTests
    {

        static GenrevContext db;
        static User defaultUser;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext) {

            db = new GenrevContext();
            defaultUser = db.Users.OrderBy(x => x.DateCreated).First();

        }

        [TestMethod]
        public void UserShouldAccessWebRoles() {

            var roles = defaultUser.Roles;

            Assert.IsNotNull(roles);

        }

        [TestMethod]
        public void UserShouldAccessAccount() {

            var account = defaultUser.Account;

            Assert.IsNotNull(account);
            Assert.IsInstanceOfType(account, typeof(Accounts.Account));
            
        }

        [TestMethod]
        public void UserShouldAccessOptions() {

            var options = defaultUser.Options;

            Assert.IsNotNull(options);

        }

        [TestMethod]
        public void UserShouldAccessMembershipDetail() {

            var md = defaultUser.MembershipDetail;

            Assert.IsNotNull(md);
            Assert.IsInstanceOfType(md, typeof(WebMembership));

        }

        [TestMethod]
        public void UserShouldAccessLoginHistory() {

            var logins = defaultUser.LoginHistory;

            Assert.IsNotNull(logins);

        }


    }
}
