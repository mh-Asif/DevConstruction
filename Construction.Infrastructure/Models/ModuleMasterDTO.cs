using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Construction.Infrastructure.KeyValues;


namespace Construction.Infrastructure.Models
{
    public class ModuleMasterDTO
    {
        public int ModuleId { get; set; }
        public string? ModuleName { get; set; }
        public bool? IsActive { get; set; }
        public string? ModuleCode { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;      
        public int? HttpStatusCode { get; set; } = 200;

    }
}
