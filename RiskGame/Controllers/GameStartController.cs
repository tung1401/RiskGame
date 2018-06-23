using RiskGame.Entity;
using RiskGame.Helper;
using RiskGame.Models;
using RiskGame.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace RiskGame.Controllers
{
    public class GameStartController : Controller
    {

        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        public ActionResult Index(int id)
        {
            var risks = _service.Risk().GetAllRisk();
            var list = new List<RiskData>();
            foreach (var item in risks.Where(x => x.RiskOptions.Any()))
            {
                var risk = new RiskData
                {
                    RiskId = item.RiskId,
                    Name = item.RiskName,
                    RiskType = item.RiskType.ToString(),
                    RiskOption = item.RiskOptions.ToList()
                };
                list.Add(risk);
            }
            var model = new RiskDataModel
            {
                RiskData = list
            };
            ViewBag.Money = Singleton.User().Money;
            return View(model);
        }
      
        public ActionResult Result()
        {

            return View();
        }

        public void InitialGame()
        {
            FormsAuthentication.SetAuthCookie("patompoltsj@gmail.com", false);
            var adminData = new UserGameModel
            {
                Team = 3,
                Project = 0,
                Money = 10000,
                GameSession = "01012017",
                UserId = 1
            };
            var serializer = new JavaScriptSerializer();
            var cookie = new HttpCookie("UserGame", serializer.Serialize(adminData))
            {
                Expires = DateTime.Now.AddHours(1)
            };
            Response.Cookies.Add(cookie);
        }


        public void UpdateGameUser(int money)
        {
            //FormsAuthentication.SetAuthCookie("patompoltsj@gmail.com", false);

            HttpCookie cookie = Request.Cookies["UserGame"];
            var adminData = new UserGameModel
            {
                Team = 3,
                Project = 0,
                Money = money,
                GameSession = "01012017"
            };

            var serializer = new JavaScriptSerializer();
            cookie = new HttpCookie("UserGame", serializer.Serialize(adminData));
            Response.SetCookie(cookie); //SetCookie() is used for update the cookie.
            Response.Cookies.Add(cookie);

            //var adminData = new UserGameModel
            //{
            //    Team = 3,
            //    Project = 0,
            //    Money = money,
            //    GameSession = "01012017"
            //};
            //var serializer = new JavaScriptSerializer();
            //var cookie = new HttpCookie("UserGame", serializer.Serialize(adminData))
            //{
            //};
            //Response.Cookies.Add(cookie);
        }


        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            Session["UserGame"] = null;
            Response.Cookies["UserGame"].Expires = DateTime.Now.AddDays(-1);
            return RedirectToAction("Index", "Home");
        }
    }
}