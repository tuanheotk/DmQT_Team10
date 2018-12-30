using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DIENMAYQUYETTIEN2.Areas.Admin.Controllers;
using DIENMAYQUYETTIEN2.Controllers;
using DIENMAYQUYETTIEN2.Models;

namespace UnitTest
{
    [TestClass]
    public class UnitTestCashBill
    {
        [TestMethod]
        public void TestIndexCashBill()
        {
            var controller = new CashbillAdminController();
            var context = new Mock<HttpContextBase>();
            var session = new Mock<HttpSessionStateBase>();
            context.Setup(c => c.Session).Returns(session.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);
            session.Setup(s => s["Username"]).Returns("abc");

            var result = controller.Index() as ViewResult;
            var db = new DmQT10Entities();


            //Assert.IsNotNull(result);
            Assert.IsInstanceOfType(result.Model, typeof(List<CashBill>));
            Assert.AreEqual(db.CashBills.Count(), ((List<CashBill>)result.Model).Count);

            session.Setup(s => s["Username"]).Returns(null);
            var redirect = controller.Index() as RedirectToRouteResult;
            //Assert.AreEqual("Login", redirect.RouteValues["controller"]);
            Assert.AreEqual("Login", redirect.RouteValues["action"]);

        }
    }
}
