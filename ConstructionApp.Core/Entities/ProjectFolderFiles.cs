using ConstructionApp.Core.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
   [Table("ProjectFolderFiles")]
    public partial class ProjectFolderFiles
    {
         [Key]
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public int? UserId { get; set; }    
        public int? CategoryId { get; set; }
        public int? FolderId { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public bool? IsActive { get; set; }
        public string? AccessIds { get; set; }
        public int? AccessType { get; set; }
        public string? EmailIds { get; set; }
        public string? OptMessage { get; set; }
        public DateTime? CreationDate { get; set; }
    }


    [Table("DrawingFolderFiles")]
    public class DrawingFolderFiles
    {
        [Key]
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public int? FolderId { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public bool? IsActive { get; set; }
        public string? AccessIds { get; set; }
        public int? AccessType { get; set; }
        public string? EmailIds { get; set; }
        public string? OptMessage { get; set; }
        public DateTime? CreationDate { get; set; }
    }

    [Table("ProjectFilePhotos")]
    public class ProjectFilePhotos
    {
        [Key]
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public int? UserId { get; set; }
        public int? CategoryId { get; set; }
        public int? FolderId { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public bool? IsActive { get; set; }
        public string? AccessIds { get; set; }
        public int? AccessType { get; set; }
        public string? EmailIds { get; set; }
        public string? OptMessage { get; set; }
        public DateTime? CreationDate { get; set; }
    }
}
