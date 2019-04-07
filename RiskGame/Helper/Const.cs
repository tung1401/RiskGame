﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskGame.Helper
{
    public static class Const
    {
        public enum RiskType
        {
            Requirement = 1,
            Design = 2,
            Implement = 3,
            Testing = 4,
            Support = 5,
            Training = 6
        }
        public enum ActionEffectType
        {
            Money = 1,
            Team = 2,
            Project = 3
        }

        public enum JobType
        {
            StartUp = 0
        }
        public enum SoftwareType
        {
            WaterFall = 0,
            Agile = 1,
            Custom = 9
        }
        public enum GoalType
        {
            Money = 0
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