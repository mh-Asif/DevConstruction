using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public class UnitCategoryMasterDTO
    {
        public int UnitCategoryId { get; set; }
        public int? MasterCategoryId { get; set; }
        public string? EncryptedId { get; set; }
        public int? UnitId { get; set; }
        public string? UnitCategory { get; set; }
        public bool? IsActive { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;
        public List<UnitCategoryMasterDTO>? UnitCategoryList { get; set; }
        public List<MasterCategoryDTO>? MasterCategoryList { get; set; }
    }
}
