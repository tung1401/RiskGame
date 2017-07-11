using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;

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
    }
}