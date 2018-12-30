using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DIENMAYQUYETTIEN2.Models;
using System.Transactions;
using System.Net;
using System.Data.Entity;
using System.Data;
using System.Web.Security;
using System.Web.Configuration;

namespace DIENMAYQUYETTIEN2.Areas.Admin.Controllers
{
    public class ProductAdminController : Controller
    {
        
        DmQT10Entities db = new DmQT10Entities();
        //
        // GET: /Admin/ProductAdmin/
        public ActionResult Index()
        {
            var product = db.Products.OrderByDescending(x => x.ID).ToList();

            if (Session["Username"] != null)
            {
                return View(product);
            }
            else
            {
                return RedirectToAction("Login");
            }

        }
        //create
        [HttpGet]
        public ActionResult Create()
        {
            //ViewBag.ProductType = db.ProductTypes.OrderByDescending(x => x.ID).ToList();
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes.OrderByDescending(x => x.ID).ToList(), "ID", "ProductTypeName");
            if (Session["Username"] != null)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Login");
            }
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Product p)
        {
            CheckBangSanPham(p);
            if (ModelState.IsValid)
            {
                //db.Products.Add(p);
                //db.SaveChanges();
                ////using (var scope = new TransactionScope())
                ////{ 
                ////var pro = new Product();

                ////pro.ProductCode = p.ProductCode;
                ////pro.ProductName = p.ProductName;
                ////pro.ProductType = p.ProductType;
                ////pro.ProductTypeID = p.ProductTypeID;
                ////pro.OriginPrice = p.OriginPrice;
                ////pro.SalePrice = p.SalePrice;
                ////pro.Status = p.Status;
                ////pro.Quantity = p.Quantity;
                ////pro.InstallmentPrice = p.InstallmentPrice;
                ////pro.Avatar = p.Avatar;
                ////db.Products.Add(pro);
                ////db.SaveChanges();
                ////var path = Server.MapPath("~/App_Data");
                ////path = path + "/" + pro.ID;
                ////    if (Request.Files["Avatar"] != null && Request.Files["Avatar"].ContentLength > 0)
                ////    {
                ////        Request.Files["Avatar"].SaveAs(path);
                ////        scope.Complete();
                ////        return RedirectToAction("Index");
                ////    }
                ////    else
                ////    {
                ////        ModelState.AddModelError("Avatar", "Chưa có hình ảnh");
                ////    }
                ////}
                //return RedirectToAction("Index");




                using (var scope = new TransactionScope())
                {
                    db.Products.Add(p);
                    db.SaveChanges();

                    if (Request.Files["Avatar"] != null &&
                        Request.Files["Avatar"].ContentLength > 0)
                    {
                        var path = Server.MapPath("~/App_Data");
                        path = System.IO.Path.Combine(path, p.ID.ToString());
                        Request.Files["Avatar"].SaveAs(path);

                        scope.Complete();
                        return RedirectToAction("Index");
                    }
                    else
                        ModelState.AddModelError("Avatar", "Chua chon hinh anh cho san pham");
                }
            }
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ID","ProductTypeName", p.ProductTypeID);
            return View(p);
        }
        public FileResult Details(int id)
        {
            var path = Server.MapPath("~/App_Data/" + id);
            return File(path, "images");
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

        [HttpGet]
        public ActionResult Edit(int ID)
        {
                Product bangsanpham = db.Products.Find(ID);
                if (bangsanpham == null)
                {
                return HttpNotFound();
                }
            if (Session["Username"] != null)
            {
                //ViewBag.ProductType = db.ProductTypes.OrderByDescending(x => x.ID).ToList();
                ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ID", "ProductTypeName", bangsanpham.ProductTypeID);
                return View(bangsanpham);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Product model)
        {
            CheckBangSanPham(model);
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope())
                {
                    db.Entry(model).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    var path = Server.MapPath("~/App_Data");
                    path = path + "/" + model.ID;
                    if (Request.Files["Avatar"] != null && Request.Files["Avatar"].ContentLength > 0)
                    {
                        Request.Files["Avatar"].SaveAs(path);


                    }
                    scope.Complete();
                    return RedirectToAction("Index");

                }
            }
            //ViewBag.ProductType = db.ProductTypes.OrderByDescending(x => x.ID).ToList();
            ViewBag.ProductTypeID = new SelectList(db.ProductTypes, "ID", "ProductTypeName", model.ProductTypeID);
            return View(model);

        }

        private void CheckBangSanPham(Product model)
        {
            
            if (model.OriginPrice < 0)
            {
                ModelState.AddModelError("OriginPrice", "Giá gốc phải lớn hơn 0");
            }
            if (model.SalePrice < 0)
            {
                ModelState.AddModelError("SalePrice", "Giá bán phải lớn hơn 0");
            }
            if (model.InstallmentPrice < 0)
            {
                ModelState.AddModelError("InstallmentPrice", "Giá trả góp phải lớn hơn 0");
            }
            if (model.Quantity < 0)
            {
                ModelState.AddModelError("Quantity", "Số lượng tồn phải lớn hơn 0");
            }
           
                
        }

        [HttpGet]
        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product bangsanpham = db.Products.Find(id);
            if (bangsanpham == null)
            {
                return HttpNotFound();
            }
            if (Session["Username"] != null)
            {
                return View(bangsanpham);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product bangsanpham = db.Products.Find(id);
            db.Products.Remove(bangsanpham);
            db.SaveChanges();
            return RedirectToAction("Index");
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
