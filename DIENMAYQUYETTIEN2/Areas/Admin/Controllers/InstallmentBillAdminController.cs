using DIENMAYQUYETTIEN2.Areas.Admin.Models;
using DIENMAYQUYETTIEN2.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DIENMAYQUYETTIEN2.Areas.Admin.Controllers
{
    public class InstallmentBillAdminController : Controller
    {
        DmQT10Entities db = new DmQT10Entities();
        // GET: Admin/InstallmentBillAdmin
        // GET: Admin/CashbillAdmin

        public int setSessionTaken(int taken)
        {
            Session["Taken"] = taken;
            return (int)Session["Taken"];
        }
        public ActionResult Index()
        {
            if (Session["Username"] != null)
            {
                return View(db.InstallmentBills.ToList());
            }
            else
            {
                return RedirectToAction("Login");
            }

        }


        // GET: Admin/CashBills/Create
        [HttpGet]
        public ActionResult Create()
        {
            if (Session["Username"] != null)
            {
                ViewBag.CustomerID = new SelectList(db.Customers, "ID", "CustomerCode");
                return View(Session["IBill"]);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }

        // POST: Admin/CashBills/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(InstallmentBill model)
        {
            checkib(model);
            if (ModelState.IsValid)
            {
                Session["IBill"] = model;
            }
            ViewBag.CustomerID = new SelectList(db.Customers, "ID", "CustomerCode", model.CustomerID);
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2()
        {
            using (var scope = new TransactionScope())
                try
                {
                    var iBill = Session["IBill"] as InstallmentBill;
                    var CTHoaDonTG = Session["IBillDetail"] as List<InstallmentBillDetail>;
                    iBill.Date = DateTime.Now;
                    iBill.GrandTotal = (int)Session["total"];
                    iBill.Taken = (int)Session["Taken"];
                    iBill.Remain = ((int)Session["total"] - (int)Session["Taken"]);

                    db.InstallmentBills.Add(iBill);
                    db.SaveChanges();

                    foreach (var chiTiet in CTHoaDonTG)
                    {
                        chiTiet.BillID = iBill.ID;
                        chiTiet.Product = null;
                        db.InstallmentBillDetails.Add(chiTiet);
                    }
                    db.SaveChanges();
                    scope.Complete();

                    Session["IBill"] = null;
                    Session["IBillDetail"] = null;
                    Session["total"] = null;
                    Session["Taken"] = null;
                    TempData["message"] = "Tạo hóa đơn thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            return View("Index");
        }

        // GET: Admin/CashBills/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashBill cashBill = db.CashBills.Find(id);
            if (cashBill == null)
            {
                return HttpNotFound();
            }
            Session["CashBill"] = null;
            return View(cashBill);
        }

        // POST: Admin/CashBills/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CashBill cashBill)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    db.Entry(cashBill).State = EntityState.Modified;
                    Session["CashBill"] = cashBill;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            }
            return View(cashBill);
        }
        // Edit 2 copy Create 2
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit2()
        {
            using (var scope = new TransactionScope())
                try
                {
                    var cashBill = Session["CashBill"] as CashBill;
                    var ctcashBill = Session["ctcashBill"] as List<CashBillDetail>;
                    cashBill.Date = DateTime.Now;
                    cashBill.GrandTotal = (int)Session["total"];

                    db.Entry(cashBill).State = EntityState.Modified;
                    db.SaveChanges();

                    foreach (var chiTiet in ctcashBill)
                    {
                        chiTiet.BillID = cashBill.ID;
                        chiTiet.Product = null;
                        db.CashBillDetails.Add(chiTiet);
                    }
                    db.SaveChanges();
                    scope.Complete();

                    Session["ctcashBill"] = null;
                    Session["total"] = null;
                    TempData["message"] = "Chỉnh sửa hóa đơn thành công.";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                }
            return View("Edit");
        }

        // GET: Admin/CashBills/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CashBill cashBill = db.CashBills.Find(id);
            if (cashBill == null)
            {
                return HttpNotFound();
            }
            return View(cashBill);
        }

        private void checkib(InstallmentBill model)
        {
            if (model.Shipper == null || model.Shipper.Equals(""))
                ModelState.AddModelError("Shipper", "Shipper không được để trống");
        }

        // POST: Admin/CashBills/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CashBill cashBill = db.CashBills.Find(id);
            db.CashBills.Remove(cashBill);
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
        public ActionResult Print(int id)
        {
            var order = db.InstallmentBills.FirstOrDefault(o => o.ID == id);
            if (order != null)
            {
                PrintInstallmentBillModel rp = new PrintInstallmentBillModel();
                rp.BillCode = order.BillCode;
                rp.CustomerID = order.CustomerID;
                rp.Date = order.Date;
                rp.Shipper = order.Shipper;
                rp.Note = order.Note;
                rp.Method = order.Method;
                rp.Period = order.Period;
                rp.GrandTotal = order.GrandTotal;
                rp.Taken = order.Taken;
                rp.Remain = order.Remain;
                rp.InstallmentBillDetail = order.InstallmentBillDetails.ToList();
                return PartialView(rp);
            }
            else
            {
                return View();
            }
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