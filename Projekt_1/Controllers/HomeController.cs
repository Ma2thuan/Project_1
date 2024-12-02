using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Projekt_1.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult login()
        {
            return View("login");
        }

        public ActionResult chart()
        {
            return View("charts");
        }

        public ActionResult light_nav()
        {
            return View("layout-sidenav-light");

        }

        public ActionResult static_nav()
        {
            return View("layout-static");
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}