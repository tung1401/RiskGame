using RiskGame.Entity;
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
            System.Threading.Thread.Sleep(2000);
            //  ReCallUserGameRoom(id);
            var gameRoom = _service.GameRoom().GetRoomById(id);
            var gameBattleRisk = _service.Game().GetGameBattleByGameRoomId(id);
            var risk = _service.Risk().GetAll().OrderBy(x => Guid.NewGuid()).Take(10);

            var riskList = new List<Risk>();
            riskList.AddRange(risk);
            riskList.AddRange(gameBattleRisk.GroupBy(x => x.RiskId).Select(x => x.FirstOrDefault().Risk));

            var model = new GameRoomModel
            {
                GameRoomId = id,
                SoftwareType = gameRoom.SoftwareType,
                MoneyInGame = gameRoom.MoneyValue,
                Risks = riskList.GroupBy(x => x.RiskId).Select(x => x.FirstOrDefault()).ToList()
            };
            return View("Index", model);
        }
    }
}