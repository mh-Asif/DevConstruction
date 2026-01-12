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
    [Table("ModuleMaster")]
    public partial class ModuleMaster
    {
        [Key]
        public int ModuleId { get; set; }       
        public string? ModuleName { get; set; }
        public bool? IsActive { get; set; }
        public string? ModuleCode { get; set; }

    }
}
