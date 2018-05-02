using RiskGame.Entity;
using RiskGame.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Services.Interface
{
    public interface IGameRoomService
    {

        IEnumerable<GameRoom> GetAllGameRoom();
        IEnumerable<UserGameRoom> GetAllUserGameRoom();

        IEnumerable<GameRoomModel> GetAllGameRoom2();

        IEnumerable<User> GetAllUser();

    }
}
