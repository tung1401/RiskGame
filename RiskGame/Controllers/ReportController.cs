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
    public class ReportController : Controller
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        // GET: Report
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UserGameHistory()
        {
            var userId = Singleton.User().UserId;
            var gameRoom = _service.GameRoom().GetGameHistory(userId);
            var model = new GameHistoryViewModel
            {
                GameRoomList = gameRoom.ToList()
            };
            return View(model);
        }
    }
}