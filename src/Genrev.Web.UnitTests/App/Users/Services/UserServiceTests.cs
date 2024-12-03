using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;
using System.Runtime.Caching;

namespace Genrev.UnitTests.Web.App.Users.Services
{

    [TestClass]
    public class UserServiceTests
    {

        [TestMethod]
        public void InvalidateLoginCache() {

            // need to come up with a cache interface instead of using MemoryCache directly
            // revise Dymeng framework to inlude an ICacheProvider interface and set up


        }



    }
}
