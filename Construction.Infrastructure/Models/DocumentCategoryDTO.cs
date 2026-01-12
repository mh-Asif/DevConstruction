using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Construction.Infrastructure.KeyValues;


namespace Construction.Infrastructure.Models
{
    public class DocumentCategoryDTO
    {
        public int Id { get; set; }
        public string? Category { get; set; }
        public string? EncCategoryId { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDisable { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;      
        public int? HttpStatusCode { get; set; } = 200;
        public List<DocumentCategoryDTO>? DocumentCategoryList { get; set; }
       

    }

    public class DrawingCategoryDTO
    {
        public int Id { get; set; }
        public string? Category { get; set; }
        public string? EncCategoryId { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDisable { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;
        public List<DrawingCategoryDTO>? DrawingCategoryList { get; set; }


    }

    public class PhotoCategoryDTO
    {
        public int Id { get; set; }
        public string? Category { get; set; }
        public string? EncCategoryId { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDisable { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;
        public List<PhotoCategoryDTO>? PhotoCategoryList { get; set; }


    }
}
