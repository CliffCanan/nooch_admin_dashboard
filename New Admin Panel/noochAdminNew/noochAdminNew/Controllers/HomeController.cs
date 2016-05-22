using noochAdminNew.Classes.ResponseClasses;
using noochAdminNew.Classes.Utility;
using noochAdminNew.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace noochAdminNew.Controllers
{
    public class HomeController : Controller
    {
        // GET: /Home/
        [HttpGet, OutputCache(NoStore = true, Duration = 1)]
        public ActionResult Index()
        {
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetExpires(DateTime.UtcNow.AddHours(-1));
            Response.Cache.SetNoStore();
            return View();
        }

        [HttpPost]
        [ActionName("ValidateUser")]
        public ActionResult ValidateUser(string UserName, string Password)
        {
            Logger.Info("Home Cntrlr -> ValidateUser Fired - Username: [" + UserName + "]");

            LoginResult res = new LoginResult();
            res.IsSuccess = false;

            try
            {
                var passEnc = CommonHelper.GetEncryptedData(Password);

                using (NOOCHEntities obj = new NOOCHEntities())
                {
                    var query = (from c in obj.AdminUsers
                                 where c.UserName == UserName && c.Status == "Active" && c.Password == passEnc
                                 select c).SingleOrDefault();

                    if (query != null)
                    {
                        res.IsSuccess = true;
                        res.Message = "Success";
                        Session["UserId"] = query.UserId;
                        Session["RoleId"] = query.AdminLevel;
                    }
                    else
                    {
                        res.Message = "Invalid username or password";
                    }
                    return Json(res);
                }
            }
            catch (Exception ex)
            {
                Logger.Error("Home Cntrlr -> ValidateUser FAILED - Exception: [" + ex.Message + "]");
                res.Message = ex.Message;
                return Json(res);
            }
        }


        public ActionResult Logout()
        {
            Session["UserId"] = null;
            Session.Abandon();
            Session.Clear();

            return RedirectToAction("Index", "Home");
        }
    }
}