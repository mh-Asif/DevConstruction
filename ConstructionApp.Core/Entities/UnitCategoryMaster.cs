using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("UnitCategoryMaster")]
    public partial class UnitCategoryMaster
    {
        [Key]
        public int UnitCategoryId { get; set; }
        public int? MasterCategoryId { get; set; }
        public int? UnitId { get; set; }
        public string? UnitCategory { get; set; }
        public bool? IsActive { get; set; }
      


    }
}
