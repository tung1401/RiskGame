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
                // getAll Risk with Risk option
                var risks = _service.Risk().GetAllRiskWithOutZeroLevel().ToList();

                // separate type
                listReq.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Requirement));
                listDesign.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Design));
                listDev.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Implement));
                listQA.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Testing));
                listSupport.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Support));

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
            catch(Exception ex)
            {

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
            return gameBattleList;
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
                            .GroupBy(d => d.Risk.RiskId).Select(x=>x.FirstOrDefault());

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
                else
                {
                    risks = _service.Risk().GetRiskByType((int)RiskType.Requirement).ToList();
                }
            }
            else if (softwareType == (int)SoftwareType.Agile)
            {
                if (gameBattleList.Any())
                {

                    var allRisks = _service.Risk().GetAllRisk().OrderBy(x => Guid.NewGuid());
                    var gameRisks = gameBattleList.Select(x => x.Risk); //หา Risk ใน Game battle
                    //var allRisksExceptGameRisks = allRisks.Except(gameRisks).ToList(); //เอา Risk ทั้งหมดยกเว้นใน Game battle

                    if(!gameRisks.Any(x => x.RiskType == (int)RiskType.Requirement))
                    {
                        risks.Add(allRisks.Where(x=>x.RiskType == (int)RiskType.Requirement).FirstOrDefault());
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
                    }
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
                risks = risks.Take(maxTake).ToList();
            }
            return risks.OrderBy(x => Guid.NewGuid()).ToList();
        }

        public void CreateAgileWorkModel(int gameRoomId, int round)
        {
            var gameBattleList = new List<GameBattle>();
            for (int i = 1; i <= round; i++)
            {
                var model = GetRiskByTypeData(false);
                var randomTake = CommonFunction.RandomNumber(1, 5);
                var risks = new List<Risk>
                {
                      model.Req,  model.Design,  model.Dev,  model.QA,  model.Support

                }.OrderBy(x => Guid.NewGuid()).Take(randomTake);

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




        //Internal helper method
        internal RiskSeparateModel GetRiskByTypeData(bool isList)
        {
            var model = new RiskSeparateModel();
            var risks = _service.Risk().GetAllRiskWithOutZeroLevel();
            // separate type
            if (isList)
            {
                model.ListReq.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Requirement));
                model.ListDesign.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Design));
                model.ListDev.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Implement));
                model.ListQA.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Testing));
                model.ListSupport.AddRange(risks.Where(x => x.RiskType == (int)RiskType.Support));
            }
            else
            {
                model.Req = risks.Where(x => x.RiskType == (int)RiskType.Requirement).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                model.Design = risks.Where(x => x.RiskType == (int)RiskType.Design).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                model.Dev = risks.Where(x => x.RiskType == (int)RiskType.Implement).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                model.QA = risks.Where(x => x.RiskType == (int)RiskType.Testing).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
                model.Support = risks.Where(x => x.RiskType == (int)RiskType.Support).OrderBy(x => Guid.NewGuid()).FirstOrDefault();
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

        public Risk Req { set; get; }
        public Risk Design { set; get; }
        public Risk Dev { set; get; }
        public Risk QA { set; get; }
        public Risk Support { set; get; }



    }

}