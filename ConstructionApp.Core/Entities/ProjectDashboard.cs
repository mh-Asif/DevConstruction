using ConstructionApp.Core.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
 
    public partial class ProjectDashboard
    {
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? ClientName { get; set; }
        public string? Status { get; set; }
        public string? ProjectOwner { get; set; }
        public int? Duration { get; set; }
        public int? TaskCount { get; set; }
        public int? Phases { get; set; }
        public string? Email { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedOn { get; set; }
        public string? Description { get; set; }

       // [MaxLength]
        public string? OwnerProfile { get; set; }

        //[MaxLength]
        public string? UserProfile { get; set; }      
        public string? Category { get; set; }
        public string? Priority { get; set; }

        public int? StatusId { get; set; }
        public int? PriorityId { get; set; }



    }
}
