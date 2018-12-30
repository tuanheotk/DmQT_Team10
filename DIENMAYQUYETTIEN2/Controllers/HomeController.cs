using DIENMAYQUYETTIEN2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Data.Entity;


namespace DIENMAYQUYETTIEN2.Controllers
{
    public class HomeController : Controller
    {
        DmQT10Entities db = new DmQT10Entities();
        public ActionResult Index()
        {
            var product = db.Products.OrderByDescending(x => x.ID).ToList();
            return View(product);
        }
        // hinh anh
        public FileResult Details(int id)
        {
            var path = Server.MapPath("~/App_Data/" + id);
            return File(path, "images");
        }

    }
}