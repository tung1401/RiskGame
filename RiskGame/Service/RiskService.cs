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




        ////Project
        //public IEnumerable<Project> GetProjectByEmployeeId(Guid userId)
        //{
        //    return _projectEmployee.GetManyWith(m => m.EmployeeId == userId, inc => inc.Project).GroupBy(x => x.ProjectId).Select(x => new Project
        //    {
        //        ProjectId = x.FirstOrDefault().ProjectId.GetValueOrDefault(),
        //        ProjectName = x.FirstOrDefault().Project.ProjectName
        //    });

        //}

        //public IEnumerable<Project> GetAllProject()
        //{
        //    return _projectEmployee.GetAllWith(inc => inc.Project).GroupBy(m => m.Project).Select(x => new Project
        //    {
        //        ProjectId = x.FirstOrDefault().ProjectId.GetValueOrDefault(),
        //        ProjectName = x.FirstOrDefault().Project.ProjectName
        //    }).OrderBy(x=>x.ProjectName);

        //}

        //public Project Find(Guid projectId)
        //{
        //    return _project.Get(m => m.ProjectId == projectId);
        //}
        //public Project GetByNumber(string projectNumber)
        //{
        //    return _project.Get(m => m.ProjectNo == projectNumber);
        //}

        //public Project Add(Project project)
        //{
        //    return _project.Add(project);
        //}


        ////ProjectEmployee
        //public ProjectEmployee FindByEmployeeIdAndProjectId(Guid employeeId, Guid projectId)
        //{
        //    return _projectEmployee.Get(m => m.ProjectId == projectId && m.EmployeeId == employeeId);
        //}
        //public ProjectEmployee Add(ProjectEmployee projectEmployee)
        //{
        //    return _projectEmployee.Add(projectEmployee);
        //}
        //public ProjectEmployee Update(ProjectEmployee projectEmployee)
        //{
        //    return _projectEmployee.Add(projectEmployee);
        //}
        //public IEnumerable<Project> GetProjectByManager(Guid userId)
        //{
        //    return _projectEmployee.GetManyWith(m => m.EmployeeId == userId && m.IsPM == true, inc => inc.Project).GroupBy(x => x.ProjectId).Select(x => new Project
        //    {
        //        ProjectId = x.FirstOrDefault().ProjectId.GetValueOrDefault(),
        //        ProjectName = x.FirstOrDefault().Project.ProjectName
        //    });

        //}
        //public IEnumerable<ProjectEmployee> GetProjectManager()
        //{
        //    //var query = _projectEmployee.GetMany(m => m.IsPM == true).GroupBy(g => new {/* ProjectId = g.ProjectId , */EmployeeId = g.EmployeeId }).Select(e => new Employee
        //    //{

        //    //    EmployeeId = e.FirstOrDefault().EmployeeId.GetValueOrDefault(),
        //    //}).OrderBy(e => e.EmployeeId);

        //    var query = _projectEmployee.GetMany(m => m.IsPM == true).OrderBy(e => e.EmployeeId);

        //    return query;
        //}

    }
}
