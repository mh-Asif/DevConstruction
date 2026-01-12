using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Construction.Infrastructure.KeyValues;


namespace Construction.Infrastructure.Models
{
    public class ProjectFolderDTO
    {
        public int Id { get; set; }     
        
        public string? EnFolderId { get; set; }
        public int? ProjectId { get; set; }
        public int? UserId { get; set; }

        public string? OptMessage { get; set; }
        public string? UserName { get; set; }
        public string? Base64ProfileImage { get; set; }
        public int? CategoryId { get; set; }
        public string? FolderName { get; set; }        
        public bool? IsActive { get; set; }
        public bool? IsFolder { get; set; }
        public string? FilePath { get; set; }
        public string? FileExt { get; set; }
        public string? AccessIds { get; set; }
        public string? ClientIds { get; set; }
        public int? AccessType { get; set; }
        public string? EmailIds { get; set; }
        public int? UserType { get; set; }       
        public DateTime? CreationDate { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;      
        public int? HttpStatusCode { get; set; } = 200;
       

    }

    public class ProjectDrawingsDTO
    {
        public int Id { get; set; }

        public string? EnFolderId { get; set; }
        public int? ProjectId { get; set; }
        public int? UserId { get; set; }
        public int? UserType { get; set; }
        public string? UserName { get; set; }
        public string? Base64ProfileImage { get; set; }
        public int? CategoryId { get; set; }
        public string? FolderName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsFolder { get; set; }
        public string? FilePath { get; set; }
        public string? FileExt { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? AccessIds { get; set; }
        public string? ClientIds { get; set; }
        public int? AccessType { get; set; }
        public string? EmailIds { get; set; }
        public string? OptMessage { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;


    }


    public class ProjectPhotosDTO
    {
        public int Id { get; set; }

        public string? EnFolderId { get; set; }
        public string? Ids { get; set; }
        public int? ProjectId { get; set; }
        public int? UserId { get; set; }
        public int? UserType { get; set; }
        public string? UserName { get; set; }
        public string? Base64ProfileImage { get; set; }
        public int? CategoryId { get; set; }
        public string? FolderName { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsFolder { get; set; }
        public string? FilePath { get; set; }
        public string? FileExt { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? AccessIds { get; set; }
        public string? ClientIds { get; set; }
        public int? AccessType { get; set; }
        public string? EmailIds { get; set; }
        public string? OptMessage { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;


    }
}
