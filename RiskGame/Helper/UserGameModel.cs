using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskGame.Helper
{
    [Serializable]
    public class UserGameModel
    {
        public string Authorization { get; set; }
        public int UserId { get; set; }
        public string TimeZone { get; set; }
        public string Culture { get; set; }
        public int? CompanyId { get; set; }
        public int? ProgramTypeId { get; set; }
        public int Role { get; set; }
        public int Money { get; set; }
        public int Project { get; set; }
        public int Team { get; set; }
        public List<CompanyModel> InsuranceCompanies { get; set; }

        public string GameSession { get; set; }

        // public List<ProgramType> ProgramType { get; set; }

        //public UserGameModel()
        //{

        //}
        public UserGameModel()
        {
            InsuranceCompanies = new List<CompanyModel>();
           // Money = money;

         //   ProgramType = new List<ProgramType>();
         // ProgramType.Add(new ProgramType { ProgramTypeId = 1047, ProgramTypeName = "Individual" });
         // ProgramType.Add(new ProgramType { ProgramTypeId = 1048, ProgramTypeName = "Group" });
        }

        [Serializable]
        public class ProgramType
        {
            public int ProgramTypeId { get; set; }
            public string ProgramTypeName { get; set; }
        }

        [Serializable]
        public class CompanyModel
        {
            public int CompanyId { get; set; }
            public string CompanyName { get; set; }
        }
    }
}