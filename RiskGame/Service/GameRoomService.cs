using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KPI.Services.Interface;
using RiskGame.Repository;
using RiskGame.Repository.Common;
using RiskGame.Repository.Interfaces;
using RiskGame.Entity;
using System.Data.SqlClient;
using System.Data;
using RiskGame.Models;

namespace KPI.Services.Service
{
    public class GameRoomService : IGameRoomService
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        private readonly IGameBattleRepository _gameBattle;
        private readonly IGameRoomRepository _gameRoom;
        private readonly IUserGameRoomRepository _userGameRoom;
        public GameRoomService(GameRoomRepository gameRoom, UserGameRoomRepository userGameRoom)
        {
            _gameRoom = gameRoom;
            _userGameRoom = userGameRoom;
        }

        public IEnumerable<GameRoom> GetAllGameRoom()
        {
            
            var gameRooms = new List<GameRoom>();
            try
            {
                using (var connection = new SqlConnection(_service.ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"SELECT [GameRoomID], [GameRoomName], [MultiPlayer] FROM [dbo].[GameRoom]", connection))
                    {
                        command.Notification = null;

                        var dependency = new SqlDependency(command);
                        dependency.OnChange += new OnChangeEventHandler(DependencyGameRoom_OnChange);

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();

                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            gameRooms.Add(item: new GameRoom
                            {
                                GameRoomId = (int)reader["GameRoomId"],
                                GameRoomName = (string)reader["GameRoomName"],
                                Multiplayer = (int)reader["MultiPlayer"]
                            });
                        }
                    }

                }
            }
            catch(Exception ex)
            {

            }
            return gameRooms;
        }

        public IEnumerable<UserGameRoom> GetAllUserGameRoom(int roomId)
        {
            var userGameRooms = new List<UserGameRoom>();
            try
            {
                using (var connection = new SqlConnection(_service.ConnectionString))
            {
                connection.Open();
                using (var command = new SqlCommand(@"SELECT [UserGameRoomID],[GameRoomID],[PlayerName] FROM [dbo].[UserGameRoom] Where [GameRoomID] = " + roomId, connection))
                {
                    command.Notification = null;

                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(DependencyUserGameRoom_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        userGameRooms.Add(item: new UserGameRoom
                        {
                            UserGameRoomId = (int)reader["UserGameRoomID"],
                            GameRoomId = (int)reader["GameRoomID"],
                            PlayerName = (string)reader["PlayerName"],
                        });
                    }
                }

                }
            }
            catch (Exception ex)
            {

            }
            return userGameRooms;
        }

        public IEnumerable<User> GetAllUser()
        {
            try
            {
                var gameRooms = new List<User>();
                using (var connection = new SqlConnection(_service.ConnectionString))
                {
                    connection.Open();

                    var query = @"SELECT [UserId], [FirstName] From [dbo].[Users]";

                    // var query = @"SELECT [GameRoomID], [GameRoomName] FROM [dbo].[GameRoom]";

                    using (var command = new SqlCommand(/*@"SELECT [GameRoomID], [GameRoomName] FROM [dbo].[GameRoom]"*/query, connection))
                    {
                        command.Notification = null;

                        var dependency = new SqlDependency(command);
                        dependency.OnChange += new OnChangeEventHandler(DependencyGameRoom_OnChange);

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();

                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            gameRooms.Add(item: new User
                            {
                                UserId = (int)reader["UserId"],
                                FirstName = (string)reader["FirstName"],
                                //Player = (int)reader["Player"],
                                //MaxPlayer = (int)reader["Multiplayer"]
                            });
                        }
                    }

                }
                return gameRooms;
            }
            catch(Exception ex)
            {

            }
            return null;
        }

        public GameRoom AddRoom(GameRoom entity)
        {
            return _gameRoom.Add(entity);
        }

        public void AddUserGameRoom(UserGameRoom userGameRoom)
        {
            _userGameRoom.Add(userGameRoom);
        }








        public IEnumerable<GameRoomModel> GetAllGameRoom2()
        {
            var gameRooms = new List<GameRoomModel>();
            using (var connection = new SqlConnection(_service.ConnectionString))
            {
                connection.Open();

                var query = @"SELECT gr.GameRoomId AS GameRoomId, GameRoomName, Multiplayer, PlayerName
                            from [dbo].[GameRoom] gr Inner Join [dbo].[UserGameRoom] ugr
                            ON gr.GameRoomId = ugr.GameRoomId";

               // var query = @"SELECT [GameRoomID], [GameRoomName] FROM [dbo].[GameRoom]";

                using (var command = new SqlCommand(/*@"SELECT [GameRoomID], [GameRoomName] FROM [dbo].[GameRoom]"*/query, connection))
                {
                    command.Notification = null;

                    var dependency = new SqlDependency(command);
                    dependency.OnChange += new OnChangeEventHandler(DependencyGameRoom_OnChange);

                    if (connection.State == ConnectionState.Closed)
                        connection.Open();

                    var reader = command.ExecuteReader();

                    while (reader.Read())
                    {
                        gameRooms.Add(item: new GameRoomModel
                        {
                            GameRoomId = (int)reader["GameRoomId"],
                            GameRoomName = (string)reader["GameRoomName"],
                            //Player = (int)reader["Player"],
                            //MaxPlayer = (int)reader["Multiplayer"]
                        });
                    }
                }

            }
            return gameRooms;
        }

        private void DependencyGameRoom_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                RiskGame.Helper.GameHub.ListGameRoom();
            }
        }

        private void DependencyUserGameRoom_OnChange(object sender, SqlNotificationEventArgs e)
        {
            if (e.Type == SqlNotificationType.Change)
            {
                RiskGame.Helper.GameHub.ListGameRoom();
            }
        }

        public GameRoom GetGameRoomByUserId(int userId, int gameRoomId)
        {
            return _gameRoom.Get(m => m.UserId == userId && m.GameRoomId == gameRoomId);
        }

    }
}
