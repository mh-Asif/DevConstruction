using ConstructionApp.Core.Helper;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Entities
{
 
    public partial class ProjectSummery
    {
        public int TotalTask { get; set; }
        public int? OpenTask { get; set; }
        public string? ProjectDescription { get; set; }
        public int? InProgressTask { get; set; }
        public int? CompletedTask { get; set; }
        public int? OverdueTask { get; set; }
        public int? ApprovedTask { get; set; }
        public decimal? OpenPrc { get; set; }
        public decimal? InProgressPrc { get; set; }
        public decimal? CompletePrc { get; set; }
        public decimal? OverduePrc { get; set; }
        public decimal? ApprovedPrc { get; set; }



    }

    public partial class ProjectRoleSummery
    {
        //[MaxLength]
        //public byte[]? ProfilePic { get; set; }
        public string? ProfilePic { get; set; }
        public string? UserName { get; set; }
        public string? UserId { get; set; }
        public string? email { get; set; }
        public string? Designation { get; set; }
        public int? IsProjectOwner { get; set; }     



    }
}
