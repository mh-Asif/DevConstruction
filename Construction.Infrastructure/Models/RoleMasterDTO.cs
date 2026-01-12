using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public partial class RoleMasterDTO
    {
        public int RoleId { get; set; }
        public string EncryptedRoleId { get; set; }        
        public string? RoleName { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? modifedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public List<RoleMasterDTO>? RoleMasterList { get; set; }
        public int? HttpStatusCode { get; set; } = 200;
        public int? UnitId { get; set; }
    }
}
