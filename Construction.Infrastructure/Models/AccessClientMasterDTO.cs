using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Construction.Infrastructure.KeyValues;


namespace Construction.Infrastructure.Models
{
    public class AccessClientMasterDTO
    {
        public int AccessId { get; set; }      
        public int? ModuleId { get; set; }
        public bool? IsClient { get; set; }
        public string? ModuleIds { get; set; }
        public string? AccessIds { get; set; }
        public string? IsDeletes { get; set; }
        public string? IsAdds { get; set; }
        public string? IsEdits { get; set; }
        public string? IsApprovals { get; set; }
        public string? IsViews { get; set; }
        public string? ModuleCode { get; set; }
        public string? ModuleName { get; set; }
        public int? UserId { get; set; }      
        public int? UnitId { get; set; }
        public bool? IsView { get; set; }
        public bool? IsDelete { get; set; }
        public bool? IsAdd { get; set; }
        public bool? IsEdit { get; set; }
        public bool? IsApproval { get; set; }
        public bool? IsActive { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;      
        public int? HttpStatusCode { get; set; } = 200;
        public List<AccessClientMasterDTO>? AccessMasterList { get; set; }
       

    }
}
