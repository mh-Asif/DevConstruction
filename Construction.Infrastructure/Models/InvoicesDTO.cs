

namespace Construction.Infrastructure.Models
{
    public class InvoicesDTO
    {
        public int invoice_id { get; set; }
        public int vendor_id { get; set; }
        public string? vendor_name { get; set; }
        public decimal? amount { get; set; }
        public int? status { get; set; }
        public decimal? tax_value { get; set; }
        public int? tax_type { get; set; }
        public DateTime? Invoice_date { get; set; }
        public string? Description { get; set; }
        public string? Invoice_number { get; set; }
        public int? created_at { get; set; }
        public DateTime? created_date { get; set; }      
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;      
        public int? HttpStatusCode { get; set; } = 200;
        public List<InvoicesDTO>? InvoicesList { get; set; }
        
        // Vendor details
        public string? companyName { get; set; }
        public string? businessEmail { get; set; }
        public string? businessContactNumber { get; set; }
        public string? address { get; set; }
        public int? cityId { get; set; }
        public int? stateId { get; set; }
        public string? gstn { get; set; }
        public string? bankName { get; set; }
        public string? accountName { get; set; }
        public string? ifscOrShiftCode { get; set; }
        public string? accountNumber { get; set; }
        
        // Company/Organization details (from current user/session)
        public string? fromCompanyName { get; set; }
        public string? fromAddress { get; set; }
        public string? fromCity { get; set; }
        public string? fromState { get; set; }
        public string? fromPostalOrZipCode { get; set; }

    }
}
