using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public partial class PriorityMasterDTO
    {
        public int PriorityId { get; set; }
        public string EncPriorityId { get; set; }
        public string? Priority { get; set; }
        public bool? IsActive { get; set; }       
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;
        public List<PriorityMasterDTO>? PriorityMasterList { get; set; }
    }
}
