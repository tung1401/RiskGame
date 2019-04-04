using RiskGame.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskGame.Models
{
    public class GameBattleViewModel
    {

        public List<GameBattle> GameBattles { set; get; }
        public List<UserGameRisk> UserGameRisk { set; get; }
        public List<UserGameBattleData> UserGameBattleData { set; get; }
        public bool GameDone { set; get; }

        public GameBattleViewModel()
        {
            GameBattles = new List<GameBattle>();
            GameDone = false;
            UserGameRisk = new List<UserGameRisk>();
            UserGameBattleData = new List<UserGameBattleData>();
        }

        

    }

    public class GameOpenRiskViewModel
    {
        


    }

    public class UserGameBattleData
    {
        public GameBattle GameBattle { set; get; }
        public string ProtectStatus { set; get; } // Can Win/Draw/Lost
        public int EffectMoney { set; get; } // Effect after open List

    }


}