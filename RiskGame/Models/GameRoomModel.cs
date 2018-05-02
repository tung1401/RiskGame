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
    }
}