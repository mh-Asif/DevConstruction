using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public partial class DepartmentMasterDTO
    {
        public int DepartmentId { get; set; }
        public string EncryptedDepartmentId { get; set; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter department code"), MaxLength(10, ErrorMessage = "Department Code cannot exceed 10 characters")]
        public string? DepartmentCode { get; set; }
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter department name"), MaxLength(100, ErrorMessage = "Department cannot exceed 100 characters")]
        public string? DepartmentName { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public List<DepartmentMasterDTO>? DepartmentMasterList { get; set; }
        public int? HttpStatusCode { get; set; } = 200;
        public int? UnitId { get; set; }
    }
}
