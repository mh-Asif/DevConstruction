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
    [Table("DocumentCategory")]
    public partial class DocumentCategory
    {
        [Key]
        public int Id { get; set; }       
        public string? Category { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDisable { get; set; }

    }


    [Table("DrawingCategory")]
    public partial class DrawingCategory
    {
        [Key]
        public int Id { get; set; }
        public string? Category { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDisable { get; set; }

    }

    [Table("PhotoCategory")]
    public partial class PhotoCategory
    {
        [Key]
        public int Id { get; set; }
        public string? Category { get; set; }
        public DateTime? CreationDate { get; set; }
        public bool? IsActive { get; set; }
        public bool IsDisable { get; set; }

    }
}
