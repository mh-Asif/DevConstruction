using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("PriorityMaster")]
    public partial class PriorityMaster
    {
        [Key]
        public int PriorityId { get; set; }
        public string? Priority { get; set; }
        public bool? IsActive { get; set; }
      


    }
}
