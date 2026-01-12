using ConstructionApp.Core.Entities;


namespace ConstructionApp.Core.Repository
{
    public interface IModuleMasterRepository : IGenericRepository<ModuleMaster>
    {
    }

    public interface IAccessMasterRepository : IDapperRepository<AccessMaster>
    {
    }
    public interface IAccessClientMasterRepository : IDapperRepository<AccessClientMaster>
    {
    }
}
