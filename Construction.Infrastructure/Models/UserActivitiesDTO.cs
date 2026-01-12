using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public partial class UserActivitiesDTO
    {
        public int Id { get; set; }
        public string? Category { get; set; }
        public string? Summary { get; set; }
        public string? Description { get; set; }
        public bool? IsActive { get; set; }
        public int? UserId { get; set; }
        public DateTime? CreationDate { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;


    }
}
