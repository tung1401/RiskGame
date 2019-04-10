using RiskGame.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace RiskGame.Core.BotExpert
{
    public interface IBotExpertService
    {
        Task CreateBotExpertAsync(GameRoom gameRoom);
        Task BotProtectRiskAsync(int gameRoomId, int turn, int jobType);
    }
}