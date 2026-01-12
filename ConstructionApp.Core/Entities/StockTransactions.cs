
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;


namespace ConstructionApp.Core.Entities
{
    [Table("StockTransactions")]
    public partial class StockTransactions
    {
       [Key]
        public int TransactionId { get; set; }
        public int? ItemId { get; set; }
        public int? VendorId { get; set; }
        public string? TransactionType { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? UnitCost { get; set; }
        public decimal? TotalCost { get; set; }
        public DateTime? TransactionDate { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
       
     
    }
}
