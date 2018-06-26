using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using RiskGame.DAL;
using RiskGame.Repository.Common;
using RiskGame.Entity;
using RiskGame.Repository.Interfaces;

namespace RiskGame.Repository
{
    public class UserGameRiskRepository : Repository<UserGameRisk>, IUserGameRiskRepository
    {
        public UserGameRiskRepository(DataContext context)
            : base(context)
        {
        }
    }
}
