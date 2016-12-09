using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

using RedisDataLayer.DataManipulate;
using System.Net.Http.Headers;

namespace Tralala.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Title = "Home Page";

            var cookie =  Request.Cookies.Get("auth");

            if (cookie != null)
            {
                UserManipulate um = new UserManipulate();
                if (um.IsLogedIn(cookie.Value))
                    return RedirectToAction("UserHome", "Home");
            }
            return View();
        }
        public ActionResult Registration()
        {
            ViewBag.Title = "Registration";

            var cookie = Request.Cookies.Get("auth");

            if (cookie != null)
            {
                UserManipulate um = new UserManipulate();
                if (um.IsLogedIn(cookie.Value))
                {
                    return RedirectToAction("UserHome", "Home");
                    //ViewBag.Title = "UserHome";                   
                    //return View("UserHome");
                }
            }
            return View();
        }
        public ActionResult UserHome()
        {
            ViewBag.Title = "UserHome";
            var cookie = Request.Cookies.Get("auth");

            if (cookie != null)
            {
                UserManipulate um = new UserManipulate();
                if (!um.IsLogedIn(cookie.Value))
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            return View();
        }
    }
}
