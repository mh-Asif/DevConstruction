using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public partial class PublicUsers
    {
        public int Id { get; set; }
        public int? dId { get; set; }
        public int? PId { get; set; }
        public int? CId { get; set; }
        public int? UserId { get; set; }
        public string? ProjectName { get; set; }
        public string? Category { get; set; }
        public string? FolderName { get; set; }
        public string? FileName { get; set; }
        public int? IsFolder { get; set; }
        public string? FilePath { get; set; }
        public int? FolderId { get; set; }
        public DateTime? CreationDate { get; set; }
        public string? UserName { get; set; }
        public string? ProfileImage { get; set; }
        public string? DocType { get; set; }

    }
}
