using RiskGame.Helper;
using RiskGame.Models;
using RiskGame.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RiskGame.Controllers
{
    public class MarketController : Controller
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        public ActionResult Index()
        {
            var data = _service.GameRoom().GetGameHistory(Singleton.User().UserId);
            var model = new GameHistoryViewModel
            {
                GameRoomList = data.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public JsonResult HireExpertSuggestion(int expertLevel)
        {
            var moneyTotal = Singleton.Game().Money;
            var turn = Singleton.Game().Turn;
            var gameBattle = _service.Game().GetGameBattleOpenRisk(Singleton.Game().GameRoomId, turn);

            var expertSuggestionModel = new ExpertSuggestionModel{
                SuggestLevel = expertLevel,

            };
            var listData = new List<ExpertSuggestion>();
            if (expertLevel == 1)
            {
                listData.Add(new ExpertSuggestion {
                    Level = gameBattle.FirstOrDefault().RiskOption.RiskLevel.GetValueOrDefault(),
                    Ratio = gameBattle.FirstOrDefault().Ratio.GetValueOrDefault(),
                    RiskType = string.Empty,
                    TotalActualPayValue = string.Empty
                });
                expertSuggestionModel.SuggestCost = 5000.ToString("n0");
                moneyTotal -= 5000;
            }
            else if(expertLevel == 2)
            {
                listData.Add(new ExpertSuggestion
                {
                    Level = gameBattle.FirstOrDefault().RiskOption.RiskLevel.GetValueOrDefault(),
                    Ratio = gameBattle.FirstOrDefault().Ratio.GetValueOrDefault(),
                    RiskType = Enum.GetName(typeof(Const.RiskType), (int)gameBattle.FirstOrDefault().Risk.RiskType),
                    TotalActualPayValue = gameBattle.Sum(x => x.Ratio * x.ActionEffectValue).GetValueOrDefault().ToString("n0")
                });
                expertSuggestionModel.SuggestCost = 10000.ToString("n0");
                moneyTotal -= 10000;
            }
            else if(expertLevel == 3)
            {
                foreach (var item in gameBattle)
                {
                    listData.Add(new ExpertSuggestion
                    {
                        Level = item.RiskOption.RiskLevel.GetValueOrDefault(),
                        Ratio = item.Ratio.GetValueOrDefault(),
                        RiskType = Enum.GetName(typeof(Const.RiskType), (int)item.Risk.RiskType),
                        TotalActualPayValue = gameBattle.Sum(x => x.Ratio * x.ActionEffectValue).GetValueOrDefault().ToString("n0")
                    });
                }
                expertSuggestionModel.SuggestCost = 30000.ToString("n0");
                moneyTotal -= 30000;
            }

            _service.GameRoom().UpdateUserGameRoom(Singleton.Game().UserId, Singleton.Game().GameRoomId, moneyTotal);
            Singleton.UpdateGameSession(Singleton.Game().Team, Singleton.Game().Project, moneyTotal, turn, Singleton.Game().SoftwareType, Singleton.Game().PlayerImageUrl);

            expertSuggestionModel.MoneyTotalFormat = moneyTotal.ToString("n0");
            expertSuggestionModel.ExpertSuggestion = listData;

            var response = CommonFunction.GetResponse(expertSuggestionModel.ExpertSuggestion.Count > 0, null, expertSuggestionModel);
            return Json(response, JsonRequestBehavior.AllowGet);
        }



        [HttpPost]
        public JsonResult InterveneLookUser(int interveneLevel)
        {
            var moneyTotal = Singleton.Game().Money;
            var turn = Singleton.Game().Turn;
            var alluserGameRoom = _service.GameRoom().GetUserGameRoom(Singleton.Game().GameRoomId)
                .Where(x=> x.UserId != Singleton.Game().UserId);

            if (!alluserGameRoom.Any())
            {
                var responseNoEnemy = CommonFunction.GetResponse(true, "ไม่มีคู่แข่ง", null);
                return Json(responseNoEnemy, JsonRequestBehavior.AllowGet);
            }


            var interveneModel = new InterveneModel
            {
                InterveneLevel = interveneLevel,

            };
            var listData = new List<Intervene>();
            if (interveneLevel == 1)
            {
                foreach (var item in alluserGameRoom)
                {
                    listData.Add(new Intervene
                    {
                        Player =  item.PlayerName,
                        CurrentProject = item.ProjectValue,
                        GameStatus = item.GameFinished.GetValueOrDefault() ? "End Game": "Playing"
                    });
                }
                interveneModel.InterveneCost = 1000.ToString("n0");
                moneyTotal -= 1000;
            }
            else if (interveneLevel == 2)
            {
                foreach (var item in alluserGameRoom)
                {
                    listData.Add(new Intervene
                    {
                        Player = item.PlayerName,
                        CurrentTeam = item.TeamValue,
                        CurrentMoney = item.MoneyValue.ToString("n0"),
                        GameStatus = item.GameFinished.GetValueOrDefault() ? "End Game" : "Playing"
                    });
                }
                interveneModel.InterveneCost = 5000.ToString("n0");
                moneyTotal -= 5000;
            }
            else if (interveneLevel == 3)
            {
                foreach (var item in alluserGameRoom)
                {
                    listData.Add(new Intervene
                    {
                        Player = item.PlayerName,
                        CurrentMoney = item.MoneyValue.ToString("n0"),
                        CurrentTeam = item.TeamValue,
                        CurrentProject = item.ProjectValue,
                        GameStatus = item.GameFinished.GetValueOrDefault() ? "End Game" : "Playing"
                    });
                }
                interveneModel.InterveneCost = 10000.ToString("n0");
                moneyTotal -= 10000;
            }

            _service.GameRoom().UpdateUserGameRoom(Singleton.Game().UserId, Singleton.Game().GameRoomId, moneyTotal);
            Singleton.UpdateGameSession(Singleton.Game().Team, Singleton.Game().Project, moneyTotal, turn, Singleton.Game().SoftwareType, Singleton.Game().PlayerImageUrl);

            interveneModel.MoneyTotalFormat = moneyTotal.ToString("n0");
            interveneModel.InterveneList = listData;

            var response = CommonFunction.GetResponse(interveneModel.InterveneList.Count > 0, null, interveneModel);
            return Json(response, JsonRequestBehavior.AllowGet);
        }

    }


    public class InterveneModel
    {
        public string InterveneCost { set; get; }
        public int InterveneLevel { set; get; }
        public string MoneyTotalFormat { set; get; }
        public List<Intervene> InterveneList { set; get; }
    }



    public class Intervene
    {
        public string Player { get; set; }
        public int CurrentTurn { get; set; }
        public int CurrentProject { get; set; }
        public int CurrentTeam { get; set; }
        public string CurrentMoney { get; set; }
        public string GameStatus { get; set; }
    }


    public class ExpertSuggestionModel
    {
        public string SuggestCost { set; get; }
        public int SuggestLevel { set; get; }
        public string MoneyTotalFormat { set; get; }
        public List<ExpertSuggestion> ExpertSuggestion { set; get; }
    }



    public class ExpertSuggestion
    {
        public int Level { get; set; }
        public int Ratio { get; set; }
        public string RiskType { get; set; }
        public string TotalActualPayValue { get; set; }
    }

}