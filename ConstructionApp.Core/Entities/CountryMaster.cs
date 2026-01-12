using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("CountryMaster")]
    public partial class CountryMaster
    {
        [Key]
        public int CountryId { get; set; }
        public string? CountryCode { get; set; }

        [StringLength(50)]
        public string CountryName { get; set; } = null!;

        public bool? IsActive { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? ModifedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }

        [InverseProperty("Country")]
        public virtual ICollection<CityMaster> CityMasters { get; set; } = new List<CityMaster>();

        //[InverseProperty("Country")]
       // public virtual ICollection<DistrictMaster> DistrictMasters { get; set; } = new List<DistrictMaster>();

        [InverseProperty("Country")]
        public virtual ICollection<StateMaster> StateMasters { get; set; } = new List<StateMaster>();
    }

}
