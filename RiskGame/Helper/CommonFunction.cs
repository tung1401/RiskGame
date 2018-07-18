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
    }
}