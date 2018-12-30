using DIENMAYQUYETTIEN2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DIENMAYQUYETTIEN2.Areas.Admin.Controllers
{
    public class InstallmentBillDetailAdminController : Controller
    {
        private DmQT10Entities db = new DmQT10Entities();
        // GET: Admin/InstallmentBillDetailAdmin
        // Get Sale Price
        public int InstallmentPrice(int ProductID)
        {
            return db.Products.Find(ProductID).InstallmentPrice;
        }
        // GET: Admin/CashbillDetailAdmin
        public ActionResult Index()
        {
            if (Session["Username"] != null)
            {
                if (Session["IBillDetail"] == null)
                {
                    Session["IBillDetail"] = new List<InstallmentBillDetail>();
                }
                return PartialView(Session["IBillDetail"]);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        // GET: /Admin/CashBillDetails/Details/5
        public int DonGiaBan(int ProductID)
        {
            return db.Products.Find(ProductID).SalePrice;
        }

        // GET: /Admin/CashBillDetails/Create
        public PartialViewResult Create()
        {
            ViewBag.ProductID = new SelectList(db.Products, "ID", "ProductName");
            var ibillDetail = new InstallmentBillDetail();
            ibillDetail.Quantity = 1;
            ibillDetail.BillID = 0;
            return PartialView(ibillDetail);
        }

        // POST: Admin/CashBillDetails/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2(InstallmentBillDetail model)
        {
            if (ModelState.IsValid)
            {
                model.ID = Environment.TickCount;
                model.Product = db.Products.Find(model.ProductID);
                var CTHoaDonTG = Session["IBillDetail"] as List<InstallmentBillDetail>;
                if (CTHoaDonTG == null)
                    CTHoaDonTG = new List<InstallmentBillDetail>();
                CTHoaDonTG.Add(model);
                Session["IBillDetail"] = CTHoaDonTG;
                return RedirectToAction("Create", "InstallmentBillAdmin");
            }

            ViewBag.ProductID = new SelectList(db.Products, "ID", "ProductName", model.ProductID);
            return View("Create", model);
        }

        public PartialViewResult Edit3()
        {
            ViewBag.ProductID = new SelectList(db.Products, "ID", "ProductName");
            var model = new CashBillDetail();
            model.BillID = 0;
            model.Quantity = 1;
            return PartialView(model);
        }
        // edit
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit2(CashBillDetail model)
        {
            if (ModelState.IsValid)
            {
                model.ID = Environment.TickCount;
                model.Product = db.Products.Find(model.ProductID);
                var ctcashBill = Session["ctcashBill"] as List<CashBillDetail>;
                if (ctcashBill == null)
                    ctcashBill = new List<CashBillDetail>();
                ctcashBill.Add(model);
                Session["ctcashBill"] = ctcashBill;
                return Redirect(ControllerContext.HttpContext.Request.UrlReferrer.ToString());
            }

            ViewBag.ProductID = new SelectList(db.Products, "ID", "ProductName", model.ProductID);
            return View("Create", model);
        }

        // GET: Admin/CashBillDetails/Edit/5
        public PartialViewResult Edit(int id)
        {

            List<CashBillDetail> cbDetails = db.CashBillDetails.Where(c => c.BillID == id).ToList();
            if (Session["ctcashBill"] == null)
                Session["ctcashBill"] = new List<CashBillDetail>();
            ViewBag.cbDetails = cbDetails;
            ViewBag.ctcashBill = Session["ctcashBill"];
            return PartialView();
        }

        // POST: Admin/CashBillDetails/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CashBillDetail cashBillDetail)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cashBillDetail).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.BillID = new SelectList(db.CashBills, "ID", "BillCode", cashBillDetail.BillID);
            ViewBag.ProductID = new SelectList(db.Products, "ID", "ProductCode", cashBillDetail.ProductID);
            return View(cashBillDetail);
        }

        // GET: Admin/CashBillDetails/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashBillDetail cashBillDetail = db.CashBillDetails.Find(id);
            if (cashBillDetail == null)
            {
                return HttpNotFound();
            }
            return View(cashBillDetail);
        }

        // POST: Admin/CashBillDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CashBillDetail cashBillDetail = db.CashBillDetails.Find(id);
            db.CashBillDetails.Remove(cashBillDetail);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        //Login
        public ActionResult Login()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Login(ACCOUNT acc)
        {
            if (ModelState.IsValid)
            {
                using (DmQT10Entities db = new DmQT10Entities())
                {
                    var obj = db.ACCOUNTs.Where(a => a.USERNAME.Equals(acc.USERNAME) && a.PASSWORD.Equals(acc.PASSWORD)).FirstOrDefault();

                    if (obj != null)
                    {
                        Session["Username"] = obj.USERNAME.ToString();
                        Session["FullName"] = obj.FULLNAME.ToString();
                        return RedirectToAction("Index");
                    }
                }
            }
            return View(acc);
        }
        //Logout
        [HttpPost]
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon(); // it will clear the session at the end of request
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}