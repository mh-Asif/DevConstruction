


namespace Construction.Infrastructure.Models
{
    public class ApprovalsDTO
    {
        public int approval_id { get; set; }
        public int invoice_id { get; set; }
        public int? approver_id { get; set; }
        public int? status { get; set; }
        public DateTime? approval_date { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;      
        public int? HttpStatusCode { get; set; } = 200;
        public List<ApprovalsDTO>? ApprovalList { get; set; }
      

    }
}
