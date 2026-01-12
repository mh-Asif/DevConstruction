using Construction.Infrastructure.Models;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("ProjectSubTasks")]
    public partial class ProjectSubTasks
    {
        [Key]
        public int SubTaskId { get; set; }
        public int? ProjectId { get; set; }       
        public int? TaskId { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public string? Collaborators { get; set; }
        public string? SubTaskName { get; set; }
        public int? UnitId { get; set; }       
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }       
        public int? OwnerId { get; set; }      
        public bool? IsActive { get; set; }
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

    }
}
