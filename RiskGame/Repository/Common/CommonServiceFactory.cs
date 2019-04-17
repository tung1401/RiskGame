using KPI.Services.Interface;
using KPI.Services.Service;
using RiskGame.Core.BotExpert;
using RiskGame.DAL;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskGame.Repository.Common
{
    public class CommonServiceFactory : IDisposable
    {
        private readonly DataContext _db = new DataContext();
        public readonly string ConnectionString = ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;

        public void Dispose()
        {
            _db.Dispose();
            GC.SuppressFinalize(this);
        }
        public IRiskService Risk()
        {
            return new RiskService(
                 new RiskRepository(_db),
                 new RiskOptionRepository(_db),
                 new RiskNewsRepository(_db)
                );
        }
        public IGameService Game()
        {
            return new GameService(
                   new GameBattleRepository(_db),
                   new UserGameBattleRepository(_db),
                   new UserGameBattleLogRepository(_db),
                   new UserGameRiskRepository(_db)
                );
        }
        public IGameRoomService GameRoom()
        {
            return new GameRoomService(
                   new GameRoomRepository(_db),
                   new UserGameRoomRepository(_db)
                //new UserGameRoomRepository(_db)
                );
        }


        public IUserService User()
        {
            return new UserService(
                   new UserRepository(_db)
                );
        }


        public IBotExpertService BotExpert()
        {
            return new BotExpertService();
        }
    }
}
