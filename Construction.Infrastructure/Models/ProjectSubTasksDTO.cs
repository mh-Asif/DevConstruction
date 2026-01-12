using Construction.Infrastructure.KeyValues;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public class ProjectSubTasksDTO
    {
        public int SubTaskId { get; set; }
        public int? ProjectId { get; set; }
        public int? TaskId { get; set; }
        public int? UserType { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public string? Collaborators { get; set; }

        public string? SubCollaborators { get; set; }
        public string? SubTaskName { get; set; }      
        public int? UnitId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }

        public DateTime? SubTaskStartDate { get; set; }
        public DateTime? SubTaskEndDate { get; set; }
        public int? OwnerId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsCreated { get; set; }
        public string? Description { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? EncryptedId { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string? DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;
        public ProjectSubTasksDTO? objSubTask { get; set; }
        public List<SubTaskDashboardDTO>? SubTaskDashboard { get; set; }
        //public List<UnitPriorityKeyValues>? PriorityList { get; set; }
        //public List<UserKeyValues>? UserList { get; set; }            
        //public List<UnitStatusKeyValues>? StatusList { get; set; }
        //public List<ProjectKeyValues>? ProjectList { get; set; }
        //public List<TaskDashboardDTO>? ProjectTaskList { get; set; }
        //public List<AccessMasterDTO>? AccessList { get; set; }
        //public List<CommentsDashboardDTO>? ActivityList { get; set; }
        //public List<ProjectRoleSummeryDTO>? TaskRoleSummeryList { get; set; }

    }
}
