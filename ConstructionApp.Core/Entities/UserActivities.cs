using ConstructionApp.Core.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("UserActivities")]
    public partial class UserActivities
    {
        [Key]
        public int Id { get; set; }
        public string? Category { get; set; }
        public string? Summary { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }       
        public int? UserId { get; set; }
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }

        public DateTime? CreationDate { get; set; }

    }
}
