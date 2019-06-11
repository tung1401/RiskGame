using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskGame.Models
{
    public class GameResultViewModel
    {
        public PlayerData MyPlayer { set; get; }
        public List<PlayerData> FriendPlayer { set; get; }
        public GameRoomModel GameRoom { set; get; }

    }


    public class PlayerData
    {
        public int GameBattleId { set; get; }
        public int GameRoomId { set; get; }
        public string PlayerName { set; get; }
        public string Rank { set; get; }
        public string Money { set; get; }
        public string Team { set; get; }
        public string Goal { set; get; }
        public string GoalStatus { set; get; }
        public string Project { set; get; }
        public string GameStatus { set; get; }
    }

}