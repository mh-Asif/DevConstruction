using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public class CompanyPriorityMasterDTO
    {
        public int CompanyPriorityId { get; set; }
        public string? EncCompanyPriorityId { get; set; }
        public int? CompanyId { get; set; }
        public string? CompanyPriority { get; set; }
        public int? UnitPriorityId { get; set; }
        public bool? IsActive { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string? DisplayMessage { get; set; } = string.Empty;
        public List<CompanyPriorityMasterDTO>? CompanyPriorityList { get; set; }
        public List<PriorityMasterDTO>? PriorityMasterList { get; set; }
        public int? HttpStatusCode { get; set; } = 200;
    }
}
