using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DIENMAYQUYETTIEN2.Models;
using System.Web.Security;
using System.Net;
using System.Data.Entity;

namespace DIENMAYQUYETTIEN2.Areas.Admin.Controllers
{
    public class ProductTypeAdminController : Controller
    {
        DmQT10Entities db = new DmQT10Entities();
        // GET: Admin/ProductTypeAdmin
        public ActionResult Index()
        {
            if (Session["UserName"] != null)
            {
                return View(db.ProductTypes.ToList());
            }
            else
            {
                return RedirectToAction("Login");
            }
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


        // GET: Admin/ProductTypes/Create
        public ActionResult Create()
        {
            if (Session["UserName"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // POST: Admin/ProductTypes/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProductType productType)
        {
            checkProductType(productType);
            if (ModelState.IsValid)
            {
                db.ProductTypes.Add(productType);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(productType);
        }

        public void checkProductType(ProductType pt)
        {
            if (pt.ProductTypeCode.Length > 3)
                ModelState.AddModelError("ProductTypeCode", "ProductTypeCode phải nhỏ hơn 4 kí tự!");
            if (pt.ProductTypeName.Length > 100)
                ModelState.AddModelError("ProductTypeName", "ProductTypeName phải nhỏ hơn 100!");
        }

        // GET: Admin/ProductTypes/Edit/5
        public ActionResult Edit(int? id)
        {
            if (Session["UserName"] != null)
            {
                if (id == null)
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
                }
                ProductType productType = db.ProductTypes.Find(id);
                if (productType == null)
                {
                    return HttpNotFound();
                }
                return View(productType);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        // POST: Admin/ProductTypes/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ProductType productType)
        {
            checkProductType(productType);
            if (ModelState.IsValid)
            {
                db.Entry(productType).State = EntityState.Modified;
                db.SaveChanges();
                TempData["message"] = "Chỉnh sửa loại sản phẩm thành công.";
                return RedirectToAction("Index");
            }
            return View(productType);
        }

        // GET: Admin/ProductTypes/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProductType productType = db.ProductTypes.Find(id);
            if (productType == null)
            {
                return HttpNotFound();
            }
            return View(productType);
        }

        // POST: Admin/ProductTypes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            if (db.Products.Where(c => c.ProductTypeID == id).ToList().Count > 0)
            {
                TempData["message"] = "Không thể xóa loại sản phẩm có chứa sản phẩm.";
                return RedirectToAction("Index");
            }
            else
            {
                ProductType productType = db.ProductTypes.Find(id);
                db.ProductTypes.Remove(productType);
                db.SaveChanges();
                TempData["message"] = "Xóa sản phẩm thành công.";
                return RedirectToAction("Index");
            }

        }

    }
}