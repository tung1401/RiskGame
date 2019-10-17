using RiskGame.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskGame.Models
{
    public class RiskDataModel
    {
        public List<RiskData> RiskData { get; set; }
    }

    public class RiskData
    {
        public int RiskId { get; set; }
        public string Name { get; set; }
        public string RiskType { get; set; }
        public int? RiskTypeValue { get; set; }
        public string RiskDetail { get; set; }
        public string RiskExpert { get; set; }
        public Nullable<int> RiskProbability { get; set; }
        public Nullable<int> RiskImpact { get; set; }
        public List<RiskOption> RiskOption { get; set; }
        public List<RiskNews> RiskNews { get; set; }
    }

    //public class RiskOption
    //{
    //    public int RiskOptionId { get; set; }
    //    public int ActionEffectType { get; set; } //money,team,project
    //    public int Value { get; set; }
    //}


    public class RiskProgressModel
    {
        public List<RiskProgressData> RiskProgressData { get; set; }
    }

    public class RiskProgressData
    {
        public int RiskId { get; set; }
        public int RiskLevel { get; set; } //1-3
        public string RiskName { get; set; }
        public string RiskDetail { get; set; }
        public int RiskType { get; set; } //req,design,program,test,support
        public int ActionEffectType { get; set; } //money,team,project
        public int Value { get; set; }
    }
}