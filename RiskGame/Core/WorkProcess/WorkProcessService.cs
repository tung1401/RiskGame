using RiskGame.DAL;
using RiskGame.Entity;
using RiskGame.Helper;
using RiskGame.Repository;
using RiskGame.Repository.Common;
using RiskGame.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using static RiskGame.Helper.Const;

namespace RiskGame.Core.WorkProcess
{
    public class WorkProcessService
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();

        public WorkProcessService()
        {
        }

        public void CreateWaterFallModel(int gameRoomId)
        {
            var listReq = new List<Risk>();
            var listDesign = new List<Risk>();
            var listDev = new List<Risk>();
            var listQA = new List<Risk>();
            var listSupport = new List<Risk>();
            // getAll Risk with Risk option
            var risks = _service.Risk().GetAllRisk();

            // separate type
            listReq.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Requirement));
            listDesign.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Design));
            listDev.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Implement));
            listQA.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Testing));
            listSupport.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Support));

            var gameBattleList = new List<GameBattle>();

            //take Random
            gameBattleList.AddRange(GenerateWorkProcess(gameRoomId, listReq, (int)RiskType.Requirement));
            gameBattleList.AddRange(GenerateWorkProcess(gameRoomId, listDesign, (int)RiskType.Design));
            gameBattleList.AddRange(GenerateWorkProcess(gameRoomId, listDev, (int)RiskType.Implement));
            gameBattleList.AddRange(GenerateWorkProcess(gameRoomId, listQA, (int)RiskType.Testing));
            gameBattleList.AddRange(GenerateWorkProcess(gameRoomId, listSupport, (int)RiskType.Support));

            //save DB
            foreach (var gameBattle in gameBattleList)
            {
                _service.Game().SaveGameBattleAsync(gameBattle);
            }
        }

        private List<GameBattle> GenerateWorkProcess(int gameRoomId, List<Risk> risks, int turn)
        {
            var gameBattleList = new List<GameBattle>();
            if (risks.Any())
            {
                var randomTake = CommonFunction.RandomNumber(1, risks.Count);
                foreach (var risk in risks.OrderBy(x => Guid.NewGuid()).Take(randomTake))
                {
                    var randomRiskOptionLevel = CommonFunction.RandomNumber(1,3);
                    var riskOption = risk.RiskOptions.FirstOrDefault(x => x.RiskLevel == randomRiskOptionLevel);
                    if (riskOption != null)
                    {
                        var game = new GameBattle
                        {
                            GameRoomId = gameRoomId,
                            RiskId = risk.RiskId,
                            RiskOptionId = riskOption.RiskOptionId,
                            Ratio = CommonFunction.RandomNumber(1, 2),
                            Turn = turn,
                            ActionEffectType = riskOption.ActionEffectType,
                            ActionEffectValue = riskOption.ActionEffectValue,
                        };
                        gameBattleList.Add(game);
                    }
                }
            }
            return gameBattleList;
        }

        public void CreateCustomWorkProcessModel(int gameRoomId, int take)
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
                        var gameBattle = new GameBattle
                        {
                            GameRoomId = gameRoomId,
                            RiskId = risk.RiskId.GetValueOrDefault(),
                            RiskOptionId = risk.RiskOptionId,
                            Ratio = CommonFunction.RandomNumber(1, 2),
                            Turn = turn,
                            ActionEffectType = risk.ActionEffectType,
                            ActionEffectValue = risk.ActionEffectValue,
                        };
                        _service.Game().SaveGameBattleAsync(gameBattle);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public List<Risk> GenerateRiskChioceModel(int softwareType, int gameRoomId, int turn)
        {
            var maxTake = 5;

            var risks = new List<Risk>();
            var gameBattleList = _service.Game().GetGameBattleOpenRisk(gameRoomId, turn);
            if (softwareType == (int)SoftwareType.WaterFall)
            {
                if (gameBattleList.Any())
                {
                    var risk = gameBattleList.LastOrDefault().Risk;
                    if (risk != null)
                    {
                        risks = _service.Risk().GetRiskByType(risk.RiskType.GetValueOrDefault()).ToList();
                    }
                }
            }
            else
            {

                var allRisks = _service.Risk().GetAllRisk().OrderBy(x => Guid.NewGuid()).Take(maxTake);
                var gameRisks = gameBattleList.Select(x => x.Risk).ToList();
                risks = allRisks.Except(gameRisks).ToList();
                risks.AddRange(gameRisks);
            }
            return risks;
        }
    }
}