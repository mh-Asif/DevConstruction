using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("RoleMaster")]
    public partial class RoleMaster
    {
        [Key]
        public int RoleId { get; set; }

       
        [StringLength(100)]
        public string? RoleName { get; set; }

        public int? UnitId { get; set; }
        public bool? IsActive { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? modifedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? modifiedOn { get; set; }
       // public virtual ICollection<RoleMenuMapping> RoleMenuMappings { get; set; } = new List<RoleMenuMapping>();
    }

}
