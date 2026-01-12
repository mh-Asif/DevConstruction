using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;


namespace ConstructionApp.Services.Repository
{
    public class ProjectSummeryRepository : DapperGenericRepository<ProjectSummery>, IProjectSummeryRepository
    {
        public ProjectSummeryRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
    public class ProjectRoleSummeryRepository : DapperGenericRepository<ProjectRoleSummery>, IProjectRoleSummeryRepository
    {
        public ProjectRoleSummeryRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
}
