using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;


namespace ConstructionApp.Services.Repository
{
    public class ProjectDashboardRepository : DapperGenericRepository<ProjectDashboard>, IProjectDashboardRepository
    {
        public ProjectDashboardRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
}
