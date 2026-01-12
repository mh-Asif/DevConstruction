using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public class SubTaskDashboardDTO
    {
        public int SubTaskId { get; set; }
        public string? eSubTaskId { get; set; }
        public string? SubTaskName { get; set; }
        public string? TaskName { get; set; }
        public string? Status { get; set; }
        public string? TaskOwner { get; set; }
        public int? Duration { get; set; }
        public string? Priority { get; set; }
        public string? StartDate { get; set; }
        public string? EndDate { get; set; }
        public string? Description { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public string? CreatedBy { get; set; }
        public string? CreatedOn { get; set; }
      
    }
}
