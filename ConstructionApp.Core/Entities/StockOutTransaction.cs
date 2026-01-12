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
    [Table("StockOutTransaction")]
    public partial class StockOutTransaction
    {
        [Key]
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int? Quantity { get; set; }  
        public int? vendorId { get; set; }
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }
        public DateTime? TransactionId { get; set; }
        public bool? IsActive { get; set; }
    }
}
