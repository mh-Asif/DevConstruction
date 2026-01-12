using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public class UnitStatusMasterDTO
    { 
      
        public int ID { get; set; }
        public int? UnitId { get; set; }
        public string? EncryptedId { get; set; }
        public string? StatusType { get; set; }
        public int? MasterStatusId { get; set; }
        public string? Status { get; set; }
        public bool? IsActive { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;
        public List<UnitStatusMasterDTO>? UnitStatusMasterList { get; set; }
        public List<StatusMasterDTO>? StatusMasterList { get; set; }
    }
}
