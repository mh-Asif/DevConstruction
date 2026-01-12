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
    public partial class ProjectsDashboard
    {
     
        public int ID { get; set; }
        public int? TotalP { get; set; }
        public int? ToBeStarted { get; set; }
        public int? InProgress { get; set; }
        public int? Overdue { get; set; }       
        public int? Completed { get; set; }
        public int? Approved { get; set; }
        public decimal? OpenPrc { get; set; }
        public decimal? InProgressPrc { get; set; }
        public decimal? CompletePrc { get; set; }
        public decimal? OverduePrc { get; set; }
       public decimal? ApprovedPrc { get; set; }


    }
}
