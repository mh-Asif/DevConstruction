using Construction.Infrastructure.KeyValues;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public partial class CommentsDashboardDTO    {
        public int ID { get; set; }
        public string? Comments { get; set; }
        public string? ProjectName { get; set; }
        public string? TaskName { get; set; }
        public string? Times { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsComment { get; set; }
        public int? UserId { get; set; }
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }

        public string? eTaskId { get; set; }
        public int? SubTaskId { get; set; }
        public string? CreationDate { get; set; }
        public string? FullName { get; set; }
        public string? Base64ProfileImage { get; set; }
        public string? ProfileName { get; set; }
        [MaxLength]
        public byte[]? ProfileImage { get; set; }

        public List<CommentsDashboardDTO>? CommentList { get; set; }


    }
}
