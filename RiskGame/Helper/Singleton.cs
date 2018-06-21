using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Security;

namespace RiskGame.Helper
{
    public  static class Singleton
    {
        public static UserGameModel User()
        {
            var cookie = HttpContext.Current.Request.Cookies["UserGame"];
            if (cookie != null)
            {
                var serializer = new JavaScriptSerializer();
                var item = (UserGameModel)serializer.Deserialize(cookie.Value, typeof(UserGameModel));
                if (item.CompanyId == null || item.CompanyId == 0)
                {
                    item.CompanyId = null;
                }

                return item;
            }
            return null;
        }


        public static void CreateGameSession(int team, int project, int money, int roomId)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["UserGame"];
            var adminData = new UserGameModel
            {
                Team = team,
                Project = project,
                Money = money,
                GameSession = Guid.NewGuid().ToString(),
                UserId = Singleton.User().UserId,
                GameRoomId = roomId,
                Turn = 1
            };
            var serializer = new JavaScriptSerializer();
            cookie = new HttpCookie("UserGame", serializer.Serialize(adminData));
            HttpContext.Current.Response.SetCookie(cookie); //SetCookie() is used for update the cookie.
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void UpdateGameSession(int team, int project, int money, int turn)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["UserGame"];
            var adminData = new UserGameModel
            {
                Team = team,
                Project = project,
                Money = money,
                Turn = turn
            };
            var serializer = new JavaScriptSerializer();
            cookie = new HttpCookie("UserGame", serializer.Serialize(adminData));
            HttpContext.Current.Response.SetCookie(cookie); //SetCookie() is used for update the cookie.
            HttpContext.Current.Response.Cookies.Add(cookie);
        }
    }
}