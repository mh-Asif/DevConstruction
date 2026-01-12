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
    [Table("Invoices")]
    public partial class Invoices
    {
        [Key]
        public int invoice_id { get; set; }
        public int vendor_id { get; set; }
        public DateTime? created_date { get; set; }
        public decimal? amount { get; set; }       
        public int? status { get; set; }
        public decimal? tax_value { get; set; }
        public int? tax_type { get; set; }
        public int? created_at { get; set; }
        public DateTime? Invoice_date { get; set; }
        public string? Description { get; set; }
        public string? Invoice_number { get; set; }

    }
}
