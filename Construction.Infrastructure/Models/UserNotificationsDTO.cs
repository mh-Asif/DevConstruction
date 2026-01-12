using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public class UserNotificationsDTO
    {
        public int ID { get; set; }
        public int? UserId { get; set; }
        public int? CollaboratorId { get; set; }
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }
        public int? SubTaskId { get; set; }
        public string? Heading { get; set; }
        public string? NotifyMessage { get; set; }
        public string? FullName { get; set; }
        public string? ProfileName { get; set; }
        public string? ProjectName { get; set; }
        public int? UserType { get; set; }
        public string? TaskName { get; set; }
        [MaxLength]
        public byte[]? ProfileImage { get; set; }
        public string Base64ProfileImage { get; set; }
      //  public string? ProfileName { get; set; }
        public bool? NotifyStatus { get; set; }
        public DateTime? CreationDate { get; set; }
        public List<AccessMasterDTO>? AccessList { get; set; }
        public List<UserNotificationsDTO>? NotificationList { get; set; }
    }
}
