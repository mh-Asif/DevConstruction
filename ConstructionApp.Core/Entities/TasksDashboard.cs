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
    public partial class TasksDashboard
    {
     
        public int ID { get; set; }
        public int? TotalTask { get; set; }
        public int? TotalProject { get; set; }
        public int? TotalClient { get; set; }
        public int? TotalVendor { get; set; }
        public int? OpenTask { get; set; }
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
}
