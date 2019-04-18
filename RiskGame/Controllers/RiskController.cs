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
    public class RiskController : BaseGameController
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        // GET: Risk
        public ActionResult Index(int id)
        {
            return View("Index");
        }


        [HttpGet]
        public JsonResult GetRiskNews(int gameRoomId, int turn)
        {
            var gameInTurn = _service.Game().GetGameBattleInTurn(gameRoomId, turn);
            var riskNewsFormat = string.Empty;
            if (gameInTurn.Any())
            {
                foreach (var item in gameInTurn)
                {
                    if(item.RiskNews != null)
                    {
                        riskNewsFormat += item.RiskNews.RiskNewsDetail;
                        if (item != gameInTurn.LastOrDefault())
                        {
                            riskNewsFormat += ", ";
                        }
                    }
                }
            }

            var response = CommonFunction.GetResponse(riskNewsFormat != null, string.Empty, riskNewsFormat);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}