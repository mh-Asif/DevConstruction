

namespace Construction.Infrastructure.Models
{
    public class PaymentsDTO
    {
        public int payment_id { get; set; }
        public int invoice_id { get; set; }
        public string? transaction_ref { get; set; }
        public int? payment_by { get; set; }
        public DateTime? payment_date { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;      
        public int? HttpStatusCode { get; set; } = 200;
        public List<PaymentsDTO>? PaymentList { get; set; }
      

    }
}
