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
   [Table("ProjectFolder")]
    public partial class ProjectFolder
    {
         [Key]
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public int? UserId { get; set; }
        public bool? IsFolder { get; set; }
        public int? CategoryId { get; set; }
        public string? FolderName { get; set; }
        public string? FilePath { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? AccessIds { get; set; }
        public string? ClientIds { get; set; }
        public int? AccessType { get; set; }
        public string? EmailIds { get; set; }
        public string? OptMessage { get; set; }
    }

    [Table("ProjectDrawings")]
    public partial class ProjectDrawings
    {
        [Key]
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public int? UserId { get; set; }
        public bool? IsFolder { get; set; }
        public int? CategoryId { get; set; }
        public string? FolderName { get; set; }
        public string? FilePath { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? AccessIds { get; set; }
        public string? ClientIds { get; set; }
        public int? AccessType { get; set; }
        public string? EmailIds { get; set; }
        public string? OptMessage { get; set; }
    }

    [Table("ProjectPhotos")]
    public partial class ProjectPhotos
    {
        [Key]
        public int Id { get; set; }
        public int? ProjectId { get; set; }
        public int? UserId { get; set; }
        public bool? IsFolder { get; set; }
        public int? CategoryId { get; set; }
        public string? FolderName { get; set; }
        public string? FilePath { get; set; }
        public bool? IsActive { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? AccessIds { get; set; }
        public string? ClientIds { get; set; }
        public int? AccessType { get; set; }
        public string? EmailIds { get; set; }
        public string? OptMessage { get; set; }
    }
}
