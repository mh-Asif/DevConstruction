using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;


namespace ConstructionApp.Services.Repository
{
    public class SubTaskDashboardRepository : DapperGenericRepository<SubTaskDashboard>, ISubTaskDashboardRepository
    {
        public SubTaskDashboardRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
}
