using RiskGame.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Services.Interface
{
    public interface IRiskService
    {
        IEnumerable<Risk> GetAllRisk();
        IEnumerable<Risk> GetRiskByType(int type);
        IEnumerable<Risk> GetRiskById(int id);
        IEnumerable<RiskOption> GetAllRiskOption();
        RiskOption GetRiskOptionById(int riskOptionId, int actionEffectType);
    }
}
