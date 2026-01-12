using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Construction.Infrastructure.KeyValues;


namespace Construction.Infrastructure.Models
{
    public class CityMasterDTO
    {

        [Column("CityID")]
        public int CityId { get; set; }

        public string EncryptedCityId { get; set; }

        [Column("CountryID")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select country")]
        public int CountryId { get; set; }
        public string? CountryName { get; set; }

        [Column("StateID")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select state")]
        public int StateId { get; set; }
        public string? StateName { get; set; }

        [StringLength(50)]
        [DataType(DataType.Text)]
        [Required(ErrorMessage = "Please enter city name"), MaxLength(50, ErrorMessage = "City cannot exceed 50 characters")]
        public string CityName { get; set; } = null!;

        public bool? IsActive { get; set; }

        [StringLength(50)]
        public string? CreatedBy { get; set; }

        [Column(TypeName = "datetime")]
        public DateTime? CreatedOn { get; set; }

        [StringLength(50)]
        public string? ModifiedBy { get; set; }

        [Column("modifiedOn", TypeName = "datetime")]
        public DateTime? ModifiedOn { get; set; }

        [ValidateNever]
        public CountryMasterDTO Country { get; set; } = null!;
        [ValidateNever]
        public StateMasterDTO State { get; set; } = null!;

        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
       // [ValidateNever]
        public List<CityMasterDTO>? CityMasterList { get; set; }

        public List<CountryKeyValues>? CountryList { get; set; }
        //public List<StateKeyValues>? StateList { get; set; }
        public int? HttpStatusCode { get; set; } = 200;

    }
}
