

namespace Construction.Infrastructure.Models
{
    public class StockTransactionsDTO
    {
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
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;      
        public int? HttpStatusCode { get; set; } = 200;
        public List<StockTransactionsDTO>? StockTransactionsList { get; set; }
        
      

    }
}
