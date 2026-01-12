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
    [Table("Payments")]
    public partial class Payments
    {
        [Key]
        public int payment_id { get; set; }
        public int invoice_id { get; set; }
        public string? transaction_ref { get; set; }     
        public int? payment_by { get; set; }
        public DateTime? payment_date { get; set; }
    }
}
