using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;


namespace ConstructionApp.Services.Repository
{
    public class ProjectsDashboardRepository : DapperGenericRepository<ProjectsDashboard>, IProjectsDashboardRepository
    {
        public ProjectsDashboardRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
}
