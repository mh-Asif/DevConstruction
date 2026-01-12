


namespace Construction.Infrastructure.Models
{
    public class StockOutTransactionDTO
    {
        public int Id { get; set; }
        public int ItemId { get; set; }
        public int? Quantity { get; set; }
        public int? vendorId { get; set; }
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }
        public DateTime? TransactionId { get; set; }
        public bool? IsActive { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;      
        public int? HttpStatusCode { get; set; } = 200;
        public List<StockOutTransactionDTO>? StockOutList { get; set; }
      

    }
}
