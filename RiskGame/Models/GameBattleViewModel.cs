using RiskGame.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RiskGame.Models
{
    public class GameBattleViewModel
    {

        public List<GameBattle> GameBattles { set; get; }
        public List<UserGameRisk> UserGameRisk { set; get; }
        public List<UserGameBattleData> UserGameBattleData { set; get; }
        public bool GameDone { set; get; }

        public OpenRiskGameBattleModel OpenRiskGameBattleModel { set; get; }
        public string OpenRiskGameBattleModelArray { set; get; }

        public GameBattleViewModel()
        {
            GameBattles = new List<GameBattle>();
            GameDone = false;
            UserGameRisk = new List<UserGameRisk>();
            UserGameBattleData = new List<UserGameBattleData>();
            OpenRiskGameBattleModel = new OpenRiskGameBattleModel();
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
        public int? RiskNewsImpact { set; get; }
        public double RiskNewsImpactPercent { set; get; }
    }


    public class OpenRiskGameBattleModel
    {
        public List<GameBattleData> RiskGameBattleData { set; get; }
    }

    public class GameBattleData
    {
        public string type { set; get; }
        public string title { set; get; }
        public string imageUrl { set; get; }
        public string imageWidth = "400";
        public string imageHeight = "200";      
    }
}