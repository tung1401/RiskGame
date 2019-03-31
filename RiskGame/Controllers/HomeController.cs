using RiskGame.DAL;
using RiskGame.Entity;
using RiskGame.Helper;
using RiskGame.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RiskGame.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
          /*  var x = "";
            for (int i = 0; i < 20; i++)
            {
                x += CommonFunction.RandomNumber(1, 3);
            }*/
            return View();
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