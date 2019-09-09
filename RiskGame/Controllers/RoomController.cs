using RiskGame.Core.WorkProcess;
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
    public class RoomController : BaseGameController
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        // GET: Room
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddRoom(int? player)
        {
            RenderJobType(null);
            RenderGoal(null);
            RenderSoftwareProcessType(null);
            RenderMultiPlayer(player);
            RenderAvartar(null);
            return View("Add");
        }

        [HttpPost]
        public ActionResult AddRoom(FormCollection form)
        {
            var roomName = form["Add.RoomName"];
            var startMoney = int.Parse(form["Add.StartMoney"]);
            var goal = int.Parse(form["Add.Goal"]);
            var softwareProcessType = int.Parse(form["Add.SoftwareProcessType"]);
            var playerName = form["Add.PlayerName"];
            var jobType = form["Add.JobType"];
            var multiPlayer = int.Parse(form["Add.MultiPlayer"]);
            var round = int.Parse(form["Add.Round"]);
            var imageUrl = form["Add.ImageUrl"];
            var expertPlayer = form["Add.ExpertPlayer"].ToLower() == "false" ? false : true;
            //Initial Game to Game Battle
            //Set game started

            var gameRoom = _service.GameRoom().AddRoom(new Entity.GameRoom
            {
                Active = true,
                CreateDate = DateTime.UtcNow,
                GameRoomName = roomName,
                Goal = goal,
                MoneyValue = startMoney,
                Multiplayer = multiPlayer,
                StartDate = multiPlayer > 1 ? (DateTime?)null : DateTime.UtcNow,
                EndDate = multiPlayer > 1 ? (DateTime?)null : DateTime.UtcNow.AddMinutes(15),
                TeamValue = 2, //if startup
                SoftwareType = softwareProcessType,
                ProjectValue = 0,
                GameRound = round,
                IncludeBot = expertPlayer,
                UserId = Singleton.User().UserId // get from session
            });

            if (gameRoom != null)
            {
                _service.Game().CreateGameAsync(gameRoom.GameRoomId, gameRoom.SoftwareType, gameRoom.GameRound);
                if (multiPlayer > 1)
                {
                    //multiplayer > wait room

                    _service.GameRoom().AddUserGameRoom(new Entity.UserGameRoom
                    {
                        GameRoomId = gameRoom.GameRoomId,
                        PlayerName = playerName,
                        JobType = int.Parse(jobType),
                        MoneyValue = gameRoom.MoneyValue,
                        ProjectValue = gameRoom.ProjectValue,
                        TeamValue = gameRoom.TeamValue,
                        TurnValue = 1,
                        GameFinished = null,
                        JoinDate = DateTime.UtcNow,
                        UserId = Singleton.User().UserId,
                        Active = true,
                        ImageUrl = imageUrl,
                        IsBot = false,
                    });

                    Singleton.CreateGameSession(gameRoom.TeamValue, gameRoom.ProjectValue, gameRoom.MoneyValue,
                        gameRoom.GameRoomId, playerName, gameRoom.SoftwareType, imageUrl);

                    return RedirectToAction("WaitRoom", "Room", new { id = gameRoom.GameRoomId });
                }
                else
                {
                    //Single > start game
                    //Todo Create UserGameRoom and log
                    _service.GameRoom().AddUserGameRoom(new Entity.UserGameRoom
                    {
                        GameRoomId = gameRoom.GameRoomId,
                        PlayerName = playerName,
                        JobType = int.Parse(jobType),
                        MoneyValue = gameRoom.MoneyValue,
                        ProjectValue = gameRoom.ProjectValue,
                        TeamValue = gameRoom.TeamValue,
                        GameFinished = null,
                        JoinDate = DateTime.UtcNow,
                        UserId = Singleton.User().UserId,
                        Active = true,
                        TurnValue = 1,
                        ImageUrl = imageUrl,
                        IsBot = false,
                    });

                    if (expertPlayer == true)
                    {
                        //create 2 bot Async
                        _service.BotExpert().CreateBotExpertAsync(gameRoom);
                    }

                    Singleton.CreateGameSession(gameRoom.TeamValue, gameRoom.ProjectValue, gameRoom.MoneyValue,
                        gameRoom.GameRoomId, playerName, gameRoom.SoftwareType, imageUrl);

                    return RedirectToAction("Index", "Story", new { id = gameRoom.GameRoomId });
                }
            }
            return RedirectToAction("Index", "Home");
        }

        public ActionResult JoinRoom(int id)
        {
            var recall = ReCallUserGameRoom(id);
            if (recall == "waitroom")
            {
                return RedirectToAction("WaitRoom", "Room", new { id });
            }

            var isGameStart = IsGameStart(Singleton.User().UserId, id);
            if (isGameStart)
            {
                return RedirectToAction("Index", "GameStart", new { id });
            }

            RenderJobType(null);
            RenderAvartar(null);
            var model = new GameRoomModel
            {
                GameRoomId = id,
            };
            return View("Join", model);
        }
        public ActionResult WaitRoom(int id)
        {
            ReCallUserGameRoom(id);
            var gameRoom = _service.GameRoom().GetRoomById(id);
            var model = new GameRoomModel
            {
                GameRoomId = id,
                SoftwareType = gameRoom.SoftwareType,
                MoneyInGame = gameRoom.MoneyValue,
            };
            return View("WaitRoom", model);
        }

        [HttpPost]
        public ActionResult JoinGameRoom(FormCollection form)
        {
            var roomId = int.Parse(form["Add.GameRoomId"]);
            var playerName = form["Add.PlayerName"];
            var jobType = form["Add.JobType"];
            var imageUrl = form["Add.ImageUrl"];
            var gameRoom = _service.GameRoom().GetRoomById(roomId);

            if (gameRoom == null) return RedirectToAction("Index", "Dashboard");

            _service.GameRoom().AddUserGameRoom(new Entity.UserGameRoom
            {
                GameRoomId = gameRoom.GameRoomId,
                PlayerName = playerName,
                JobType = int.Parse(jobType),
                MoneyValue = gameRoom.MoneyValue,
                ProjectValue = gameRoom.ProjectValue,
                TeamValue = gameRoom.TeamValue,
                TurnValue = 1,
                GameFinished = null,
                JoinDate = DateTime.UtcNow,
                UserId = Singleton.User().UserId,
                Active = true,
                ImageUrl = imageUrl
            });

            Singleton.CreateGameSession(gameRoom.TeamValue, gameRoom.ProjectValue, gameRoom.MoneyValue,
                gameRoom.GameRoomId, playerName, gameRoom.SoftwareType, imageUrl);

            return RedirectToAction("WaitRoom", "Room", new { id = roomId });
        }






        public ActionResult GetListRoom()
        {
            var r1 = _service.GameRoom().GetAllGameRoom();

            // var r1 = _service.GameRoom().GetAllUser();
            var model = new List<GameRoomModel>();
            foreach (var item in r1)
            {
                var r2 = _service.GameRoom().GetAllUserGameRoom(item.GameRoomId);
                var g = new GameRoomModel
                {
                    GameRoomId = item.GameRoomId,
                    GameRoomName = item.GameRoomName,
                    MaxPlayer = item.Multiplayer,
                    Player = r2.Count()
                };
                model.Add(g);
            }

            //   var model = _service.GameRoom().GetAllGameRoom2();
            return PartialView("_GameRoomList", model);
        }

        public ActionResult GetPlayer(int id)
        {
            var userGameRoom = _service.GameRoom().GetAllUserGameRoom(id);
            var gameRoom = _service.GameRoom().GetRoomById(id);
            var model = new GameRoomModel
            {
                GameRoomId = id,
                UserGameRooms = userGameRoom.ToList(),
                MaxPlayer = gameRoom.Multiplayer,
                Player = userGameRoom.Count(),
                CreateByUserId = gameRoom.UserId,
            };
            return PartialView("_PlayerList", model);
        }


        public void RenderSoftwareProcessType(int? value)
        {
            var selectSoftwareProcessType = new List<SelectListItem>
            {
                new SelectListItem { Text = "Water Fall", Value = "0" },
                new SelectListItem { Text = "Agile", Value = "1", Selected = true },
                new SelectListItem { Text = "Custom", Value = "9" }
            };
            ViewBag.SelectSoftwareProcessType = selectSoftwareProcessType;
        }
        public void RenderGoal(int? value)
        {
            var selectGoal = new List<SelectListItem>
            {
                new SelectListItem { Text = "Max Money", Value = "0", Selected = true },
                new SelectListItem { Text = "Money Over 50 Percent", Value = "1", Selected = false },
                new SelectListItem { Text = "Money Over 60 Percent", Value = "2", Selected = false },
                new SelectListItem { Text = "Money Over 75 Percent", Value = "3", Selected = false }
            };
            ViewBag.SelectGoal = selectGoal;
        }
        public void RenderJobType(int? value)
        {
            var selectJobType = new List<SelectListItem>
            {
                new SelectListItem { Text = "Start Up (+2 Person)", Value = "0", Selected = true }
            };
            ViewBag.SelectJobType = selectJobType;
        }
        public void RenderAvartar(int? value)
        {
            var selectAvartar = new List<SelectListItem>
            {
                new SelectListItem { Text = "Picture 1", Value = "/Content/sufee/images/boy.png", Selected = true },
                  new SelectListItem { Text = "Picture 2", Value = "/Content/sufee/images/man.png" },
                    new SelectListItem { Text = "Picture 3", Value = "/Content/sufee/images/man2.png" },
                      new SelectListItem { Text = "Picture 4", Value = "/Content/sufee/images/girl.png"},
                         new SelectListItem { Text = "Picture 5", Value = "/Content/sufee/images/girl2.png" }
            };
            ViewBag.SelectAvartar = selectAvartar;
        }
        public void RenderMultiPlayer(int? value)
        {
            var selectMultiPlayer = new List<SelectListItem>();
            if (value == 1)
            {
                selectMultiPlayer.Add(new SelectListItem { Text = "1", Value = "1", Selected = true });
            }
            else
            {
                selectMultiPlayer.Add(new SelectListItem { Text = "2", Value = "2", Selected = true });
                selectMultiPlayer.Add(new SelectListItem { Text = "3", Value = "3" });
                selectMultiPlayer.Add(new SelectListItem { Text = "4", Value = "4" });
            }
            ViewBag.SelectMultiPlayer = selectMultiPlayer;
        }

        public void InitialGame(int team, int project, int money, int roomId)
        {
            HttpCookie cookie = Request.Cookies["UserGame"];
            var adminData = new UserGameModel
            {
                Team = team,
                Project = project,
                Money = money,
                GameSession = Guid.NewGuid().ToString(),
                UserId = Singleton.User().UserId,
                GameRoomId = roomId,
            };
            var serializer = new JavaScriptSerializer();
            cookie = new HttpCookie("UserGame", serializer.Serialize(adminData));
            Response.SetCookie(cookie); //SetCookie() is used for update the cookie.
            Response.Cookies.Add(cookie);
        }


        public string ReCallUserGameRoom(int gameRoomId)
        {
            int? userId = Singleton.User() != null ? Singleton.User().UserId : (int?)null;
            if (userId.HasValue)
            {
                var currentUserGameRoom = _service.GameRoom().GetUserGameRoom(Singleton.User().UserId, gameRoomId);
                var currentGameRoom = _service.GameRoom().GetRoomById(gameRoomId);
                if (currentUserGameRoom != null && currentGameRoom != null)
                {
                    if (currentUserGameRoom.Active.GetValueOrDefault())
                    {
                        if (Singleton.Game() == null)
                        {
                            Singleton.CreateGameSession(currentUserGameRoom.TeamValue, currentUserGameRoom.ProjectValue, currentUserGameRoom.MoneyValue,
                           currentUserGameRoom.GameRoomId, currentUserGameRoom.PlayerName, currentGameRoom.SoftwareType, currentUserGameRoom.ImageUrl);

                        }
                        return "waitroom";
                    }
                }
            }
            return string.Empty;
        }

        public bool IsGameStart(int userId, int gameRoomId)
        {
            var gameRoom = _service.GameRoom().GetGameRoomByUserId(userId, gameRoomId);
            if (gameRoom != null)
            {
                if (gameRoom.StartDate.HasValue)
                {
                    return true;
                }
            }
            return false;
        }
    }
}