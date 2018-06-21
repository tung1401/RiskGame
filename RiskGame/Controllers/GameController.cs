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
    public class GameController : Controller
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        public ActionResult Index()
        {
            // get risk from db battle progress 

            var requirementList = _service.Risk().GetRiskByType(1).ToList();
            var list = new List<RiskData>();
            foreach (var req in requirementList)
            {
                var risk = new RiskData
                {
                    RiskId = req.RiskId,
                    Name = req.RiskName,
                    RiskType = req.RiskType.ToString(),
                    RiskOption = req.RiskOptions.ToList()
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
        [HttpPost]
        public ActionResult OpenRisk(FormCollection form)
        {
            var selectedRisk = form.AllKeys.Where(x => x.Contains("riskoption")).ToList();
            var moneySummary = 0;
            if (selectedRisk.Any())
            {
                foreach (var item in selectedRisk)
                {
                    var moneyValue = form[item];
                    if (moneyValue != null)
                    {
                        moneySummary += int.Parse(moneyValue);
                    }
                    //save database
                }
            }

            var total = Singleton.User().Money - moneySummary;
            UpdateGameUser(total);
            ViewBag.Money = total;
            return View();
        }

        public ActionResult OpenRisk()
        {
            // Random Risk
            var model = new RiskDataModel();
            var openriks = _service.Game().GetGameBattleOpenRisk(Singleton.User().GameRoomId, Singleton.User().Turn);
            if (openriks.Any())
            {
               // model.RiskData = openriks;
            }




            // get risk selected from db


            return View();
        }
        public ActionResult Result()
        {

            return View();
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