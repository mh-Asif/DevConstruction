using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Construction.Infrastructure.KeyValues;


namespace Construction.Infrastructure.Models
{
    public class ExternalUsersDTO
    {
        public int Id { get; set; }
        public string? ShareIds { get; set; }
        public int? TableId { get; set; }
        public string? emailPath { get; set; }
        public string? EmailId { get; set; }
        public string? OptMessage { get; set; }
        public string? UniqueId { get; set; }
        public bool? IsActive { get; set; }
        public string? CreatedBy { get; set; }      
        public DateTime? CreatedOn { get; set; }

    }
}
