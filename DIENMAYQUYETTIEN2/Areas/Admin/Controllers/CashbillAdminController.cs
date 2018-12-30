using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DIENMAYQUYETTIEN2.Models;
using System.Web.Security;
using System.Transactions;
using System.Data.Entity;
using System.Net;
using DIENMAYQUYETTIEN2.Areas.Admin.Models;

namespace DIENMAYQUYETTIEN2.Areas.Admin.Controllers
{
    public class CashbillAdminController : Controller
    {
        DmQT10Entities db = new DmQT10Entities();


        public int SalePrice(int ProductID)
        {
            return db.Products.Find(ProductID).SalePrice;
        }
        // GET: Admin/CashbillAdmin
        public ActionResult Index()
        {
            if (Session["Username"] != null)
            {
                return View(db.CashBills.ToList());
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
                return View(Session["CashBill"]);
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
        public ActionResult Create(CashBill model)
        {
            checkCashBill(model);
            if (ModelState.IsValid)
            {
                Session["CashBill"] = model;
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create2()
        {
            using (var scope = new TransactionScope())
                try
                {
                    var cashBill = Session["CashBill"] as CashBill;
                    var ctcashBill = Session["ctcashBill"] as List<CashBillDetail>;
                    cashBill.Date = DateTime.Now;
                    cashBill.GrandTotal = (int)Session["total"];
                    db.CashBills.Add(cashBill);
                    db.SaveChanges();

                    foreach (var chiTiet in ctcashBill)
                    {
                        chiTiet.BillID = cashBill.ID;
                        chiTiet.Product = null;
                        db.CashBillDetails.Add(chiTiet);
                    }

                    db.SaveChanges();
                    scope.Complete();

                    Session["CashBill"] = null;
                    Session["ctcashBill"] = null;
                    Session["total"] = null;
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
                try { 
                db.Entry(cashBill).State = EntityState.Modified;
                Session["CashBill"] = cashBill;
                db.SaveChanges();
                return RedirectToAction("Index");
                }
                catch(Exception e)
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

        private void checkCashBill(CashBill cashBill)
        {
            if (cashBill.CustomerName == null || cashBill.CustomerName.Equals(""))
                ModelState.AddModelError("CustomerName", "Tên khách hàng không được bỏ trống");
            if (cashBill.Address == null || cashBill.Address.Equals(""))
                ModelState.AddModelError("Address", "Địa chỉ không được bỏ trống");
            if (cashBill.PhoneNumber == null || cashBill.PhoneNumber.Equals(""))
                ModelState.AddModelError("PhoneNumber", "Số điện thoại không được bỏ trống");
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
            var order = db.CashBills.FirstOrDefault(o => o.ID == id);
            if (order != null)
            {
                PrintModel rp = new PrintModel();
                rp.Address = order.Address;
                rp.BillCode = order.BillCode;
                rp.CustomerName = order.CustomerName;
                rp.Date = order.Date;
                rp.GrandTotal = order.GrandTotal;
                rp.Note = order.Note;
                rp.PhoneNumber = order.PhoneNumber;
                rp.Shipper = order.Shipper;
                rp.CashBillDetail = order.CashBillDetails.ToList();
                return View(rp);
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