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
        IEnumerable<UserGameRoom> GetAllUserGameRoom(int roomId);

        IEnumerable<GameRoomModel> GetAllGameRoom2();

        IEnumerable<User> GetAllUser();
        GameRoom AddRoom(GameRoom entity);

    }
}
