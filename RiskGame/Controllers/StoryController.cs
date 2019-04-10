using RiskGame.Models;
using RiskGame.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RiskGame.Controllers
{
    public class StoryController : BaseGameController
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        // GET: Story
        public ActionResult Index(int id)
        {
          //  ReCallUserGameRoom(id);
            var gameRoom = _service.GameRoom().GetRoomById(id);
            var model = new GameRoomModel
            {
                GameRoomId = id,
                SoftwareType = gameRoom.SoftwareType,
                MoneyInGame = gameRoom.MoneyValue,
            };
            return View("Index", model);
        }
    }
}