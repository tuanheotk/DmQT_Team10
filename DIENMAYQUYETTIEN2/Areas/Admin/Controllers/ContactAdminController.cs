using DIENMAYQUYETTIEN2.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

namespace DIENMAYQUYETTIEN2.Areas.Admin.Controllers
{
    public class ContactAdminController : Controller
    {
        DmQT10Entities db = new DmQT10Entities();
        // GET: Admin/ContactAdmin
        public ActionResult Index()
        {
            var messages = db.Messages.OrderByDescending(x => x.Email).ToList();
            if (Session["Username"] != null)
            {
                return View(messages);
            }
            else
            {
                return RedirectToAction("Login");
            }
        }


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

        public ActionResult Delete(int id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Message bangsanpham = db.Messages.Find(id);
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
            Message bangsanpham = db.Messages.Find(id);
            db.Messages.Remove(bangsanpham);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


    }
}