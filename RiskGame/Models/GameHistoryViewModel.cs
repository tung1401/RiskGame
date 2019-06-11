using RiskGame.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskGame.Models
{
    public class GameHistoryViewModel
    {
        public List<GameRoom> GameRoomList { set; get; }
        public GameRoom GameRoom { set; get; }
        public List<UserGameRoom> UserGameRoomList { set; get; }
        public List<RiskOption> RiskOptionList { set; get; }
    }
}