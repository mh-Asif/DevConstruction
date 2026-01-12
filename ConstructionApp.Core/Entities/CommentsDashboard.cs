using ConstructionApp.Core.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Construction.Infrastructure.Models;

namespace ConstructionApp.Core.Entities
{
   // [Table("UserComments")]
    public partial class CommentsDashboard
    {
     
        public int ID { get; set; }
        public string? Comments { get; set; }
        public string? ProjectName { get; set; }
        public string? TaskName { get; set; }
        public string? Times { get; set; }
        public bool? IsActive { get; set; }       
        public int? UserId { get; set; }
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }
        public string? CreationDate { get; set; }
        public string? FullName { get; set; }
        public string? Base64ProfileImage { get; set; }
        public string? ProfileName { get; set; }
        [MaxLength]
        public byte[]? ProfileImage { get; set; }


    }
}
