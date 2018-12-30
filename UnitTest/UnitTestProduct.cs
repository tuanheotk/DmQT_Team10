using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DIENMAYQUYETTIEN2.Areas.Admin.Controllers;
using DIENMAYQUYETTIEN2.Controllers;
using DIENMAYQUYETTIEN2.Models;
using Moq;
using System.Linq;
using System.Web;
using System.Web.Routing;
using System.Collections.Generic;
using System.Web.Mvc;
using System.Transactions;
using System.IO;

namespace UnitTest
{
    [TestClass]
    public class UnitTestProduct
    {
        [TestMethod]
        public void TestIndexProduct()
        {
            var controller = new ProductAdminController();
            var context = new Mock<HttpContextBase>();
            var session = new Mock<HttpSessionStateBase>();
            context.Setup(c => c.Session).Returns(session.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            session.Setup(s => s["Username"]).Returns("abc");

            var result = controller.Index() as ViewResult;
            var db = new DmQT10Entities();


            //Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<Product>));
            Assert.AreEqual(db.Products.Count(), ((List<Product>)result.Model).Count);

            session.Setup(s => s["Username"]).Returns(null);
            var redirect = controller.Index() as RedirectToRouteResult;
            //Assert.AreEqual("Login", redirect.RouteValues["controller"]);
            Assert.AreEqual("Login", redirect.RouteValues["action"]);
        }
    }
}
