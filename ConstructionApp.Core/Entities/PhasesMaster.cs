using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("PhasesMaster")]
    public partial class PhasesMaster
    {
        [Key]
        public int PhaseID { get; set; }
        public string? Phases { get; set; }
        public bool? IsActive { get; set; }
       


    }
}
