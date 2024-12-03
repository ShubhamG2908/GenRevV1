using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

using Genrev.Domain.Companies;

namespace Genrev.Domain.IntegrationTests.Companies
{
    [TestClass]
    public class CompaniesTests
    {

        static Genrev.Data.GenrevContext db;
        static Company defaultCompany;
        
        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext) {
            
            db  = new Genrev.Data.GenrevContext();
            defaultCompany = db.Companies.OrderBy(x => x.DateCreated).First();

        }

        [TestMethod]
        public void CompanyShouldAccessAccount() {

            var account = defaultCompany.Account;

            Assert.IsInstanceOfType(account, typeof(Accounts.Account));
            Assert.IsNotNull(account);
        }

        [TestMethod]
        public void CompanyShouldReturnNullParentCompany() {

            var parentCompany = defaultCompany.ParentCompany;

            Assert.IsNull(parentCompany);
        }

        [TestMethod]
        public void CompanyShouldAccessPersonnel() {

            var personnel = defaultCompany.Personnel.ToList();

            Assert.IsNotNull(personnel);
            CollectionAssert.AllItemsAreUnique(personnel);
            CollectionAssert.AllItemsAreNotNull(personnel);
            CollectionAssert.AllItemsAreInstancesOfType(personnel, typeof(Person));
        }

        [TestMethod]
        public void CompanyShouldAccessRoles() {

            var roles = defaultCompany.Roles.ToList();

            Assert.IsNotNull(roles);
            CollectionAssert.AllItemsAreUnique(roles);
            CollectionAssert.AllItemsAreNotNull(roles);
            CollectionAssert.AllItemsAreInstancesOfType(roles, typeof(Role));
        }

        [TestMethod]
        public void CompanyShouldAccessIndustries() {

            var items = defaultCompany.Industries.ToList();

            Assert.IsNotNull(items);
            CollectionAssert.AllItemsAreUnique(items);
            CollectionAssert.AllItemsAreNotNull(items);
            CollectionAssert.AllItemsAreInstancesOfType(items, typeof(Industry));

        }

        [TestMethod]
        public void CompanyShouldAccessCustomerTypes() {

            var items = defaultCompany.CustomerTypes.ToList();

            Assert.IsNotNull(items);
            CollectionAssert.AllItemsAreUnique(items);
            CollectionAssert.AllItemsAreNotNull(items);
            CollectionAssert.AllItemsAreInstancesOfType(items, typeof(CustomerType));

        }

        [TestMethod]
        public void CompanyShouldAccessCustomers() {

            var items = defaultCompany.Customers.ToList();

            Assert.IsNotNull(items);
            CollectionAssert.AllItemsAreUnique(items);
            CollectionAssert.AllItemsAreNotNull(items);
            CollectionAssert.AllItemsAreInstancesOfType(items, typeof(Customer));
        }
    }
}
