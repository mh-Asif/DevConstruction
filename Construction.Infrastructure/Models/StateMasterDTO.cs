using Construction.Infrastructure.KeyValues;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public class StateMasterDTO
    {
        [ScaffoldColumn(false)]
        public int StateId { get; set; }

        public string EncryptedStateId { get; set; }

        [Range(1, int.MaxValue, ErrorMessage = "Please select country")]
        public int CountryId { get; set; }
        [ValidateNever]
        public string? CountryName { get; set; }

        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter state"), MaxLength(100, ErrorMessage = "State cannot exceed 100 characters")]
        public string StateName { get; set; } = null!;
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? ModifiedBy { get; set; }
        public DateTime? ModifiedOn { get; set; }

        [ValidateNever]
        public CountryMasterDTO Country { get; set; }
        //[ValidateNever]
        //public CountryMasterDTO CountryMaster { get; set; }
        //public bool IsIncludes { get; set; } = false;
        public HttpResponseMessage? HttpMessage { get; set; }
        public List<StateMasterDTO>? StateMasterList { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public List<CountryKeyValues>? CountryList { get; set; }
        public int? HttpStatusCode { get; set; } = 200;


    }
}
