﻿using RiskGame.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KPI.Services.Interface
{
    public interface IRiskService
    {
        Risk GetById(int id);
        Risk Add(Risk risk);
        RiskOption AddRiskOption(RiskOption riskOption);
        IEnumerable<Risk> GetAll();
        IEnumerable<Risk> GetAllRisk();
        IEnumerable<Risk> GetRiskByType(int type, bool includeGeneral);
        IEnumerable<Risk> GetRiskById(int id);
        IEnumerable<RiskOption> GetAllRiskOption();
        RiskOption GetRiskOptionById(int riskOptionId, int actionEffectType);
        IEnumerable<RiskOption> GetAllRiskOptionWithoutZeroLevel();

        IEnumerable<Risk> GetAllRiskWithOutZeroLevel();
        IEnumerable<RiskOption> GetAllRiskOptionByRiskId(int riskId, int? level);

        RiskNews GetRandomRiskNews(int riskId);
        RiskNews GetRiskNewsById(int riskNewsId);
    }
}
