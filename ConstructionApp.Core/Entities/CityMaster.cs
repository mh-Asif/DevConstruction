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
    [Table("CityMaster")]
    public partial class CityMaster
    {
        [Key]
        public int CityId { get; set; }

        public int CountryId { get; set; }

        public int StateId { get; set; }

        [StringLength(50)]
        public string CityName { get; set; } = null!;

        public bool? IsActive { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? ModifedBy { get; set; }

        [Column("modifiedOn", TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }

        //  [ForeignKey("CountryId")]
        //  [InverseProperty("CityMasters")]

        [NavigationProperty]
        public virtual CountryMaster Country { get; set; } = null!;

        //   [ForeignKey("StateId")]
        //   [InverseProperty("CityMasters")]
        [NavigationProperty]
        public virtual StateMaster State { get; set; } = null!;
    }
}
