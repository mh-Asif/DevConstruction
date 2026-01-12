using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
    public partial class ExternalUsers
    {
        public int Id { get; set; }
        public string? ShareIds { get; set; }
        public int? TableId { get; set; }
        public string? EmailId { get; set; }
        public string? UniqueId { get; set; }
        public bool? IsActive { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }
    }
}
