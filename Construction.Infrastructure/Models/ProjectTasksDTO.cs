using Construction.Infrastructure.KeyValues;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public class ProjectTasksDTO
    {
        public int TaskId { get; set; }
        public int? ProjectId { get; set; }
        public string? EncProjectId { get; set; }
        public int? PhaseId { get; set; }
        public int? FPhaseId { get; set; }
        public int? PriorityId { get; set; }
        public int? FPriorityId { get; set; }
        public int? StatusId { get; set; }
        public int? FStatusId { get; set; }
        public int? Duration { get; set; }
        public string? Collaborators { get; set; }
        public string? TaskName { get; set; }        
        public int? UnitId { get; set; }
        public bool? IsVendor { get; set; }
        public bool IsCreated { get; set; }
        public int? VendorId { get; set; }
        public int? UserType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? TaskOwnerId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsView { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? Description { get; set; }
        public string? FileName { get; set; }
        public string? FilePath { get; set; }
        public string? FileExt { get; set; }
        public string? EncryptedId { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string? DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;
        public ProjectTasksDTO? objTask { get; set; }
        public List<UnitPriorityKeyValues>? PriorityList { get; set; }
      //  public List<UnitPriorityKeyValues>? PriorityListFilter { get; set; }
        public List<UserKeyValues>? UserList { get; set; }
        public List<UserKeyValues>? UserListForSubTask { get; set; }
        public List<UserKeyValues>? VendorList { get; set; }
        public List<ProjectTasksDTO>? TaskList { get; set; }
        public List<UnitPhaseKeyValues>? PhaseList { get; set; }
        public List<UnitStatusKeyValues>? StatusList { get; set; }

       // public List<UnitPhaseKeyValues>? PhaseListFilter { get; set; }
       // public List<UnitStatusKeyValues>? StatusList { get; set; }
        public List<ProjectKeyValues>? ProjectList { get; set; }
        public List<ProjectDashboardDTO>? ProjectsList { get; set; }
        public List<TaskDashboardDTO>? ProjectTaskList { get; set; }
        public List<AccessMasterDTO>? AccessList { get; set; }
        public List<CommentsDashboardDTO>? ActivityList { get; set; }
        public List<ProjectRoleSummeryDTO>? TaskRoleSummeryList { get; set; }
       

        

    }


    public class TasksDTO
    {
        public int TaskId { get; set; }
        public int? ProjectId { get; set; }
        public int? PhaseId { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public int? Duration { get; set; }
        public string? Collaborators { get; set; }
        public string? TaskName { get; set; }
        public int? UnitId { get; set; }
        public bool? IsVendor { get; set; }
        public bool IsCreated { get; set; }
        public int? VendorId { get; set; }
        public int? UserType { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? TaskOwnerId { get; set; }
        public bool? IsActive { get; set; }
        public bool? IsView { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }
        public string? Description { get; set; }
        public string? EncryptedId { get; set; }
        public string? FileName { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string? DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;
       

    }
}
