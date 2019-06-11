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
    [Authorize]
    public class GameHistoryController : Controller
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        // GET: GameHistory

        public ActionResult Index()
        {
            var userId = Singleton.User().UserId;
            var data = _service.GameRoom().GetUserHistoryGame(Singleton.User().UserId);
            var model = new GameHistoryViewModel
            {
                UserGameRoomList = data.ToList()
            };
            return View(model);
        }

        public ActionResult Detail(int gameRoomId)
        {
            // get gameroom detail and user protect risk
            var userId = Singleton.User().UserId;
            var data = _service.GameRoom().GetGameHistoryDetail(gameRoomId);
            var model = new GameHistoryViewModel
            {
                GameRoom = data,
                RiskOptionList = _service.Risk().GetAllRiskOption().ToList()
            };
            return View(model);
        }
    }
}