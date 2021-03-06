﻿using RiskGame.Core.WorkProcess;
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
    public class GameStartController : BaseGameController
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        private readonly WorkProcessService _workProcessService = new WorkProcessService();
        public ActionResult Index(int id)
        {
            if (CommonFunction.CheckCurrentGame() == false) return RedirectToAction("Index", "Home");

            var softwareType = Singleton.Game().SoftwareType;
            var listRisk = _workProcessService.GenerateRiskChioceModel(softwareType, Singleton.Game().GameRoomId, Singleton.Game().Turn);
            var list = new List<RiskData>();
            foreach (var item in listRisk.Where(x => x.RiskOptions.Any()))
            {
                var risk = new RiskData
                {
                    RiskId = item.RiskId,
                    Name = item.RiskName,
                    RiskType = Enum.GetName(typeof(Const.RiskType), (int)item.RiskType),
                    RiskTypeValue = item.RiskType,
                    RiskDetail = item.RiskDetail,
                    RiskOption = item.RiskOptions.ToList(),
                    RiskImpact = item.RiskImpact,
                    RiskProbability = item.RiskProbability
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


        public JsonResult MultiPlayerGameStart(int id)
        {
            //update start - end
            var gameRoom = _service.GameRoom().GetRoomById(id);
            gameRoom.StartDate = DateTime.UtcNow;
            gameRoom.EndDate = DateTime.UtcNow.AddMinutes(15);
            _service.GameRoom().UpdateGameRoom(gameRoom);

            var response = CommonFunction.GetResponse(gameRoom != null, string.Empty, gameRoom);
            return Json(response);
        }

        [HttpGet]
        public JsonResult GetGameRoomStatus(int id)
        {
            var gameRoom = _service.GameRoom().GetGameRoomByIdSQL(id);
            var response = CommonFunction.GetResponse(gameRoom != null, string.Empty, gameRoom);
            return Json(response,JsonRequestBehavior.AllowGet);
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