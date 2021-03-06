﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using KPI.Services.Interface;
using RiskGame.Repository;
using RiskGame.Repository.Common;
using RiskGame.Repository.Interfaces;
using RiskGame.Entity;
using static RiskGame.Helper.Const;

namespace KPI.Services.Service
{
    public class RiskService : IRiskService
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        private readonly IRiskRepository _risk;
        private readonly IRiskOptionRepository _riskOption;
        private readonly IRiskNewsRepository _riskNews;
        public RiskService(RiskRepository risk, RiskOptionRepository riskOption, RiskNewsRepository riskNews)
        {
            _risk = risk;
            _riskOption = riskOption;
            _riskNews = riskNews;
        }

        public Risk Add(Risk risk)
        {
            return _risk.Add(risk);
        }

        public RiskOption AddRiskOption(RiskOption riskOption)
        {
            return _riskOption.Add(riskOption);
        }

        public IEnumerable<Risk> GetAll()
        {
            return _risk.GetAll();
        }

        public Risk GetById(int id)
        {
            return _risk.GetWith(x => x.RiskId == id, inc => inc.RiskOptions, inc => inc.RiskNews);
        }

        public IEnumerable<Risk> GetAllRisk()
        {
            return _risk.GetManyWith(x => x.Active == true, inc => inc.RiskOptions);
        }

        public IEnumerable<Risk> GetAllRiskWithOutZeroLevel()
        {
            var risks = GetAllRisk().Select( x=> new Risk
            {
                RiskId = x.RiskId,
                RiskName = x.RiskName,
                RiskDetail = x.RiskDetail,
                RiskType = x.RiskType,
                RiskOptions = x.RiskOptions.Where(m => m.RiskLevel != 0).ToList()
            });

            return risks;
        }

        public IEnumerable<Risk> GetRiskById(int id)
        {
            return _risk.GetManyWith(x => x.RiskId == id && x.Active == true, inc => inc.RiskOptions);
        }
        public IEnumerable<Risk> GetRiskByType(int type, bool includeGeneral)
        {
            if (includeGeneral)
            {
                return _risk.GetManyWith(x => (x.RiskType == type || x.RiskType == (int)RiskType.General) && x.Active == true, inc => inc.RiskOptions);
            }

            return _risk.GetManyWith(x => x.RiskType == type && x.Active == true, inc => inc.RiskOptions);
        }

        public IEnumerable<RiskOption> GetAllRiskOption()
        {
            return _riskOption.GetAllWith(inc => inc.Risk).Where(x => x.Risk.Active == true);
        }
        public IEnumerable<RiskOption> GetAllRiskOptionWithoutZeroLevel()
        {
            return _riskOption.GetManyWith(x => x.RiskLevel != (int)RiskGameLevel.ZeroLevel, inc => inc.Risk).Where(x => x.Risk.Active == true);
        }

        public RiskOption GetRiskOptionById(int riskOptionId, int actionEffectType)
        {
            var query = _riskOption.GetManyWith(x => x.RiskOptionId == riskOptionId && x.ActionEffectType == actionEffectType, inc => inc.Risk).Where(x=>x.Risk.Active == true);
            if (query.Any())
            {
                return query.FirstOrDefault();
            }
            return null;
        }

        public IEnumerable<RiskOption> GetAllRiskOptionByRiskId(int riskId, int? level)
        {
            if (level.HasValue)
            {
                return _riskOption.GetMany(x => x.RiskId == riskId && x.RiskLevel == level);
            }
            return _riskOption.GetMany(x => x.RiskId == riskId);
        }

        public RiskNews GetRandomRiskNews(int riskId)
        {
            var allNews = _riskNews.GetMany(x => x.RiskId == riskId);
            if (allNews.Any())
            {
                return allNews.OrderBy(r => Guid.NewGuid()).FirstOrDefault();
            }
            return null;
        }
        
        public RiskNews GetRiskNewsById(int riskNewsId)
        {
            return _riskNews.Get(x => x.RiskNewsId == riskNewsId);
        }
    }
}
