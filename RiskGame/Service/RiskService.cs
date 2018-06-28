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
    public class RiskService : IRiskService
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        private readonly IRiskRepository _risk;
        private readonly IRiskOptionRepository _riskOption;

        public RiskService(RiskRepository risk, RiskOptionRepository riskOption)
        {
            _risk = risk;
            _riskOption = riskOption;
        }

        //
        public IEnumerable<Risk> GetAllRisk()
        {
            return _risk.GetAllWith(inc => inc.RiskOptions);
        }
        public IEnumerable<Risk> GetRiskById(int id)
        {
            return _risk.GetManyWith(x => x.RiskId == id, inc => inc.RiskOptions);
        }
        public IEnumerable<Risk> GetRiskByType(int type)
        {
            return _risk.GetManyWith(x => x.RiskType == type,inc => inc.RiskOptions);
        }

        public IEnumerable<RiskOption> GetAllRiskOption()
        {
            return _riskOption.GetAllWith(inc => inc.Risk);
        }

        public RiskOption GetRiskOptionById(int riskOptionId, int actionEffectType)
        {
            return _riskOption.GetWith(x => x.RiskOptionId == riskOptionId && x.ActionEffectType == actionEffectType, inc => inc.Risk);
        }

        
    }
}
