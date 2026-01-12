using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("UnitStatusMaster")]
    public partial class UnitStatusMaster
    {
        [Key]
        public int ID { get; set; }
        public int? UnitId { get; set; }
        public int? MasterStatusId { get; set; }
        public string? Status { get; set; }
        public string? StatusType { get; set; }
        public bool? IsActive { get; set; }
      


    }
}
