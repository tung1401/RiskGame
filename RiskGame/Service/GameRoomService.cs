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
                    using (var command = new SqlCommand(@"SELECT [GameRoomID], [GameRoomName], [MultiPlayer], [UserId] FROM [dbo].[GameRoom] Where Active = 1 and Multiplayer > 1", connection))
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
                                Multiplayer = (int)reader["MultiPlayer"],
                                UserId = (int)reader["UserId"],
                            });
                        }
                    }

                }
            }
            catch (Exception ex)
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
                    using (var command = new SqlCommand(@"SELECT [UserGameRoomID],[GameRoomID],[UserId],[PlayerName],[MoneyValue],[Active],[ImageUrl],[GameFinished] FROM [dbo].[UserGameRoom] Where [GameRoomID] = " + roomId, connection))
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
                                UserId = (int)reader["UserId"],
                                PlayerName = (string)reader["PlayerName"],
                                MoneyValue = (int)reader["MoneyValue"],
                                Active = string.IsNullOrEmpty(reader["Active"].ToString()) ? (bool?)null : (bool?)reader["Active"],
                                GameFinished = string.IsNullOrEmpty(reader["GameFinished"].ToString()) ? (bool?)null : (bool?)reader["GameFinished"],
                                ImageUrl = !string.IsNullOrEmpty(reader["ImageUrl"].ToString()) ? (string)reader["ImageUrl"] : null,
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

        public GameRoom GetGameRoomByIdSQL(int gameRoomId)
        {
            var gameRoom = new GameRoom();
            try
            {
                using (var connection = new SqlConnection(_service.ConnectionString))
                {
                    connection.Open();
                    using (var command = new SqlCommand(@"SELECT [GameRoomID], [GameRoomName], [MultiPlayer], [UserId], [StartDate], [EndDate], [Active] FROM [dbo].[GameRoom] Where Active = 1 and Multiplayer > 1 and GameRoomId = "+ gameRoomId, connection))
                    {
                        command.Notification = null;

                        var dependency = new SqlDependency(command);
                        dependency.OnChange += new OnChangeEventHandler(DependencyGameRoomId_OnChange);

                        if (connection.State == ConnectionState.Closed)
                            connection.Open();

                        var reader = command.ExecuteReader();

                        while (reader.Read())
                        {
                            gameRoom = new GameRoom
                            {
                                GameRoomId = (int)reader["GameRoomId"],
                                GameRoomName = (string)reader["GameRoomName"],
                                Multiplayer = (int)reader["MultiPlayer"],
                                UserId = (int)reader["UserId"],
                                StartDate = string.IsNullOrEmpty(reader["StartDate"].ToString()) ? (DateTime?)null : (DateTime?)reader["StartDate"],
                                EndDate = string.IsNullOrEmpty(reader["EndDate"].ToString()) ? (DateTime?)null : (DateTime?)reader["EndDate"],
                            };
                        }
                    }
                }
            }
            catch (Exception ex)
            {

            }
            return gameRoom;
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
            catch (Exception ex)
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

        public bool CheckGameProgress(int gameRoomId, int userId)
        {
            var item = _gameRoom.Get(x => x.GameRoomId == gameRoomId && x.UserId == userId && x.EndDate != null);
            return item != null ? true : false; // game done or game not start
        }

        public IEnumerable<UserGameRoom> GetCurrentUserGame(int gameRoomId)
        {
            return _userGameRoom.GetManyWith(x => x.GameRoomId == gameRoomId, inc => inc.GameRoom.GameBattles, inc => inc.User);
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

        private void DependencyGameRoomId_OnChange(object sender, SqlNotificationEventArgs e)
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

        public bool UpdateGameRoomDone(int userId, int gameRoomId)
        {
            var userGameRoom = GetUserGameRoom(userId, gameRoomId);
            if (userGameRoom != null && !userGameRoom.GameFinished.GetValueOrDefault())
            {
                userGameRoom.Active = false;
                userGameRoom.GameFinished = true;
                _userGameRoom.Update(userGameRoom);
            }

            if (userGameRoom.IsBot.GetValueOrDefault()) return true;

            var gameRoom = GetRoomById(gameRoomId);
            if (gameRoom != null)
            {
                if (gameRoom.Multiplayer == 1)
                {
                    gameRoom.Active = false;
                    gameRoom.EndDate = DateTime.UtcNow;
                    _gameRoom.Update(gameRoom);
                    return true;
                }
                else
                {
                    var userGameFinished = CountUserGameStatus(gameRoomId, true);
                    if (gameRoom.Multiplayer == userGameFinished)
                    {
                        gameRoom.Active = false;
                        gameRoom.EndDate = DateTime.UtcNow;
                        _gameRoom.Update(gameRoom);
                        return true;
                    }
                }
            }
            return false;
        }

        public void UpdateGameRoom(GameRoom gameRoom)
        {
            _gameRoom.Update(gameRoom);
        }

        public void UpdateUserGameRoom(int userId, int gameRoomId, int moneyValue, int turnValue)
        {
            var currentGame = GetUserGameRoom(userId, gameRoomId);
            if (currentGame != null && !currentGame.GameFinished.GetValueOrDefault())
            {
                currentGame.MoneyValue = moneyValue;
                currentGame.TurnValue = turnValue;
                _userGameRoom.Update(currentGame);
            }
        }


        public int CountUserGameStatus(int gameRoomId, bool? status = null)
        {
            return _userGameRoom.Count(x => x.GameFinished == status && x.GameRoomId == gameRoomId);
        }

        public UserGameRoom GetUserGameRoom(int userId, int gameRoomId)
        {
            return _userGameRoom.Get(x => x.UserId == userId && x.GameRoomId == gameRoomId);
        }
        public UserGameRoom GetUserGameRoom(int gameRoomId, bool isbot, int jobType)
        {
            return _userGameRoom.Get(x => x.GameRoomId == gameRoomId && x.IsBot == isbot && x.JobType == jobType);
        }

        public IEnumerable<UserGameRoom> GetUserGameRoom(int gameRoomId)
        {
            return _userGameRoom.GetMany(x => x.GameRoomId == gameRoomId);
        }

        public IEnumerable<GameRoom> GetGameHistory(int userId)
        {
            return _gameRoom.GetManyWith(x => x.UserId == userId, inc => inc.UserGameRooms, inc => inc.User);
        }
        public GameRoom GetRoomById(int gameRoomId)
        {
            return _gameRoom.Get(x => x.GameRoomId == gameRoomId);
        }

        public void SaveUserGameRoomAsync(UserGameRoom userGameRoom)
        {
            _userGameRoom.AddAsync(userGameRoom);
        }

    }
}
