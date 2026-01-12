using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public partial class PhasesMasterDTO
    {
        public int PhaseID { get; set; }
        public string? Phases { get; set; }
        public string EncryptedId { get; set; }
        public bool? IsActive { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;
        public List<PhasesMasterDTO>? PhasesList { get; set; }
    }
}
