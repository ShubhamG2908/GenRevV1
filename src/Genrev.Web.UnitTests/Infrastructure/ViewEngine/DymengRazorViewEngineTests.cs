using System;
using System.Web.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Linq;

namespace Genrev.Web.Infrastructure.ViewEngine.Tests
{
    [TestClass]
    public class DymengRazorViewEngineTests
    {

        //http://aspnetwebstack.codeplex.com/SourceControl/latest#test/System.Web.Mvc.Test/Test/RazorViewEngineTest.cs

        [TestMethod]
        public void AreaMasterLocationFormats() {
            string[] expected = new[] {
                "~/Areas/{2}/App/{1}/Views/{0}.cshtml",
                "~/Areas/{2}/App/Shared/Views/{0}.cshtml"
            };

            DymengRazorViewEngine engine = new DymengRazorViewEngine();
            
            CollectionAssert.AreEqual(expected, engine.AreaMasterLocationFormats);
        }

        [TestMethod]
        public void AreaPartialViewLoadcationFormats() {
            string[] expected = new[] {
                "~/Areas/{2}/App/{1}/Views/{0}.cshtml",
                "~/Areas/{2}/App/Shared/Views/{0}.cshtml"
            };

            DymengRazorViewEngine engine = new DymengRazorViewEngine();

            CollectionAssert.AreEqual(expected, engine.AreaPartialViewLocationFormats);
        }

        [TestMethod]
        public void AreaViewLocationFormats() {
            string[] expected = new[] {
                "~/Areas/{2}/App/{1}/Views/{0}.cshtml",
                "~/Areas/{2}/App/Shared/Views/{0}.cshtml"
            };

            DymengRazorViewEngine engine = new DymengRazorViewEngine();

            CollectionAssert.AreEqual(expected, engine.AreaViewLocationFormats);
        }

        [TestMethod]
        public void MasterLocationFormats() {
            string[] expected = new[] {
                "~/App/{1}/Views/{0}.cshtml",
                "~/App/Shared/Views/{0}.cshtml"
            };

            DymengRazorViewEngine engine = new DymengRazorViewEngine();

            CollectionAssert.AreEqual(expected, engine.MasterLocationFormats);
        }

        [TestMethod]
        public void PartialViewLocationFormats() {
            string[] expected = new[] {
                "~/App/{1}/Views/{0}.cshtml",
                "~/App/Shared/Views/{0}.cshtml"
            };

            DymengRazorViewEngine engine = new DymengRazorViewEngine();

            CollectionAssert.AreEqual(expected, engine.PartialViewLocationFormats);
        }

        [TestMethod]
        public void ViewLocationFormats() {
            string[] expected = new[] {
                "~/App/{1}/Views/{0}.cshtml",
                "~/App//Shared/Views/{0}.cshtml"
            };

            DymengRazorViewEngine engine = new DymengRazorViewEngine();

            CollectionAssert.AreEqual(expected, engine.ViewLocationFormats);
        }

        //[TestMethod]
        //public void ViewEngineSetsViewPageActivator() {

        //    Mock<IViewPageActivator> activator = new Mock<IViewPageActivator>();

        //    TestableDymengRazorViewEngine engine = new TestableDymengRazorViewEngine(activator.Object);

        //    Assert.AreEqual(activator.Object, engine.ViewPageActivator);

        //}

        [TestMethod]
        public void CreatePartialView_ReturnsRazorView() {

            TestableDymengRazorViewEngine engine = new TestableDymengRazorViewEngine();

            RazorView result = (RazorView)engine.CreatePartialView("partial");

            Assert.AreEqual("partial", result.ViewPath);
            Assert.AreEqual(string.Empty, result.LayoutPath);
            Assert.IsFalse(result.RunViewStartPages);
        }

        [TestMethod]
        public void CreateView_ReturnsRazorView() {
            TestableDymengRazorViewEngine engine = new TestableDymengRazorViewEngine();
            engine.FileExtensions = new[] { "cshtml" };

            RazorView result = (RazorView)engine.CreateView("path", "master path");

            Assert.AreEqual("path", result.ViewPath);
            Assert.AreEqual("master path", result.LayoutPath);
            CollectionAssert.AreEqual(new[] { "cshtml" }, result.ViewStartFileExtensions.ToArray());
            Assert.IsTrue(result.RunViewStartPages);
        }


        [TestMethod]
        public void FileExtensionsProperty() {

            string[] expected = new[] { "cshtml", "vbhtml" };

            DymengRazorViewEngine engine = new DymengRazorViewEngine();

            CollectionAssert.AreEqual(expected, engine.FileExtensions);
        }








        private sealed class TestableDymengRazorViewEngine : DymengRazorViewEngine
        {

            public TestableDymengRazorViewEngine() : base() { }

            public TestableDymengRazorViewEngine(IViewPageActivator activator) : base(activator) { }

            public new IViewPageActivator ViewPageActivator { get { return base.ViewPageActivator; } }

            public IView CreatePartialView(string partialPath) {
                return base.CreatePartialView(new ControllerContext(), partialPath);
            }

            public IView CreateView(string viewPath, string masterPath) {
                return base.CreateView(new ControllerContext(), viewPath, masterPath);
            }

            // This method should remain overridable in derived view engines
            protected override bool FileExists(ControllerContext controllerContext, string virtualPath) {
                return base.FileExists(controllerContext, virtualPath);
            }

        }

    }
}
