﻿using System;
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
    public class RiskRepository : Repository<Risk>, IRiskRepository
    {
        public RiskRepository(DataContext context)
            : base(context)
        {
        }
    }
}
