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
        GameRoom GetGameRoomByIdSQL(int gameRoomId);
        IEnumerable<GameRoomModel> GetAllGameRoom2();

        IEnumerable<User> GetAllUser();
        GameRoom AddRoom(GameRoom entity);

        void AddUserGameRoom(UserGameRoom userGameRoom);
        GameRoom GetGameRoomByUserId(int userId, int gameRoomId);
        bool UpdateGameRoomDone(int userId, int gameRoomId);
        bool CheckGameProgress(int gameRoomId, int userId);
        IEnumerable<UserGameRoom> GetCurrentUserGame(int gameRoomId);

        IEnumerable<GameRoom> GetGameHistory(int userId);
        GameRoom GetRoomById(int gameRoomId);
        void UpdateUserGameRoom(int userId, int gameRoomId, int moneyValue);
        void UpdateGameRoom(GameRoom gameRoom);

        UserGameRoom GetUserGameRoom(int userId, int gameRoomId);

    }
}
