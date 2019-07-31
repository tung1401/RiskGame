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

        public void CreateWaterFallModel(int gameRoomId, int round)
        {
            try
            {
                var listReq = new List<Risk>();
                var listDesign = new List<Risk>();
                var listDev = new List<Risk>();
                var listQA = new List<Risk>();
                var listSupport = new List<Risk>();
                var listAll = new List<Risk>();
                // getAll Risk with Risk option
                var risks = _service.Risk().GetAllRiskWithOutZeroLevel().ToList();

                listAll.AddRange(risks.Where(x => x.RiskType == (int)RiskType.General));

                var generalReq = listAll.OrderBy(x => Guid.NewGuid()).Take(5).ToList();
                // separate type
                listReq.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Requirement));
                listReq.AddRange(generalReq);

                var generalDesign = listAll.OrderBy(x => Guid.NewGuid()).Take(5).ToList();
                listDesign.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Design));
                listDesign.AddRange(generalDesign);

                var generalDev = listAll.OrderBy(x => Guid.NewGuid()).Take(5).ToList();
                listDev.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Implement));
                listDev.AddRange(generalDev);

                var generalQA = listAll.OrderBy(x => Guid.NewGuid()).Take(5).ToList();
                listQA.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Testing));
                listQA.AddRange(generalQA);

                var generalSupport = listAll.OrderBy(x => Guid.NewGuid()).Take(5).ToList();
                listSupport.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Support));
                listSupport.AddRange(generalSupport);

                var gameBattleList = new List<GameBattle>();

                //take Random
                var turn = 0;
                for (int i = 0; i < round; i++)
                {
                    if (i > 0)
                    {
                        turn += 5;
                    }

                    gameBattleList.AddRange(GenerateWorkProcess(gameRoomId, listReq, ((int)RiskType.Requirement + turn)));
                    gameBattleList.AddRange(GenerateWorkProcess(gameRoomId, listDesign, ((int)RiskType.Design + turn)));
                    gameBattleList.AddRange(GenerateWorkProcess(gameRoomId, listDev, ((int)RiskType.Implement + turn)));
                    gameBattleList.AddRange(GenerateWorkProcess(gameRoomId, listQA, ((int)RiskType.Testing + turn)));
                    gameBattleList.AddRange(GenerateWorkProcess(gameRoomId, listSupport, ((int)RiskType.Support + turn)));
                }
                //save DB
                foreach (var gameBattle in gameBattleList)
                {
                    _service.Game().SaveGameBattleAsync(gameBattle);
                }
            }
            catch (Exception ex)
            {

            }
        }

        private List<GameBattle> GenerateWorkProcess(int gameRoomId, List<Risk> risks, int turn)
        {
            var gameBattleList = new List<GameBattle>();
            if (risks.Any())
            {
                var maxRisk = risks.Count > 15 ? 15 : risks.Count;
                var randomTake = CommonFunction.RandomNumber(1, maxRisk);
                foreach (var risk in risks.OrderBy(x => Guid.NewGuid()).Take(randomTake))
                {
                    var isProbability = true;
                    if (risk.RiskType == (int)RiskType.General)
                    {
                        isProbability = CommonFunction.IsProbability(risk.RiskProbability);
                    }

                    if (isProbability)
                    {
                        var randomRiskOptionLevel = CommonFunction.RandomNumber(1, 3);
                        var riskOption = risk.RiskOptions.FirstOrDefault(x => x.RiskLevel == randomRiskOptionLevel);
                        if (riskOption != null)
                        {
                            int? riskNewsId = null;
                            var riskNews = GetRandomRiskNews(risk.RiskId, risk.RiskProbability.GetValueOrDefault());
                            if (riskNews != null)
                            {
                                riskNewsId = riskNews.RiskNewsId;
                            }

                            var game = new GameBattle
                            {
                                GameRoomId = gameRoomId,
                                RiskId = risk.RiskId,
                                RiskOptionId = riskOption.RiskOptionId,
                                Ratio = CommonFunction.RandomNumber(1, 3),
                                Turn = turn,
                                ActionEffectType = riskOption.ActionEffectType,
                                ActionEffectValue = riskOption.ActionEffectValue,
                                RiskNewsId = riskNewsId
                            };
                            gameBattleList.Add(game);
                        }
                    }
                }

                if (!gameBattleList.Any())
                {
                    var defaultRisk = risks.OrderBy(x => Guid.NewGuid()).LastOrDefault();
                    var riskOption = defaultRisk.RiskOptions.FirstOrDefault(x => x.RiskLevel == (int)RiskGameLevel.FirstLevel);
                    var game = new GameBattle
                    {
                        GameRoomId = gameRoomId,
                        RiskId = defaultRisk.RiskId,
                        RiskOptionId = riskOption.RiskOptionId,
                        Ratio = CommonFunction.RandomNumber(1, 3),
                        Turn = turn,
                        ActionEffectType = riskOption.ActionEffectType,
                        ActionEffectValue = riskOption.ActionEffectValue
                        // no news
                    };
                    gameBattleList.Add(game);
                }

                return gameBattleList;
            }
            return null;
        }

        public void CreateCustomWorkProcessModel(int gameRoomId, int round)
        {
            //custom 1 turn = 1 round
            try
            {
                //get all risk
                var allRiskOption = _service.Risk().GetAllRiskOptionWithoutZeroLevel().ToList();
                if (allRiskOption.Any())
                {
                    var list = new List<GameBattle>();
                    for (int turn = 1; turn <= round; turn++)
                    {
                        var randomTake = CommonFunction.RandomNumber(1, 5);
                        var listRiskInTurn = new List<Risk>();

                        var riskOptionRandom = allRiskOption.OrderBy(x => Guid.NewGuid()).Take(randomTake)
                            .GroupBy(d => d.Risk.RiskId).Select(x => x.FirstOrDefault());

                        foreach (var risk in riskOptionRandom)
                        {
                            if (!listRiskInTurn.Any(x => x.RiskId == risk.Risk.RiskId) || !listRiskInTurn.Any())
                            {
                                listRiskInTurn.Add(risk.Risk);

                                int? riskNewsId = null;
                                var riskNews = GetRandomRiskNews(risk.Risk.RiskId, risk.Risk.RiskProbability.GetValueOrDefault());
                                if (riskNews != null)
                                {
                                    riskNewsId = riskNews.RiskNewsId;
                                }

                                var gameBattle = new GameBattle
                                {
                                    GameRoomId = gameRoomId,
                                    RiskId = risk.RiskId.GetValueOrDefault(),
                                    RiskOptionId = risk.RiskOptionId,
                                    Ratio = CommonFunction.RandomNumber(1, 3),
                                    Turn = turn,
                                    ActionEffectType = risk.ActionEffectType,
                                    ActionEffectValue = risk.ActionEffectValue,
                                    RiskNewsId = riskNewsId
                                };
                                _service.Game().SaveGameBattleAsync(gameBattle);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        public List<Risk> GenerateRiskChioceModel(int softwareType, int gameRoomId, int turn)
        {
            var maxTake = 10;
            var risks = new List<Risk>();
            var gameBattleList = _service.Game().GetGameBattleOpenRisk(gameRoomId, turn);
            if (softwareType == (int)SoftwareType.WaterFall)
            {
                if (gameBattleList.Any())
                {
                    //TODO Fix bug
                    var riskInGame = gameBattleList.LastOrDefault(x => x.Risk.RiskType != (int)RiskType.General);
                    if (riskInGame != null)
                    {
                        var risk = riskInGame.Risk;
                        risks = _service.Risk().GetRiskByType(risk.RiskType.GetValueOrDefault(), true).ToList();
                    }
                    else
                    {
                        var riskType = CalculateRiskTypeFromWaterFallTurn(turn);
                        risks = _service.Risk().GetRiskByType(riskType, true).ToList();
                    }
                }
                else
                {
                    risks = _service.Risk().GetRiskByType((int)RiskType.Requirement, true).ToList();
                }
            }
            else if (softwareType == (int)SoftwareType.Agile)
            {
                if (gameBattleList.Any())
                {
                    var allRisks = _service.Risk().GetAllRisk().OrderBy(x => Guid.NewGuid());
                    var gameRisks = gameBattleList.Select(x => x.Risk); //หา Risk ใน Game battle
                    var allRisksExceptGameRisks = allRisks.Except(gameRisks).ToList(); //เอา Risk ทั้งหมดยกเว้นใน Game battle

                    var expectItem = 2;
                    var reqCount = gameRisks.Count(x => x.RiskType == (int)RiskType.Requirement);
                    var designCount = gameRisks.Count(x => x.RiskType == (int)RiskType.Design);
                    var devCount = gameRisks.Count(x => x.RiskType == (int)RiskType.Implement);
                    var qaCount = gameRisks.Count(x => x.RiskType == (int)RiskType.Testing);
                    var supportCount = gameRisks.Count(x => x.RiskType == (int)RiskType.Support);
                    var generalCount = gameRisks.Count(x => x.RiskType == (int)RiskType.General);

                    if (reqCount < expectItem)
                    {
                        risks.AddRange(allRisksExceptGameRisks.Where(x => x.RiskType == (int)RiskType.Requirement)
                            .OrderBy(x => Guid.NewGuid()).Take(expectItem - reqCount));
                    }
                    if (designCount < expectItem)
                    {
                        risks.AddRange(allRisksExceptGameRisks.Where(x => x.RiskType == (int)RiskType.Design)
                            .OrderBy(x => Guid.NewGuid()).Take(expectItem - designCount));
                    }
                    if (devCount < expectItem)
                    {
                        risks.AddRange(allRisksExceptGameRisks.Where(x => x.RiskType == (int)RiskType.Implement)
                            .OrderBy(x => Guid.NewGuid()).Take(expectItem - devCount));
                    }
                    if (qaCount < expectItem)
                    {
                        risks.AddRange(allRisksExceptGameRisks.Where(x => x.RiskType == (int)RiskType.Testing)
                            .OrderBy(x => Guid.NewGuid()).Take(expectItem - qaCount));
                    }
                    if (supportCount < expectItem)
                    {
                        risks.AddRange(allRisksExceptGameRisks.Where(x => x.RiskType == (int)RiskType.Support)
                            .OrderBy(x => Guid.NewGuid()).Take(expectItem - supportCount));
                    }
                    if (generalCount < expectItem)
                    {
                        risks.AddRange(allRisksExceptGameRisks.Where(x => x.RiskType == (int)RiskType.General)
                            .OrderBy(x => Guid.NewGuid()).Take(expectItem - generalCount));
                    }

                    /*
                    if (!gameRisks.Any(x => x.RiskType == (int)RiskType.Requirement))
                    {
                        if(gameRisks.Count(x=>x.RiskType == (int)RiskType.Requirement) > 2)
                        risks.Add(allRisks.Where(x => x.RiskType == (int)RiskType.Requirement).FirstOrDefault());
                    }
                    if (!gameRisks.Any(x => x.RiskType == (int)RiskType.Design))
                    {
                        risks.Add(allRisks.Where(x => x.RiskType == (int)RiskType.Design).FirstOrDefault());
                    }
                    if (!gameRisks.Any(x => x.RiskType == (int)RiskType.Implement))
                    {
                        risks.Add(allRisks.Where(x => x.RiskType == (int)RiskType.Implement).FirstOrDefault());
                    }
                    if (!gameRisks.Any(x => x.RiskType == (int)RiskType.Testing))
                    {
                        risks.Add(allRisks.Where(x => x.RiskType == (int)RiskType.Testing).FirstOrDefault());
                    }
                    if (!gameRisks.Any(x => x.RiskType == (int)RiskType.Support))
                    {
                        risks.Add(allRisks.Where(x => x.RiskType == (int)RiskType.Support).FirstOrDefault());
                    }*/
                    risks.AddRange(gameRisks);
                }
                else
                {
                    // risks = _service.Risk().GetAllRisk().OrderBy(x => Guid.NewGuid()).Take(maxTake).ToList();
                }
            }
            else
            {
                var gameRisks = gameBattleList.Select(x => x.Risk); //หา Risk ใน Game battle
                var allRisksExceptGameRisks = _service.Risk().GetAllRisk().Except(gameRisks).ToList(); //เอา Risk ทั้งหมดยกเว้นใน Game battle

                risks.AddRange(gameRisks);
                risks.AddRange(allRisksExceptGameRisks);
                if (risks.Count >= maxTake)
                {
                    risks = risks.Take(maxTake).ToList();
                }
            }
            return risks.OrderBy(x => Guid.NewGuid()).ToList();
        }

        private int CalculateRiskTypeFromWaterFallTurn(int turn)
        {
            if (turn <= 5)
            {
                return turn;
            }

            if (turn % 5 == 0)
            {
                return 5;
            }

            return turn % 5;
        }

        public void CreateAgileWorkModel(int gameRoomId, int round)
        {
            try
            {
                var gameBattleList = new List<GameBattle>();
                for (int i = 1; i <= round; i++)
                {
                    var model = GetRiskByTypeData(false);
                    var randomTake = CommonFunction.RandomNumber(3, 6);
                    var risks = new List<Risk>
                    {
                          model.Req,  model.Design,  model.Dev,  model.QA,  model.Support
                    }
                    .Union(model.ListGeneral)
                    .OrderBy(x => Guid.NewGuid()).Take(randomTake);

                    foreach (var item in risks)
                    {
                        var riskOptionItem = item.RiskOptions.OrderBy(x => Guid.NewGuid()).FirstOrDefault();

                        int? riskNewsId = null;
                        var riskNews = GetRandomRiskNews(item.RiskId, item.RiskProbability.GetValueOrDefault());
                        if (riskNews != null)
                        {
                            riskNewsId = riskNews.RiskNewsId;
                        }

                        var game = new GameBattle
                        {
                            GameRoomId = gameRoomId,
                            RiskId = item.RiskId,
                            RiskOptionId = riskOptionItem.RiskOptionId,
                            Ratio = CommonFunction.RandomNumber(1, 3),
                            Turn = i,
                            ActionEffectType = riskOptionItem.ActionEffectType,
                            ActionEffectValue = riskOptionItem.ActionEffectValue,
                            RiskNewsId = riskNewsId
                        };
                        gameBattleList.Add(game);
                    }
                }

                if (gameBattleList.Any())
                {
                    foreach (var gameBattle in gameBattleList)
                    {
                        _service.Game().SaveGameBattleAsync(gameBattle);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }

        //Internal helper method
        internal RiskSeparateModel GetRiskByTypeData(bool isList)
        {
            var model = new RiskSeparateModel();
            var risks = _service.Risk().GetAllRiskWithOutZeroLevel();
            // separate type
            if (isList)
            {
                model.ListReq.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Requirement || x.RiskType == (int)RiskType.General));
                model.ListDesign.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Design || x.RiskType == (int)RiskType.General));
                model.ListDev.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Implement || x.RiskType == (int)RiskType.General));
                model.ListQA.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Testing || x.RiskType == (int)RiskType.General));
                model.ListSupport.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Support || x.RiskType == (int)RiskType.General));

            }
            else
            {
                model.Req = risks.Where(x => x.RiskType == (int)RiskType.Requirement).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                model.Design = risks.Where(x => x.RiskType == (int)RiskType.Design).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                model.Dev = risks.Where(x => x.RiskType == (int)RiskType.Implement).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                model.QA = risks.Where(x => x.RiskType == (int)RiskType.Testing).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                model.Support = risks.Where(x => x.RiskType == (int)RiskType.Support).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                model.ListGeneral = risks.Where(x => x.RiskType == (int)RiskType.General).OrderBy(x => Guid.NewGuid()).Take(5).ToList(); // get top 5 for random
            }
            return model;
        }

        internal RiskNews GetRandomRiskNews(int riskId, int riskProbability)
        {
            //random for risk news
            var isNews = CommonFunction.IsProbability(riskProbability);
            if (isNews)
            {
                return _service.Risk().GetRandomRiskNews(riskId);
            }
            return null;
        }
    }

    public class RiskSeparateModel
    {
        public List<Risk> ListReq { set; get; }
        public List<Risk> ListDesign { set; get; }
        public List<Risk> ListDev { set; get; }
        public List<Risk> ListQA { set; get; }
        public List<Risk> ListSupport { set; get; }
        public List<Risk> ListGeneral { set; get; }

        public Risk Req { set; get; }
        public Risk Design { set; get; }
        public Risk Dev { set; get; }
        public Risk QA { set; get; }
        public Risk Support { set; get; }
        public Risk General { set; get; }
    }
}