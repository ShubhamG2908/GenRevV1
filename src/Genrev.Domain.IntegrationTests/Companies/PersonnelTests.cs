using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

using Genrev.Domain.Companies;

namespace Genrev.Domain.IntegrationTests.Companies
{
    [TestClass]
    public class PersonnelTests
    {

        static Genrev.Data.GenrevContext db;
        static Person defaultPerson;

        [ClassInitialize]
        public static void Initialize(TestContext testContext) {
            db = new Genrev.Data.GenrevContext();
            defaultPerson = db.Personnel.Where(x => x.FirstName == "Jack" && x.LastName == "Leach").First();
        }


        [TestMethod]
        public void ModelBuildsPersonnelAvailability() {

            var sut = db.PersonnelAvailability.FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsInstanceOfType(sut, typeof(PersonnelAvailability));
        }

        [TestMethod]
        public void PersonFetchesAvailability() {

            var sut = db.Personnel.Where(x => x.Availability.Count > 0).FirstOrDefault();

            if (sut == null) {
                Assert.Inconclusive("null");
            }

            Assert.IsTrue(sut.Availability.Count > 0);

        }


        [TestMethod]
        public void PersonHierarchyShouldPreventRecursion() {
            
            var hierarchy = db.GetDownstreamPersonnel(defaultPerson.ID).ToList();

            Assert.IsTrue(hierarchy.Count == 8);
            CollectionAssert.AllItemsAreUnique(hierarchy);
            CollectionAssert.AllItemsAreNotNull(hierarchy);
            CollectionAssert.AllItemsAreInstancesOfType(hierarchy, typeof(Person));
            
        }

        [TestMethod]
        public void PersonShouldAccessCompany() {

            var company = defaultPerson.Company;

            Assert.IsNotNull(company);
            Assert.IsInstanceOfType(company, typeof(Company));

        }

        [TestMethod]
        public void PersonShouldAccessCustomers() {

            var customers = defaultPerson.Customers.ToList();

            Assert.IsNotNull(customers);
            CollectionAssert.AllItemsAreNotNull(customers);
            CollectionAssert.AllItemsAreUnique(customers);
            CollectionAssert.AllItemsAreInstancesOfType(customers, typeof(Customer));
        }

        [TestMethod]
        public void PersonShouldAccessRoles() {
            var roles = defaultPerson.Roles.ToList();

            Assert.IsNotNull(roles);
            CollectionAssert.AllItemsAreNotNull(roles);
            CollectionAssert.AllItemsAreUnique(roles);
            CollectionAssert.AllItemsAreInstancesOfType(roles, typeof(Role));
        }

    }
}
