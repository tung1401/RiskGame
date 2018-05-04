using RiskGame.Models;
using RiskGame.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RiskGame.Controllers
{
    public class RoomController : Controller
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        // GET: Room
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult AddRoom()
        {
            RenderJobType(null);
            RenderGoal(null);
            RenderSoftwareProcessType(null);
            RenderMultiPlayer(null);
            return View("Add");
        }

        [HttpPost]
        public ActionResult AddRoom(FormCollection form)
        {
            var roomName = form["Add.RoomName"];
            var startMoney = form["Add.StartMoney"];
            var goal = form["Add.Goal"];
            var softwareProcessType = form["Add.SoftwareProcessType"];
            var playerName = form["Add.PlayerName"];
            var jobType = form["Add.JobType"];
            var multiPlayer = form["Add.MultiPlayer"];
        

            return View("Add");
        }

        public ActionResult JoinRoom(int id)
        {
            RenderJobType(null);
            return View("Join");
        }

        public ActionResult WaitRoom(int id)
        {
            var model = new GameRoomModel
            {
                GameRoomId = id
            };
            return View("WaitRoom", model);
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
            var userGameRoom =_service.GameRoom().GetAllUserGameRoom(id);
            var model = new GameRoomModel
            {
                GameRoomId = id,
                UserGameRooms = userGameRoom.ToList()
            };
            return PartialView("_PlayerList", model);
        }






        public void RenderSoftwareProcessType(int? value)
        {
            var selectSoftwareProcessType = new List<SelectListItem>();
            // var partnerList = _adminService.Partner().GetAllPartnerCoupon(Singleton.UserAdmin().CompanyId, Singleton.UserAdmin().ProgramTypeId.GetValueOrDefault());
            selectSoftwareProcessType.Add(new SelectListItem { Text = "Water Fall", Value = "0", Selected = true });
            //if (partnerList.Any())
            //{
            //    selectPartnerList.AddRange(
            //        partnerList.Select(
            //            m =>
            //                new SelectListItem
            //                {
            //                    Text = m.PartnerName,
            //                    Value = m.PartnerId.ToString(),
            //                    Selected = m.PartnerId == value
            //                }));
            //}
            ViewBag.SelectSoftwareProcessType = selectSoftwareProcessType;
        }
        public void RenderGoal(int? value)
        {
            var selectGoal = new List<SelectListItem>();

            selectGoal.Add(new SelectListItem { Text = "Max Money", Value = "0", Selected = true });
            ViewBag.SelectGoal = selectGoal;
        }
        public void RenderJobType(int? value)
        {
            var selectJobType = new List<SelectListItem>();
            selectJobType.Add(new SelectListItem { Text = "Start Up (+2 Person)", Value = "0", Selected = true });
            ViewBag.SelectJobType = selectJobType;
        }
        public void RenderMultiPlayer(int? value)
        {
            var selectMultiPlayer = new List<SelectListItem>();
            selectMultiPlayer.Add(new SelectListItem { Text = "1", Value = "1", Selected = true });
            selectMultiPlayer.Add(new SelectListItem { Text = "2", Value = "2" });
            selectMultiPlayer.Add(new SelectListItem { Text = "3", Value = "3"});
            selectMultiPlayer.Add(new SelectListItem { Text = "4", Value = "4"});
            ViewBag.SelectMultiPlayer = selectMultiPlayer;
        }
    }
}