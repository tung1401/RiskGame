using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RiskGame.Helper
{
    public static class CommonFunction
    {
        public static bool CheckCurrentGame()
        {
            return Singleton.Game() != null ? true : false;
        }

        [Serializable]
        public struct ResponseModel
        {
            public bool IsSuccess { get; set; }
            public string Description { get; set; }
            public Object Result { get; set; }
        }

        public static ResponseModel GetResponse(bool isSuccess, string description = null, Object result = null)
        {
            return new ResponseModel
            {
                IsSuccess = isSuccess,
                Description = description,
                Result = result
            };
        }
    }
}