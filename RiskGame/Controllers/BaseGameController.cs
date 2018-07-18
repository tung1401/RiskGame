using RiskGame.Helper;
using RiskGame.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace RiskGame.Controllers
{
    public class BaseGameController : Controller
    {
        public ActionResult BeginExecuteCoreActionResult { get; set; }
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        protected override IAsyncResult BeginExecuteCore(AsyncCallback callback, object state)
        {
            return base.BeginExecuteCore(callback, state);
        }

        protected override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (Singleton.Game() != null)
            {
                if (Singleton.Game().GameRoomId > 0 && Singleton.Game().Turn > 0)
                {
                    var checkGameProgress = _service.GameRoom().CheckGameProgress(Singleton.Game().GameRoomId, Singleton.Game().UserId);
                    if (checkGameProgress == false)
                    {
                        filterContext.Result = new RedirectToRouteResult(new RouteValueDictionary { { "action", "Index" }, { "controller", "Home" } });
                    }
                }
            }
        }
    }
}