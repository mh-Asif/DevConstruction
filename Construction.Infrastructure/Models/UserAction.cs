using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public partial class UserAction
    {
        public string? PriorityIds { get; set; }
        public string? Priority { get; set; }

      
    }
    public partial class DashboardAction
    {
        public string? UserId { get; set; }
        public string? UnitId { get; set; }


    }
}
