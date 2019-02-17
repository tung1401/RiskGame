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

namespace KPI.Services.Service
{
    public class UserService : IUserService
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        private readonly IUserRepository _user;

        public UserService(IUserRepository user)
        {
            _user = user;
        }

        public void Add(User user)
        {
            _user.Add(user);
        }

        
    }
}
