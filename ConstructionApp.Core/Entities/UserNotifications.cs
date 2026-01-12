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
  
    public partial class UserNotifications
    {
         public int ID { get; set; }
        public int UserId { get; set; }
        public int? CollaboratorId { get; set; }
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }
        public int? SubTaskId { get; set; }
        public string? Heading { get; set; }
        public string? NotifyMessage { get; set; }
        public string? FullName { get; set; }
        public string? ProjectName { get; set; }
        public string? TaskName { get; set; }
        public string? ProfileName { get; set; }
        [MaxLength]
        public byte[]? ProfileImage { get; set; }
        public bool? NotifyStatus { get; set; }
        public DateTime? CreationDate { get; set; }       
       
      
    }
}
