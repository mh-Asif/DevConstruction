using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Services.Repository
{
    public class AccessMasterRepository : DapperGenericRepository<AccessMaster>, IAccessMasterRepository
    {
        public AccessMasterRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
    public class ModuleMasterRepository : GenericRepository<ModuleMaster>, IModuleMasterRepository
    {
        public ModuleMasterRepository(ConstDbContext context) : base(context)
        {

        }
    }
    public class AccessClientMasterRepository : DapperGenericRepository<AccessClientMaster>, IAccessClientMasterRepository
    {
        public AccessClientMasterRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
}
