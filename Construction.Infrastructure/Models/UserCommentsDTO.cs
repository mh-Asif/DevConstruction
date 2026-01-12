using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public partial class UserCommentsDTO
    {
         public int ID { get; set; }
        public string? Comments { get; set; }
        public string? UserName { get; set; }
        public bool? IsActive { get; set; }
        public int? UserId { get; set; }
        public string? Descriptions { get; set; }
        public string? Summary { get; set; }
        public int? SubTaskId { get; set; }
        public DateTime? CreationDate { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public string? Base64ProfileImage { get; set; }
        public int? HttpStatusCode { get; set; } = 200;
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }

        //[ValidateNever]
        //public UsersMasterDTO Profile { get; set; }


    }
}
