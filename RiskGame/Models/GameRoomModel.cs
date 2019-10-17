using RiskGame.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskGame.Models
{
    public class GameRoomModel
    {
        public int GameRoomId { set; get; }
        public string GameRoomName { set; get; }
        public int Player { set; get; }
        public int MaxPlayer { set; get; }
        public string CreateBy { set; get; }
        public int CreateByUserId { set; get; }
        public int SoftwareType { set; get; }
        public int MoneyInGame { set; get; }

        public List<UserGameRoom> UserGameRooms { set; get; }

        public List<Risk> Risks { set; get; }

        public GameRoomModel()
        {
            UserGameRooms = new List<UserGameRoom>();
            Risks = new List<Risk>();
        }

        public List<UserGameRoom> GetResult()
        {
            return UserGameRooms.OrderByDescending(x => x.MoneyValue).ToList();
        }
    }
}