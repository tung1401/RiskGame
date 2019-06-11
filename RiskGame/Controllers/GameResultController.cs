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
    public class GameResultController : BaseGameController
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        public ActionResult Index()
        {
            if (CommonFunction.CheckCurrentGame() == false) return RedirectToAction("Index", "Home");

            var gameRoom = _service.GameRoom().GetRoomById(Singleton.Game().GameRoomId);

            //Display all result
            var model = new GameResultViewModel
            {
                MyPlayer = new PlayerData
                {
                    GameBattleId = Singleton.Game().GameBattleId,
                    GameRoomId = Singleton.Game().GameRoomId,
                    PlayerName = Singleton.Game().PlayerName,
                    Money = Singleton.Game().Money.ToString(),
                    Team = Singleton.Game().Team.ToString(),
                    Project = Singleton.Game().Project.ToString(),
                    Goal = Enum.GetName(typeof(Const.GoalType), gameRoom.Goal),
                    GoalStatus = "",
                    Rank = "0",
                    GameStatus = "0",
                    
                },
                GameRoom = new GameRoomModel
                {
                    GameRoomId = gameRoom.GameRoomId
                }

            };

            var friendList = new List<PlayerData>();
            var userGameRoom =_service.GameRoom().GetCurrentUserGame(Singleton.Game().GameRoomId);
            if (userGameRoom.Count() > 1)
            {
                foreach (var item in userGameRoom.Where(x=>x.UserId != Singleton.Game().UserId))
                {
                    friendList.Add(new PlayerData
                    {
                        GameBattleId = Singleton.Game().GameBattleId, // same my player
                        GameRoomId = item.GameRoomId,
                        PlayerName = item.PlayerName,
                        Money = item.MoneyValue.ToString(),
                        Team = item.TeamValue.ToString(),
                        Project = item.ProjectValue.ToString(),
                        Rank = "0",
                        GameStatus = "0"
                    });
                }
            }

            //set status done
            var done = _service.GameRoom().UpdateGameRoomDone(Singleton.Game().UserId, Singleton.Game().GameRoomId);
            if (done)
            {
                //clear session
                Singleton.ClearGameSession();
            }

            return View(model);
        }
        public ActionResult ReDashBoard()
        {
            // go to new game
            return RedirectToAction("Index", "Home");
        }


        public ActionResult GetPlayerResult(int id)
        {
            var userGameRoom = _service.GameRoom().GetAllUserGameRoom(id);
            var model = new GameRoomModel
            {
                GameRoomId = id,
                UserGameRooms = userGameRoom.OrderByDescending(x => x.MoneyValue).ToList()
            };
            return PartialView("_PlayerResult", model);
        }

    }
}