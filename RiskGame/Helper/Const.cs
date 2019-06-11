using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskGame.Helper
{
    public static class Const
    {
        public enum RiskType
        {
            General = 0,
            Requirement = 1,
            Design = 2,
            Implement = 3,
            Testing = 4,
            Support = 5,
            Training = 6,          
        }
        public enum ActionEffectType
        {
            Money = 1,
            Team = 2,
            Project = 3
        }

        public enum JobType
        {
            StartUp = 0,
            ExpertSpecialist = 99,
            Newbies = 100
        }

        public enum SoftwareType
        {
            WaterFall = 0,
            Agile = 1,
            Custom = 9
        }
        public enum GoalType
        {
            MaxMoney = 0,
            MoneyOver50Percent = 1,
            MoneyOver60Percent = 2,
            MoneyOver75Percent = 3
        }

        public enum RiskGameLevel
        {
            ZeroLevel = 0,
            FirstLevel = 1,
            SecondLevel = 2,
            ThirdLevel = 3
        }
        public enum ProtecStatus
        {
            Lose = 1,
            Draw = 2,
            Win = 3
        }

        public enum GemeStatus
        {
            Playing = 0,
            GameDone = 1
        }
    }
}