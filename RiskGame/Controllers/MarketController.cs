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
    }
}