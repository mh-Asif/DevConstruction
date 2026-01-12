using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public partial class JobTitleMasterDTO
    {
        public int JobTitleId { get; set; }
        public string EncryptedJobTitleId { get; set; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter job title"), MaxLength(50, ErrorMessage = "Job Title cannot exceed 50 characters")]
        public string? JobTitle { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public List<JobTitleMasterDTO>? JobTitleMasterList { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;

        public int? HttpStatusCode { get; set; } = 200;
        public int? UnitId { get; set; }


    }
}
