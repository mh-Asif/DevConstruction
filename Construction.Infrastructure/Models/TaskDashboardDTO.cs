using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public class TaskDashboardDTO
    {
        public int TaskId { get; set; }
        public string? eTaskId { get; set; }
        public int ProjectId { get; set; }
        public string? TaskName { get; set; }
        public string? Status { get; set; }
        public string? TaskOwner { get; set; }
        public string? Description { get; set; }
        public string? Phase { get; set; }
        public int? Duration { get; set; }
        public string? Priority { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public int? PhaseId { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedOn { get; set; }
        public string? Vendor { get; set; }
        public string? OwnerProfile { get; set; }
        public string? UserProfile { get; set; }
    }
}
