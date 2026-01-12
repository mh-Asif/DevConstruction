using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("JobTitleMaster")]
    public partial class JobTitleMaster
    {
        [Key]
        public int JobTitleId { get; set; }

        [StringLength(50)]
        public string? JobTitle { get; set; }

        public bool? IsActive { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? ModifiedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
        public int? UnitId { get; set; }

       // public virtual ICollection<RoleMenuMapping> RoleMenuMappings { get; set; } = new List<RoleMenuMapping>();
    }

}
