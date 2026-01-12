using Construction.Infrastructure.Models;
using ConstructionApp.Core.Entities;
using System.Linq.Expressions;


namespace ConstructionApp.Core.Repository
{
    public interface IUsersMasterRepository : IGenericRepository<UsersMaster>
    {
       // public Task<IList<UsersMasterDTO>> GetUserListing(Expression<Func<UsersMaster, bool>> expression);
    }
}
