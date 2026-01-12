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
    public class UserNotificationsRepository : DapperGenericRepository<UserNotifications>, IUserNotificationsRepository
    {
        public UserNotificationsRepository(DapperDBContext dapperDBContext) : base(dapperDBContext)
        {

        }
    }
}
