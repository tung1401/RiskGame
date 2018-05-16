﻿using RiskGame.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Services.Interface
{
    public interface IGameService
    {
        //IEnumerable<Risk> GetAllRisk();
        //IEnumerable<Risk> GetRiskByType(int type);
        //IEnumerable<Risk> GetRiskById(int id);

        IEnumerable<GameBattle> GetGameBattleByGameRoomId(int gameRoomId);
        IEnumerable<GameBattle> GetAllGameBattle();
        Task CreateGameAsync(int gameRoomId, int take = 2);
    }
}
