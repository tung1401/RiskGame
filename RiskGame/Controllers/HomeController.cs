using RiskGame.DAL;
using RiskGame.Entity;
using RiskGame.Helper;
using RiskGame.Models;
using RiskGame.Repository;
using RiskGame.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RiskGame.Controllers
{
    public class HomeController : Controller
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        public ActionResult Index()
        {
            var t6 = 6 % 5;
            var t7 = 7 % 5;
            var t8 = 8 % 5;
            var t9 = 9 % 5;
            var t10 = 10 % 5;
            var t11 = 11 % 5;
            var t12 = 12 % 5;
            var t13 = 13 % 5;
            var t14 = 14 % 5;

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        [HttpGet]
        public JsonResult GetOpenRiskGameBattle()
        {
            var list = new OpenRiskGameBattleModel
            {
                RiskGameBattleData = new List<GameBattleData>
                {
                    new GameBattleData
                    {
                        title ="ความเสี่ยงที่คุณได้รับมีทั้งหมด",
                        type = "warning"
                    },
                    new GameBattleData
                    {
                        title ="3 ความเสี่ยง คือ",
                        type = "warning"
                    },
                    new GameBattleData
                    {
                        title ="1",
                        imageUrl = "/Content/sufee/images/logo.png"
                    },
                    new GameBattleData
                    {
                        title ="2",
                        imageUrl = "/Content/sufee/images/logo.png"
                    },
                    new GameBattleData
                    {
                        title ="3",
                        imageUrl = "/Content/sufee/images/logo.png"
                    },
                }
            };
            var response = CommonFunction.GetResponse(list != null, string.Empty, list);
            return Json(response, JsonRequestBehavior.AllowGet);
        }
    }
}