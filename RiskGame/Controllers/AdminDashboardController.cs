using RiskGame.Entity;
using RiskGame.Helper;
using RiskGame.Models;
using RiskGame.Repository.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RiskGame.Controllers
{
    [Authorize]
    public class AdminDashboardController : Controller
    {
        private readonly CommonServiceFactory _service = new CommonServiceFactory();
        // GET: AdminDashboard
        public ActionResult Index()
        {
            var data = _service.Risk().GetAll();
            var model = new RiskDataModel
            {
                RiskData = data.Select(x => new RiskData
                {
                    RiskId = x.RiskId,
                    Name = x.RiskName,
                    RiskDetail = x.RiskDetail,
                    RiskType = Enum.GetName(typeof(Const.RiskType), (int)x.RiskType)
                }).ToList()
            };
            return View(model);
        }

        public ActionResult AddRisk()
        {
            RenderRiskType(null);
            return View();
        }

        [HttpPost]
        public ActionResult AddRisk(FormCollection form)
        {
            // add risk
            var riskData = new Risk
            {
               RiskName = form["Add.RiskName"],
               RiskDetail = form["Add.RiskDetail"],
               RiskImpact = int.Parse(form["Add.RiskImpact"]),
               RiskProbability = int.Parse(form["Add.RiskProbability"]),
               RiskType = int.Parse(form["Add.RiskType"]),
               Active = form["Add.RiskActive"].ToLower() == "false" ? false : true
            };

            var risk = _service.Risk().Add(riskData);
            if(risk != null)
            {
                // add risk option
                _service.Risk().AddRiskOption(new RiskOption
                {
                    RiskId = risk.RiskId,
                    RiskLevel = 0,
                    ActionEffectType = 1,
                    ActionEffectValue = 0
                });
                //level 1 * 100
                _service.Risk().AddRiskOption(new RiskOption
                {
                    RiskId = risk.RiskId,
                    RiskLevel = 1,
                    RiskImageUrl = $"/Content/risks/{risk.RiskId}/{risk.RiskId}-1.png",
                    ActionEffectType = 1,
                    ActionEffectValue = (risk.RiskProbability * risk.RiskImpact) * 100
                });

                //level 2 * 150
                _service.Risk().AddRiskOption(new RiskOption
                {
                    RiskId = risk.RiskId,
                    RiskLevel = 2,
                    RiskImageUrl = $"/Content/risks/{risk.RiskId}/{risk.RiskId}-2.png",
                    ActionEffectType = 1,
                    ActionEffectValue = (risk.RiskProbability * risk.RiskImpact) * 150
                });

                //level 3 * 200
                _service.Risk().AddRiskOption(new RiskOption
                {
                    RiskId = risk.RiskId,
                    RiskLevel = 3,
                    RiskImageUrl = $"/Content/risks/{risk.RiskId}/{risk.RiskId}-3.png",
                    ActionEffectType = 1,
                    ActionEffectValue = (risk.RiskProbability * risk.RiskImpact) * 200                   
                });
            }

            return RedirectToAction("Index", "AdminDashboard");
        }

        public ActionResult EditRisk(int id)
        {
            var risk = _service.Risk().GetById(id);
            RenderRiskType(risk.RiskType);
            var model = new RiskData
            {
                RiskId = risk.RiskId,
                Name = risk.RiskName,
                RiskDetail = risk.RiskDetail,
                RiskProbability = risk.RiskProbability,
                RiskImpact = risk.RiskImpact,
                RiskType = Enum.GetName(typeof(Const.RiskType), (int)risk.RiskType),
                RiskOption = risk.RiskOptions.ToList(),
                RiskNews = risk.RiskNews.ToList()
            };
            return View(model);
        }

        [HttpPost]
        public ActionResult EditRisk(FormCollection form)
        {
            var id = int.Parse(form["Edit.RiskId"]);
            var verifyRisk = _service.Risk().GetById(id);
            if (verifyRisk != null)
            {
                verifyRisk.RiskName = form["Edit.RiskName"];
                verifyRisk.RiskDetail = form["Edit.RiskDetail"];
                verifyRisk.RiskImpact = int.Parse(form["Edit.RiskImpact"]);
                verifyRisk.RiskProbability = int.Parse(form["Edit.RiskProbability"]);
                verifyRisk.RiskType = int.Parse(form["Edit.RiskType"]);
                verifyRisk.Active = form["Edit.RiskActive"].ToLower() == "false" ? false : true;
              
                if(verifyRisk.RiskOptions.Any())
                {
                    //update RiskOptions
                }
                else
                {
                    // add RiskOptions
                }


                if (verifyRisk.RiskNews.Any())
                {
                    // update last record
                }
                else
                {
                    // add new record
                }                
            }

            return RedirectToAction("Index", "AdminDashboard");
        }



        public void RenderRiskType(int? value)
        {
            var selectRiskType = new List<SelectListItem>
            {
                new SelectListItem { Text = "General", Value = "0", Selected = value == 0 },
                new SelectListItem { Text = "Requirement", Value = "1", Selected = value == 1 },
                new SelectListItem { Text = "Design", Value = "2", Selected = value == 2 },
                new SelectListItem { Text = "Implement", Value = "3" , Selected = value == 3},
                new SelectListItem { Text = "Testing", Value = "4" , Selected = value == 4 },
                new SelectListItem { Text = "Support", Value = "5" , Selected = value == 5 }
            };
            ViewBag.SelectRiskType = selectRiskType;
        }

    }
}