using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public class UnitPhasesMasterDTO
    {
        public int ID { get; set; }
        public int? UnitId { get; set; }
        public string EncryptedId { get; set; }
        public int? MasterPhaseId { get; set; }
        public string? Phase { get; set; }
        public bool? IsActive { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;
        public List<PhasesMasterDTO>? MasterPhasesList { get; set; }
        public List<UnitPhasesMasterDTO>? UnitPhasesList { get; set; }
    }
}
