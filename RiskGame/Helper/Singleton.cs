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
            var cookie = HttpContext.Current.Request.Cookies["User"];
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


        public static void CreateGameSession(int team, int project, int money, int roomId, string playerName, int softwareType)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["Game"];
            var gameData = new GameModel
            {
                Team = team,
                Project = project,
                Money = money,
                GameSession = Guid.NewGuid().ToString(),
                UserId = Singleton.User().UserId,
                PlayerName = playerName,
                GameRoomId = roomId,
                SoftwareType = softwareType,
                Turn = 1
            };
            var serializer = new JavaScriptSerializer();
            cookie = new HttpCookie("Game", serializer.Serialize(gameData));
            HttpContext.Current.Response.SetCookie(cookie); //SetCookie() is used for update the cookie.
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void UpdateGameSession(int team, int project, int money, int turn)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["Game"];
            var adminData = new GameModel
            {
                Team = team,
                Project = project,
                Money = money,
                GameSession = Guid.NewGuid().ToString(),
                UserId = Singleton.User().UserId,
                PlayerName = Singleton.Game().PlayerName,
                GameRoomId = Singleton.Game().GameRoomId,
                Turn = turn
            };
            var serializer = new JavaScriptSerializer();
            cookie = new HttpCookie("Game", serializer.Serialize(adminData));
            HttpContext.Current.Response.SetCookie(cookie); //SetCookie() is used for update the cookie.
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void ClearGameSession()
        {
            HttpContext.Current.Session["Game"] = null;
            HttpContext.Current.Response.Cookies["Game"].Expires = DateTime.Now.AddDays(-1);
        }

        public static GameModel Game()
        {
            var cookie = HttpContext.Current.Request.Cookies["Game"];
            if (cookie != null)
            {
                var serializer = new JavaScriptSerializer();
                var item = (GameModel)serializer.Deserialize(cookie.Value, typeof(GameModel));
                return item;
            }
            return null;
        }

        public static void AddUserSession(int team, int project, int money, int roomId)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["User"];
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
            cookie = new HttpCookie("User", serializer.Serialize(adminData));
            HttpContext.Current.Response.SetCookie(cookie); //SetCookie() is used for update the cookie.
            HttpContext.Current.Response.Cookies.Add(cookie);
        }

        public static void UpdateUserSession(int team, int project, int money, int turn)
        {
            HttpCookie cookie = HttpContext.Current.Request.Cookies["User"];
            var adminData = new UserGameModel
            {
                Team = team,
                Project = project,
                Money = money,
                GameSession = Guid.NewGuid().ToString(),
                UserId = Singleton.User().UserId,
                GameRoomId = Singleton.Game().GameRoomId,
                Turn = turn
            };
            var serializer = new JavaScriptSerializer();
            cookie = new HttpCookie("User", serializer.Serialize(adminData));
            HttpContext.Current.Response.SetCookie(cookie); //SetCookie() is used for update the cookie.
            HttpContext.Current.Response.Cookies.Add(cookie);
        }


       
    }
}