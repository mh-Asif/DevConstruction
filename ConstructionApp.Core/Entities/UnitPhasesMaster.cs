using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("UnitPhasesMaster")]
    public partial class UnitPhasesMaster
    {
        [Key]
        public int ID { get; set; }
        public int? UnitId { get; set; }
        public int? MasterPhaseId { get; set; }
        public string? Phase { get; set; }
        public bool? IsActive { get; set; }
      


    }
}
