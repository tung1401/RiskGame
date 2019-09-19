using RiskGame.Core.WorkProcess;
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
using static RiskGame.Helper.Const;

namespace RiskGame.Controllers
{
    public class GameController : BaseGameController
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        private readonly WorkProcessService _workProcessService = new WorkProcessService();
        public ActionResult Index()
        {
            if (CommonFunction.CheckCurrentGame() == false) return RedirectToAction("Index", "Home");
            // get risk from db battle progress 

            var softwareType = Singleton.Game().SoftwareType;
            var listRisk = _workProcessService.GenerateRiskChioceModel(softwareType, Singleton.Game().GameRoomId, Singleton.Game().Turn);
            var list = new List<RiskData>();
            foreach (var item in listRisk)
            {
                var risk = new RiskData
                {
                    RiskId = item.RiskId,
                    Name = item.RiskName,
                    RiskType = Enum.GetName(typeof(Const.RiskType), (int)item.RiskType),
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
        //[HttpPost]
        //public ActionResult OpenRisk(FormCollection form)
        //{
        //    var selectedRisk = form.AllKeys.Where(x => x.Contains("riskoption")).ToList();
        //    var moneySummary = 0;
        //    if (selectedRisk.Any())
        //    {
        //        foreach (var item in selectedRisk)
        //        {
        //            var moneyValue = form[item];
        //            if (moneyValue != null)
        //            {
        //                moneySummary += int.Parse(moneyValue);
        //            }
        //            //save database
        //        }
        //    }

        //    var total = Singleton.User().Money - moneySummary;
        //    UpdateGameUser(total);
        //    ViewBag.Money = total;
        //    return View();
        //}

        public ActionResult OpenRisk()
        {
            // check risk, and reduce money
            var moneyTotal = Singleton.Game().Money;
            var model = new GameBattleViewModel();
            var openRisk = _service.Game().GetGameBattleOpenRisk(Singleton.Game().GameRoomId, Singleton.Game().Turn);
            if (openRisk.Any())
            {
                model.GameBattles = openRisk.ToList();            
                var gameBattleData = _service.Game().GetGameBattleData(openRisk.ToList());
                var javaScriptSearilizer = new JavaScriptSerializer();
                var searializedObject = javaScriptSearilizer.Serialize(gameBattleData);
                model.OpenRiskGameBattleModelArray = searializedObject;


                var userGameRisk = _service.Game().GetUserGameRisk(Singleton.Game().GameRoomId,
                    Singleton.Game().Turn, Singleton.Game().UserId);

                model.UserGameRisk = userGameRisk.ToList();
                foreach (var item in model.GameBattles)
                {
                    var userGameBattleData = new UserGameBattleData
                    {
                        GameBattle = item,
                        ProtectStatus = ProtecStatus.Lose.ToString()                 
                    };

                    var effectItemMoney = item.Ratio.GetValueOrDefault() * item.ActionEffectValue.GetValueOrDefault();
                    var riskProtect = userGameRisk.FirstOrDefault(x => x.RiskId == item.RiskId);
                    var effectMoney = 0;
                    if (riskProtect != null && riskProtect.RiskOption.RiskLevel != (int)RiskGameLevel.ZeroLevel)
                    {
                        if(riskProtect.RiskOption.RiskLevel != item.RiskOption.RiskLevel)
                        {
                            if (riskProtect.RiskOption.RiskLevel > item.RiskOption.RiskLevel)
                            {
                                // ไม่ต้องจ่าย ป้องกันได้ 100%
                                //moneyTotal = Singleton.Game().Money;
                                userGameBattleData.ProtectStatus = ProtecStatus.Win.ToString();
                            }
                            else
                            {
                                if (riskProtect.RiskOption.RiskLevel == (int)RiskGameLevel.ThirdLevel)
                                {
                                    //ป้องกัน 100%
                                    // moneyTotal = Singleton.Game().Money;
                                    userGameBattleData.ProtectStatus = ProtecStatus.Win.ToString();
                                }
                                else if (riskProtect.RiskOption.RiskLevel == (int)RiskGameLevel.SecondLevel)
                                {
                                    //ป้องกัน 50% จ่าย 50%
                                    effectMoney = (int)(effectItemMoney * 0.5);
                                    moneyTotal = moneyTotal - (int)(effectItemMoney * 0.5);
                                }
                                else if (riskProtect.RiskOption.RiskLevel == (int)RiskGameLevel.FirstLevel)
                                {
                                    //ป้องกัน 25% จ่าย 75%
                                    effectMoney = (int)(effectItemMoney * 0.75);
                                    moneyTotal = moneyTotal - (int)(effectItemMoney * 0.75);
                                }
                                else if(riskProtect.RiskOption.RiskLevel == (int)RiskGameLevel.ZeroLevel)
                                {
                                    effectMoney = (int)(effectItemMoney * 1);
                                    moneyTotal = moneyTotal - (int)(effectItemMoney * 1);
                                }
                            }  
                        }
                        else
                        {
                            // ถ้าเลือกแล้ว Level เท่ากัน ป้องกันได้ 100%
                           // moneyTotal = Singleton.Game().Money;
                            userGameBattleData.ProtectStatus = ProtecStatus.Draw.ToString();
                        }   
                    }
                    else
                    {
                        // ถ้าไม่ได้เลือก หรือ ไม่ได้ป้องกัน จ่าย 100%
                        effectMoney = effectItemMoney;
                        moneyTotal = moneyTotal - effectItemMoney;
                    }

                    // ถ้าแพ้ และ มีข่าว จะโดนผลกระทบเพิ่ม
                    if (item.RiskNewsId != null && userGameBattleData.ProtectStatus == ProtecStatus.Lose.ToString())
                    {
                        // fact impact
                        var riskNews = _service.Risk().GetRiskNewsById(item.RiskNewsId.GetValueOrDefault());
                        if (riskNews != null)
                        {
                            var riskNewsImpactPercent = CommonFunction.RiskImpactFormat(riskNews.RiskNewsImpact.GetValueOrDefault());
                            var riskNewsImpact = (int)(effectItemMoney * riskNewsImpactPercent);

                            moneyTotal = moneyTotal - riskNewsImpact;
                            effectMoney = effectMoney + riskNewsImpact;

                            userGameBattleData.RiskNewsImpactPercent = riskNewsImpactPercent;
                            userGameBattleData.RiskNewsImpact = riskNewsImpact; // ค่าเงิน
                        }
                    }

                    userGameBattleData.EffectMoney = effectMoney;
                    model.UserGameBattleData.Add(userGameBattleData);
                }
            }

            // get risk selected from db
            model.GameDone = _service.Game().CheckMaxTurn(Singleton.Game().GameRoomId, Singleton.Game().Turn);
            var nextTurn = Singleton.Game().Turn;
            if (!model.GameDone)
            {
                nextTurn += 1;
            }

            _service.GameRoom().UpdateUserGameRoom(Singleton.Game().UserId, Singleton.Game().GameRoomId, moneyTotal, nextTurn);
            Singleton.UpdateGameSession(Singleton.Game().Team, Singleton.Game().Project, moneyTotal, nextTurn, Singleton.Game().SoftwareType, Singleton.Game().PlayerImageUrl);

            return View(model);
        }

        [HttpPost]
        public ActionResult ProtectRisk(FormCollection form)
        {
            var selectedRisk = form.AllKeys.Where(x => x.Contains("RiskId")).ToList();
            var moneySummary = 0;
            if (selectedRisk.Any())
            {
                foreach (var item in selectedRisk)
                {
                    var optionId = int.Parse(form[item]);
                    var riskOption = _service.Risk().GetRiskOptionById(optionId, 1);

                    if (riskOption != null)
                    {
                        moneySummary += riskOption.ActionEffectValue.GetValueOrDefault();

                        _service.Game().AddUserGameRisk(new UserGameRisk
                        {
                            UserId = Singleton.User().UserId,
                            Turn = Singleton.Game().Turn,
                            GameRoomId = Singleton.Game().GameRoomId,
                            RiskOptionId = optionId,
                            RiskId = riskOption.RiskId.GetValueOrDefault(),
                            CreateDate = DateTime.UtcNow,
                            CreateBy = Singleton.User().UserId
                        });
                    }
                    //save database          
                }
            }

            var gameRoom = _service.GameRoom().GetRoomById(Singleton.Game().GameRoomId);
            if (gameRoom.IncludeBot.GetValueOrDefault() == true) {

                var botTurn = Singleton.Game().Turn;
                var botGameRoomId = gameRoom.GameRoomId;
                _service.BotExpert().BotProtectRiskAsync(botGameRoomId, botTurn, (int)JobType.ExpertSpecialist);
                _service.BotExpert().BotProtectRiskAsync(botGameRoomId, botTurn, (int)JobType.Newbies);
            }
            //if (gameRoom != null)
            //{
            //    gameRoom.MoneyValue = Singleton.Game().Money - moneySummary;
            //}
            var money = Singleton.Game().Money - moneySummary;
            _service.GameRoom().UpdateUserGameRoom(Singleton.Game().UserId, Singleton.Game().GameRoomId, money, Singleton.Game().Turn);
            Singleton.UpdateGameSession(Singleton.Game().Team, Singleton.Game().Project, money, Singleton.Game().Turn, Singleton.Game().SoftwareType, Singleton.Game().PlayerImageUrl);
            return RedirectToAction("OpenRisk", "Game");
        }

        public ActionResult Result()
        {

            return View();
        }
        //public void UpdateGameUser(int money)
        //{
        //    //FormsAuthentication.SetAuthCookie("patompoltsj@gmail.com", false);

        //    HttpCookie cookie = Request.Cookies["UserGame"];
        //    var adminData = new UserGameModel
        //    {
        //        Team = 3,
        //        Project = 0,
        //        Money = money,
        //        GameSession = "01012017"
        //    };

        //    var serializer = new JavaScriptSerializer();
        //    cookie = new HttpCookie("UserGame", serializer.Serialize(adminData));
        //    Response.SetCookie(cookie); //SetCookie() is used for update the cookie.
        //    Response.Cookies.Add(cookie);

        //    //var adminData = new UserGameModel
        //    //{
        //    //    Team = 3,
        //    //    Project = 0,
        //    //    Money = money,
        //    //    GameSession = "01012017"
        //    //};
        //    //var serializer = new JavaScriptSerializer();
        //    //var cookie = new HttpCookie("UserGame", serializer.Serialize(adminData))
        //    //{
        //    //};
        //    //Response.Cookies.Add(cookie);
        //}
        //public ActionResult LogOff()
        //{
        //    FormsAuthentication.SignOut();
        //    Session["UserGame"] = null;
        //    Response.Cookies["UserGame"].Expires = DateTime.Now.AddDays(-1);
        //    return RedirectToAction("Index", "Home");
        //}


    }
}