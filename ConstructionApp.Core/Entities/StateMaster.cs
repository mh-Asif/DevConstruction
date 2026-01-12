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
    [Table("StateMaster")]
    public partial class StateMaster
    {
        [Key]
        public int StateId { get; set; }

        public int CountryId { get; set; }

        [StringLength(100)]
        public string StateName { get; set; } = null!;

        public bool? IsActive { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? ModifiedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }

       
        [NavigationProperty]
        public virtual CountryMaster Country { get; set; } = null!;

    }
}
