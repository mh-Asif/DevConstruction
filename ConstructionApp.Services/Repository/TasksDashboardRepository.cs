using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;


namespace ConstructionApp.Services.Repository
{
    public class TasksDashboardRepository : DapperGenericRepository<TasksDashboard>, ITasksDashboardRepository
    {
        public TasksDashboardRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
}
