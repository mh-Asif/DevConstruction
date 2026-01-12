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
    [Table("Approvals")]
    public partial class Approvals
    {
        [Key]
        public int approval_id { get; set; }
        public int invoice_id { get; set; }
        public int? approver_id { get; set; }       
        public int? status { get; set; }
        public DateTime? approval_date { get; set; }
    }
}
