using RiskGame.DAL;
using RiskGame.Entity;
using RiskGame.Helper;
using RiskGame.Repository;
using RiskGame.Repository.Common;
using RiskGame.Repository.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using static RiskGame.Helper.Const;

namespace RiskGame.Core.BotExpert
{
    public class BotExpertService : IBotExpertService
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();

        public BotExpertService()
        {
        }

        public async Task CreateBotExpertAsync(GameRoom gameRoom)
        {
            await Task.Run(() => CreateBotExpert(gameRoom));
        }
        private void CreateBotExpert(GameRoom gameRoom)
        {
            var userBotNewbies = new User();
            var userBotExpert = new User();
            using (var context = new DataContext())
            {
                userBotNewbies = context.Users.FirstOrDefault(x => x.BotExpert == (int)JobType.Newbies);
                userBotExpert = context.Users.FirstOrDefault(x => x.BotExpert == (int)JobType.ExpertSpecialist);
            }

            if (userBotNewbies != null)
            {
                var userGameBotLevel1 = new UserGameRoom
                {
                    GameRoomId = gameRoom.GameRoomId,
                    PlayerName = "Junior (Risk Newbies)",
                    JobType = (int)JobType.Newbies,
                    TurnValue = 1,
                    MoneyValue = gameRoom.MoneyValue,
                    ProjectValue = gameRoom.ProjectValue,
                    TeamValue = gameRoom.TeamValue,
                    GameFinished = null,
                    JoinDate = DateTime.UtcNow,
                    UserId = userBotNewbies.UserId,
                    Active = true,
                    IsBot = true,
                    ImageUrl = "/Content/sufee/images/newbies.png"
                };
                _service.GameRoom().SaveUserGameRoomAsync(userGameBotLevel1);
            }

            if (userBotExpert != null)
            {
                var userGameBotLevel3 = new UserGameRoom
                {
                    GameRoomId = gameRoom.GameRoomId,
                    PlayerName = "Expert User (Risk Specialist)",
                    JobType = (int)JobType.ExpertSpecialist,
                    TurnValue = 1,
                    MoneyValue = gameRoom.MoneyValue,
                    ProjectValue = gameRoom.ProjectValue,
                    TeamValue = gameRoom.TeamValue,
                    GameFinished = null,
                    JoinDate = DateTime.UtcNow,
                    UserId = userBotExpert.UserId,
                    Active = true,
                    IsBot = true,
                    ImageUrl = "/Content/sufee/images/expert.png"
                };
                _service.GameRoom().SaveUserGameRoomAsync(userGameBotLevel3);
            }
        }


        public User GetUser()
        {
            var userBotExpert = new User();
            using (var context = new DataContext())
            {
                userBotExpert = context.Users.FirstOrDefault(x => x.BotExpert == (int)JobType.ExpertSpecialist);
            }
            return userBotExpert;
        }

        //public User GetExpertUser()
        //{
        //    var userBotExpert = new User();
        //    using (var context = new DataContext())
        //    {
        //        userBotExpert = context.Users.FirstOrDefault(x => x.BotExpert == (int)JobType.ExpertSpecialist);
        //    }
        //    return userBotExpert;
        //}


        public async Task BotProtectRiskAsync(int gameRoomId, int turn, int jobType)
        {
            await Task.Run(() => BotProtectRisk(gameRoomId, turn, jobType));
        }
        public void BotProtectRisk(int gameRoomId, int turn, int jobType)
        {
            var userGameRiskInThisTurn = _service.Game().GetUserGameRisk(gameRoomId, turn).ToList();
            var gameBattleInThisTurn = _service.Game().GetGameBattleOpenRisk(gameRoomId, turn).ToList();
            var userGameRoom = _service.GameRoom().GetUserGameRoom(gameRoomId, true, jobType);
            var userId = userGameRoom.UserId;
            foreach (var item in userGameRiskInThisTurn)
            {
                //var riskOptionId = 0;
                var checkRiskInGameBattle = gameBattleInThisTurn.FirstOrDefault(x => x.RiskId == item.RiskId);
                var riskOptionId = AnalystRiskOption(jobType, checkRiskInGameBattle, item.RiskId);
                if (riskOptionId > 0)
                {
                    var botExpertUserGameRisk = new UserGameRisk
                    {
                        RiskId = item.RiskId,
                        Turn = turn,
                        GameRoomId = gameRoomId,
                        UserId = userId,
                        CreateDate = DateTime.UtcNow,
                        CreateBy = userId,
                        RiskOptionId = riskOptionId
                    };
                    // save bot expert user game Risk
                    _service.Game().AddUserGameRisk(botExpertUserGameRisk);
                }
            }

            //Calculate Protect Risk
            var currentMoney = userGameRoom.MoneyValue;
            var botExpertUserGameRiskList = _service.Game().GetUserGameRisk(gameRoomId, turn, userId).ToList();
            var moneyForProtectRisk = botExpertUserGameRiskList.Sum(x => x.RiskOption.ActionEffectValue);
            currentMoney = currentMoney - moneyForProtectRisk.GetValueOrDefault();
            _service.GameRoom().UpdateUserGameRoom(userId, gameRoomId, currentMoney, turn);

            var moneyTotal = CalculateRiskCostToBot(userId, gameRoomId, currentMoney, gameBattleInThisTurn, botExpertUserGameRiskList);

            // update game room
            _service.GameRoom().UpdateUserGameRoom(userId, gameRoomId, moneyTotal, turn);

            var gameDone = _service.Game().CheckMaxTurn(gameRoomId, turn);
            if (gameDone)
            {
                _service.GameRoom().UpdateGameRoomDone(userId, gameRoomId);
            }
        }

