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
    [Table("Projects")]
    public partial class Projects
    {
        [Key]
        public int ProjectId { get; set; }
        public int? CategoryId { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public string? Collaborators { get; set; }
        public int? UnitId { get; set; }
        public int? ClientId { get; set; }        
        public string? FileDetails { get; set; }
        public string? ProjectName { get; set; }
        public string? Description { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }       
        public int? UserId { get; set; }      
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }

        //[NavigationProperty]
        //public virtual UsersMaster Users { get; set; } = null!;

        //public int? ModifiedBy { get; set; }
        //public DateTime? ModifiedOn { get; set; }
    }
}
