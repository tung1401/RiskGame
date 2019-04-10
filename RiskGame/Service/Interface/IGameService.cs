using RiskGame.Entity;
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
        Task CreateGameAsync(int gameRoomId, int workprocessType, int round);
        IEnumerable<GameBattle> GetGameBattleOpenRisk(int gameRoomId, int turn);
        void AddUserGameBattleLog(UserGameBattleLog log);
        int GetMaxTurnByRoomId(int gameRoomId);
        bool CheckMaxTurn(int gameRoomId, int turn);

        IEnumerable<UserGameRisk> GetUserGameRisk(int gameRoomId, int turn);
        IEnumerable<UserGameRisk> GetUserGameRisk(int gameRoomId, int turn, int userId);
        UserGameRisk AddUserGameRisk(UserGameRisk entity);

        void SaveGameBattleAsync(GameBattle gameBattle);
    }
}
