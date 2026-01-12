using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public class ProjectDashboardDTO
    {
        public int ProjectId { get; set; }
        public string? ProjectName { get; set; }
        public string? ClientName { get; set; }
        public string? Status { get; set; }
        public int? StatusId { get; set; }
        public int? PriorityId { get; set; }
        public string? ProjectOwner { get; set; }
        public int? TaskCount { get; set; }
        public int? Phases { get; set; }
        public int? Duration { get; set; }
        public string? StartDate { get; set; }
        public string? Description { get; set; }
        public string? Email { get; set; }
        public string? EndDate { get; set; }
        public string? EncProjectId { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedOn { get; set; }
       // [MaxLength]
        public string? OwnerProfile { get; set; }

       // [MaxLength]
        public string? UserProfile { get; set; }
        public string? PMProfile { get; set; }
        public string? UProfile { get; set; }
        public string? Category { get; set; }
        public string? Priority { get; set; }
       

    }
}
