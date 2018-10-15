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
        public List<UserGameRoom> UserGameRooms { set; get; }

        public GameRoomModel()
        {
            UserGameRooms = new List<UserGameRoom>();
        }

        public List<UserGameRoom> GetResult()
        {
            return UserGameRooms.OrderByDescending(x => x.MoneyValue).ToList();
        }

    }
}