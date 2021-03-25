using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using ChetuProject.Models;
using System.Web.Security;
using System.IO;

namespace ChetuProject.Controllers
{

   [Authorize]
    public class UserController : Controller
    {       

        DatabaseContext db = new DatabaseContext();

        [AllowAnonymous]
        public ActionResult Index()
        {
            return View(db.userAccounts.ToList());
        }

        [AllowAnonymous]
        public ActionResult Delete(int id)
        {
            var d =db.userAccounts.SingleOrDefault(m => m.Id == id);
            db.userAccounts.Remove(d);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        [AllowAnonymous]
        public ActionResult DeleteAccount(int id)
        {
            var d = db.userAccounts.SingleOrDefault(m => m.Id == id);
            db.userAccounts.Remove(d);
            db.SaveChanges();
            return RedirectToAction("Register");
        }

        [AllowAnonymous]
        public ActionResult Update(int id)
        {
            if (db.userAccounts.Any(x => x.Id == id))
            {
                var e = db.userAccounts.SingleOrDefault(n => n.Id == id);
                return View(new UserAccount
                { 
                UserName=e.UserName,
                Email=e.Email,
                gender=e.gender,
                MobileNo=e.MobileNo,
                Status=e.Status
                });
            }
            else
            {
                return RedirectToAction("index");
            }
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Update(UserAccount obj)
        {
            var dbUser = db.userAccounts.Find(obj.Id);
            dbUser.UserName = obj.UserName;
            dbUser.Email = obj.Email;
            dbUser.gender = obj.gender;
            dbUser.MobileNo = obj.MobileNo;
            dbUser.Status = obj.Status;

            db.Entry(dbUser).State = System.Data.Entity.EntityState.Modified;
            db.SaveChanges();
            ViewBag.Message = obj.UserName + "  SuccessFully Updated Record.";
            return RedirectToAction("Login");
        }

        [AllowAnonymous]
        public ActionResult Register()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Register(UserAccount obj, HttpPostedFileBase ImageFile)
        {
            if (ModelState.IsValid)
            {
                string filename = Path.GetFileNameWithoutExtension(ImageFile.FileName);
                string extension = Path.GetExtension(ImageFile.FileName);
                filename = DateTime.Now.ToString("yymmssfff") + extension;
                obj.ProfileImage = "~/PICS/" + filename;
                filename = Path.Combine(Server.MapPath("~/PICS/"), filename);
                ImageFile.SaveAs(filename);


                db.userAccounts.Add(obj);
                db.SaveChanges();

            }
            ModelState.Clear();
            ViewBag.Message = obj.UserName + "  SuccessFully Registred.";
            return View();
        }


        [AllowAnonymous]
        public ActionResult Login()
        {
            return View();

        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult Login(UserAccount obj)
        {
            var usr = db.userAccounts.Single(u => u.UserName == obj.UserName && u.Password == obj.Password);
            if (usr!= null)
            {
                Session["UserID"] = usr.Id.ToString();
                Session["UserName"] = usr.UserName.ToString();
               //Session["Img"]= usr.ProfileImage.ToString();
                

                FormsAuthentication.SetAuthCookie(usr.Email,false);
                return RedirectToAction("LoggedIn");
            }
            else
            {
                ModelState.AddModelError("", "UserName Password is wrong");
            }
            return View();
        }

        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login");
            
        }

        public ActionResult LoggedIn()
        {         
            if (User.Identity.IsAuthenticated)
            {
                var user = db.userAccounts.SingleOrDefault(e => e.Email == User.Identity.Name);
                return View(user);
            }
            else
            {
                return RedirectToAction("Login");
            }          

        }
        
    }
    }