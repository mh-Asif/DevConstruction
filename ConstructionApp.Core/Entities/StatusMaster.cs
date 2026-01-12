using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    [Table("StatusMaster")]

    public partial class StatusMaster
    {
        [Key]
        public int StatusId { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
        
    }
}
