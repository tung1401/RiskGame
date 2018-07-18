using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KPI.Services.Interface;
using RiskGame.Repository;
using RiskGame.Repository.Common;
using RiskGame.Repository.Interfaces;
using RiskGame.Entity;

namespace KPI.Services.Service
{
    public class GameService : IGameService
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        private readonly IGameBattleRepository _gameBattle;
        private readonly IUserGameBattleRepository _userGameBattle;
        private readonly IUserGameBattleLogRepository _userGameBattleLog;
        private readonly IUserGameRiskRepository _userGameRisk;
        public GameService(GameBattleRepository gameBattle, 
            UserGameBattleRepository userGameBattle,
            UserGameBattleLogRepository userGameBattleLog,
            UserGameRiskRepository userGameRisk
            )
        {
            _gameBattle = gameBattle;
            _userGameBattle = userGameBattle;
            _userGameBattleLog = userGameBattleLog;
            _userGameRisk = userGameRisk;
        }

        public IEnumerable<GameBattle> GetAllGameBattle()
        {
            return _gameBattle.GetAll();
        }

        public IEnumerable<GameBattle> GetGameBattleByGameRoomId(int gameRoomId)
        {
            return _gameBattle.GetManyWith(x => x.GameRoomId == gameRoomId, inc => inc.Risk, includes => includes.Risk.RiskOptions);
        }
        public IEnumerable<GameBattle> GetGameBattleOpenRisk(int gameRoomId, int turn)
        {
            return _gameBattle.GetManyWith(x => x.GameRoomId == gameRoomId && x.Turn == turn, inc => inc.Risk, includes => includes.Risk.RiskOptions);
        }
        public int GetMaxTurnByRoomId(int gameRoomId)
        {
            var data = _gameBattle.GetLastOrderBy(x => x.GameRoomId == gameRoomId, x=>x.Turn.GetValueOrDefault());
            return data.Turn.GetValueOrDefault();
        }

        public bool CheckMaxTurn(int gameRoomId, int turn)
        {
            var maxTurn = GetMaxTurnByRoomId(gameRoomId);
            return turn == maxTurn;
        }

        public void AddGameBattle(List<GameBattle> listEntity)
        {
            _gameBattle.AddList(listEntity);
        }
        public void AddGameBattle(GameBattle entity)
        {
            _gameBattle.Add(entity);
        }

        public async Task CreateGameAsync(int gameRoomId, int take = 2)
        {
            await Task.Run(() => CreateGame(gameRoomId, take));
        }
        public void CreateGame(int gameRoomId, int take)
        {
            try
            {
                //get all risk
                var allRiskOption = _service.Risk().GetAllRiskOption();
                if (allRiskOption.Any())
                {
                    var list = new List<GameBattle>();
                    var turn = 0;
                    foreach (var risk in allRiskOption.OrderBy(x => Guid.NewGuid()).Take(take))
                    {
                        turn++;
                        var game = new GameBattle
                        {
                            GameRoomId = gameRoomId,
                            RiskId = risk.RiskId.GetValueOrDefault(),
                            RiskOptionId = risk.RiskOptionId,
                            Ratio = new Random().Next(1, 2),
                            Turn = turn,
                            ActionEffectType = risk.ActionEffectType,
                            ActionEffectValue = risk.ActionEffectValue,
                        };
                        _gameBattle.AddAsync(game);
                    }
                }
            }
            catch(Exception ex)
            {

            }
        }



        //user game battle log
        public void AddUserGameBattleLog(UserGameBattleLog log)
        {
            _userGameBattleLog.Add(log);
        }




        public IEnumerable<UserGameRisk> GetUserGameRisk(int gameRoomId, int turn, int userId)
        {
           return _userGameRisk.GetManyWith(x => x.GameRoomId == gameRoomId && x.Turn == turn && x.UserId == userId, inc=>inc.RiskOption,includes=>includes.Risk);
        }
        public UserGameRisk AddUserGameRisk(UserGameRisk entity)
        {
            return _userGameRisk.Add(entity);
        }




    }
}
