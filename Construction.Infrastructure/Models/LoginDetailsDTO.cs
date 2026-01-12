using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public class LoginDetailsDTO
    {
        public int LoginId { get; set; }
        public string? LoginUser { get; set; }
        public string? MobileNo { get; set; }
        public string? LoginPassword { get; set; }
        public int? UserId { get; set; }
        public int? ClientId { get; set; }
        public int? UnitId { get; set; }
        public bool? IsActive { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public int? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;
    }
}
