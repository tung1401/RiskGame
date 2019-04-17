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
    public class RiskNewsRepository : Repository<RiskNews>, IRiskNewsRepository
    {
        public RiskNewsRepository(DataContext context)
            : base(context)
        {
        }
    }
}
