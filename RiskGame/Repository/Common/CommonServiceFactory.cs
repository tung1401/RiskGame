using KPI.Services.Interface;
using KPI.Services.Service;
using RiskGame.DAL;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RiskGame.Repository.Common
{
    public class CommonServiceFactory : IDisposable
    {
        private readonly DataContext _db = new DataContext();

        public void Dispose()
        {
            _db.Dispose();
            GC.SuppressFinalize(this);
        }
        public IRiskService Risk()
        {
            return new RiskService(
                new RiskRepository(_db),
                 new RiskOptionRepository(_db) 
                );
        }
        public IGameService Game()
        {
            return new GameService(
                new GameBattleRepository(_db),
                 new UserGameBattleRepository(_db)
                );
        }

        //public IPredictiveService Predictive()
        //{
        //    return new PredictiveService(
        //        new PredictiveRepository(_db),
        //        new ProjectRepository(_db),
        //        new JSAScoreRepository(_db)
        //        );
        //}
        //public IProjectService Project()
        //{
        //    return new ProjectService(
        //        new ProjectRepository(_db),
        //        new ProjectEmployeeRepository(_db),
        //        new PredictiveRepository(_db)
        //        );
        //}
        //public ITimberlineService Timberline()
        //{
        //    return new TimberlineService(
        //        new TimberlineRepository(_db)
        //        );
        //}

        //public IEmployeeService Employee()
        //{
        //    return new EmployeeService(
        //        new EmployeeRepository(_db)
        //        );
        //}
        //public ICustomerService Customer()
        //{
        //    return new CustomerService(
        //        new CustomerRepository(_db)
        //        );
        //}
    }
}
