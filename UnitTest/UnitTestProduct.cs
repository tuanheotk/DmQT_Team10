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



        [TestMethod]
        public void TestDetails()
        {
            var controller = new ProductAdminController();
            var context = new Mock<HttpContextBase>();
            context.Setup(c => c.Server.MapPath("~/App_Data/0")).Returns("~/App_Data/0");
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            var result = controller.Details(0) as FilePathResult;

            Assert.IsNotNull(result);
            Assert.AreEqual("images", result.ContentType);
            Assert.AreEqual("~/App_Data/0", result.FileName);
        }
        [TestMethod]
        public void TestDelete()
        {
            var db = new DmQT10Entities();
            var product = new Product
            {
                ProductName = "ProductName",
                ProductTypeID = db.ProductTypes.First().ID,
                SalePrice = 123,
                OriginPrice = 123,
                InstallmentPrice = 123,
                Quantity = 123,
                Avatar = ""
            };

            var controller = new ProductAdminController();
            var context = new Mock<HttpContextBase>();
            var session = new Mock<HttpSessionStateBase>();
            session.Setup(s => s["USERNAME"]).Returns("abc");
            context.Setup(c => c.Session).Returns(session.Object);
            controller.ControllerContext = new ControllerContext(context.Object, new RouteData(), controller);

            using (var scope = new TransactionScope())
            {
                db.Products.Add(product);
                db.SaveChanges();
                var count = db.Products.Count();
                var result2 = controller.DeleteConfirmed(product.ID) as RedirectToRouteResult;
                Assert.IsNotNull(result2);
                Assert.AreEqual(count - 1, db.Products.Count());
            }
        }
    }
}
