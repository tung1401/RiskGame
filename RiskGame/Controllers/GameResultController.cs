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
            //Display all result

            //set status done

            var done = _service.GameRoom().UpdateGameRoomDone(Singleton.Game().UserId, Singleton.Game().GameRoomId);
            if (done)
            {
                Singleton.ClearGameSession();
            }

            // go to new game

            //clear session
           
            return View();
        }
        public ActionResult ReDashBoard()
        {

            return RedirectToAction("Index","DashBoard");
        }
    }
}