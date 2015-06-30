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
        //
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
              LoginResult lr = new LoginResult();
              try
              {

                  //  var userNameEnc = CommonHelper.GetEncryptedData(UserName);

                  var passEnc = CommonHelper.GetEncryptedData(Password);

                  using (NOOCHEntities obj = new NOOCHEntities())
                  {
                      var query = (from c in obj.AdminUsers
                                   where c.Password == passEnc && c.UserName == UserName &&
                                c.Status == "Active"
                                   select c).SingleOrDefault();

                      if (query != null)
                      {
                          lr.IsSuccess = true;
                          lr.Message = "Success";
                          Session["UserId"] = query.UserId;
                          Session["RoleId"] = query.AdminLevel;
                      }
                      else
                      {
                          lr.IsSuccess = false;
                          lr.Message = "Invalid username or password";
                      }
                      return Json(lr);
                  }


              }
              catch (Exception ex)
              {
                  Logger.Info("test message");
                  Logger.Error(ex);
                  lr.IsSuccess = false;
                  lr.Message = "Invalid username or password";
                  return Json(lr);
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
