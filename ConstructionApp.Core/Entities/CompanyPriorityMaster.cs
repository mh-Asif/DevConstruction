using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("CompanyPriorityMaster")]
    public partial class CompanyPriorityMaster
    {
        [Key]
        public int CompanyPriorityId { get; set; }
        public int? CompanyId { get; set; }
        public int? UnitPriorityId { get; set; }
        public string? CompanyPriority { get; set; }
        public bool? IsActive { get; set; }
      


    }
}
