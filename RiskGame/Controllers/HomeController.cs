using RiskGame.DAL;
using RiskGame.Entity;
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
           /* var data = new DataContext();
            var repo = new UserRepository(data);
            var test = repo.GetAll();
            User user = new User();*/
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