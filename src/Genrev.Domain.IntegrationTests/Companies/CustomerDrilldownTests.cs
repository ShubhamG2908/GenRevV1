using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

using Genrev.Domain.Companies;

namespace Genrev.Domain.IntegrationTests.Companies
{
    [TestClass]
    public class CustomerDrilldownTests
    {

        static Genrev.Data.GenrevContext db;
        static Company defaultCompany;

        [ClassInitialize]
        public static void ClassInitialize(TestContext testContext) {

            db = new Genrev.Data.GenrevContext();
            defaultCompany = db.Companies.OrderBy(x => x.DateCreated).First();

        }


        [TestMethod]
        public void DrilldownShouldAccessData() {


            var subject = db.CustomerDrilldowns.FirstOrDefault();

            Assert.IsNotNull(subject);

        }

    }
}
