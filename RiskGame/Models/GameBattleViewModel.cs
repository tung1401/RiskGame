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

        public GameBattleViewModel()
        {
            GameBattles = new List<GameBattle>();
        }

    }
}