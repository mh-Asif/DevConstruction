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
    public class UnitCategoryMasterRepository : GenericRepository<UnitCategoryMaster>, IUnitCategoryMasterRepository
    {
        public UnitCategoryMasterRepository(ConstDbContext context) : base(context)
        {

        }
    }
}
