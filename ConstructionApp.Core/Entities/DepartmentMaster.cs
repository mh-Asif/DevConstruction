using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("DepartmentMaster")]
    public partial class DepartmentMaster
    {
        [Key]
        public int DepartmentId { get; set; }

        [StringLength(10)]
        public string? DepartmentCode { get; set; }

        [StringLength(100)]
        public string? DepartmentName { get; set; }

        public int? UnitId { get; set; }
        public bool? IsActive { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? ModifedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }
       // public virtual ICollection<RoleMenuMapping> RoleMenuMappings { get; set; } = new List<RoleMenuMapping>();
    }

}
