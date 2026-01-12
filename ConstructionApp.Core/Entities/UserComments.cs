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
    [Table("UserComments")]
    public partial class UserComments
    {
        [Key]
        public int ID { get; set; }
        public string? Comments { get; set; }
        public string? Summary { get; set; }
        public int? SubTaskId { get; set; }
        public bool? IsActive { get; set; }       
        public int? UserId { get; set; }
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }
        public DateTime? CreationDate { get; set; }

        //[NavigationProperty]
        //public virtual UsersMaster Profile { get; set; } = null!;

    }
}