        public int AnalystRiskOption(int jobType, GameBattle gameBattle, int riskId)
        {
            var riskOptionId = 0;
            if (jobType == (int)JobType.ExpertSpecialist)
            {
                if (gameBattle != null)
                {
                    riskOptionId = gameBattle.RiskOptionId;
                }
                else
                {
                    var allRiskOption = _service.Risk().GetAllRiskOptionByRiskId(riskId, 0);
                    if (allRiskOption.Any())
                    {
                        riskOptionId = allRiskOption.FirstOrDefault().RiskOptionId;
                    }
                }
            }
            else if (jobType == (int)JobType.Newbies)
            {
                var newbiesRandomLevel = CommonFunction.RandomNumber(0, 1);
                var allRiskOption = _service.Risk().GetAllRiskOptionByRiskId(riskId, newbiesRandomLevel);
                if (allRiskOption.Any())
                {
                    riskOptionId = allRiskOption.FirstOrDefault().RiskOptionId;
                }
            }
            return riskOptionId;
        }
        public int CalculateRiskCostToBot(int userId, int gameRoomId, int currentMoney, List<GameBattle> listGameBattle, List<UserGameRisk> userGameRisk)
        {
            var moneyTotal = currentMoney;
            var userGameBattleData = new Models.UserGameBattleData
            {
                ProtectStatus = ProtecStatus.Lose.ToString()
            };

            foreach (var item in listGameBattle)
            {
                var effectItemMoney = item.Ratio.GetValueOrDefault() * item.ActionEffectValue.GetValueOrDefault();
                var riskProtect = userGameRisk.FirstOrDefault(x => x.RiskId == item.RiskId);
                var effectMoney = 0;
                if (riskProtect != null && riskProtect.RiskOption.RiskLevel != (int)RiskGameLevel.ZeroLevel)
                {
                    if (riskProtect.RiskOption.RiskLevel != item.RiskOption.RiskLevel)
                    {
                        if (riskProtect.RiskOption.RiskLevel > item.RiskOption.RiskLevel)
                        {
                            // ไม่ต้องจ่าย ป้องกันได้ 100%
                            //moneyTotal = Singleton.Game().Money;
                            userGameBattleData.ProtectStatus = ProtecStatus.Win.ToString();
                        }
                        else
                        {
                            if (riskProtect.RiskOption.RiskLevel == (int)RiskGameLevel.ThirdLevel)
                            {
                                //ป้องกัน 100%
                                // moneyTotal = Singleton.Game().Money;
                                userGameBattleData.ProtectStatus = ProtecStatus.Win.ToString();
                            }
                            else if (riskProtect.RiskOption.RiskLevel == (int)RiskGameLevel.SecondLevel)
                            {
                                //ป้องกัน 50% จ่าย 50%
                                effectMoney = (int)(effectItemMoney * 0.5);
                                moneyTotal = moneyTotal - (int)(effectItemMoney * 0.5);
                            }
                            else if (riskProtect.RiskOption.RiskLevel == (int)RiskGameLevel.FirstLevel)
                            {
                                //ป้องกัน 25% จ่าย 75%
                                effectMoney = (int)(effectItemMoney * 0.75);
                                moneyTotal = moneyTotal - (int)(effectItemMoney * 0.75);
                            }
                            else if (riskProtect.RiskOption.RiskLevel == (int)RiskGameLevel.ZeroLevel)
                            {
                                effectMoney = (int)(effectItemMoney * 1);
                                moneyTotal = moneyTotal - (int)(effectItemMoney * 1);
                            }
                        }
                    }
                    else
                    {
                        // ถ้าเลือกแล้ว Level เท่ากัน ป้องกันได้ 100%
                        // moneyTotal = Singleton.Game().Money;
                        userGameBattleData.ProtectStatus = ProtecStatus.Draw.ToString();
                    }
                }
                else
                {
                    // ถ้าไม่ได้เลือก หรือ ไม่ได้ป้องกัน จ่าย 100%
                    effectMoney = effectItemMoney;
                    moneyTotal = moneyTotal - effectItemMoney;
                }

                // ถ้าแพ้ และ มีข่าว จะโดนผลกระทบเพิ่ม
                if (item.RiskNewsId != null && userGameBattleData.ProtectStatus == ProtecStatus.Lose.ToString())
                {
                    // fact impact
                    var riskNews = _service.Risk().GetRiskNewsById(item.RiskNewsId.GetValueOrDefault());
                    if (riskNews != null)
                    {
                        var riskNewsImpactPercent = CommonFunction.RiskImpactFormat(riskNews.RiskNewsImpact.GetValueOrDefault());
                        var riskNewsImpact = (int)(effectItemMoney * riskNewsImpactPercent);

                        moneyTotal = moneyTotal - riskNewsImpact;
                        effectMoney = effectMoney + riskNewsImpact;

                        userGameBattleData.RiskNewsImpactPercent = riskNewsImpactPercent;
                        userGameBattleData.RiskNewsImpact = riskNewsImpact; // ค่าเงิน
                    }
                }
            }
            return moneyTotal;
        }
    }
}