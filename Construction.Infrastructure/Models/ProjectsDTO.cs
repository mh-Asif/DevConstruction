using Construction.Infrastructure.KeyValues;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Construction.Infrastructure.Models
{
    public class ProjectsDTO
    {
        public int ProjectId { get; set; }
        public int? CategoryId { get; set; }
        public int? PriorityId { get; set; }
        public int? StatusId { get; set; }
        public string? Collaborators { get; set; }
        public string? EncProjectId { get; set; }
        public int? UnitId { get; set; }
        public int? FolderId { get; set; }
        public int? ClientId { get; set; }
        public string? FileDetails { get; set; }
        public string? TagifyUserList { get; set; }
        public string? ProjectName { get; set; }
        public string? CategoryName { get; set; }
        public string? Description { get; set; }
        public string? Status { get; set; }
        public string? ProjectOwner { get; set; }
        public int? Tasks { get; set; }
        public int? Phases { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? Duration { get; set; }
        public int? UserId { get; set; }
        public int? UserType { get; set; }
        public bool? IsActive { get; set; }
        public bool IsCreated { get; set; }
        public int? CreatedBy { get; set; }
        public DateTime? CreatedOn { get; set; }       
        public string EncryptedId { get; set; }
        public HttpResponseMessage? HttpMessage { get; set; }
        public string DisplayMessage { get; set; } = string.Empty;
        public int? HttpStatusCode { get; set; } = 200;
        public List<UnitPriorityKeyValues>? PriorityList { get; set; }
        public List<UserKeyValues>? UserList { get; set; }
        public List<UserKeyValues>? ClientList { get; set; }
        public List<UserKeyValues>? VendorList { get; set; }
        public List<CategoryKeyValues>? CategoryList { get; set; }
        public List<UnitStatusKeyValues>? StatusList { get; set; }
        public List<ProjectDashboardDTO>? ProjectDashboardList { get; set; }
        public List<ProjectsDTO>? ProjectList { get; set; }
        public List<ProjectSummeryDTO>? ProjectSummeryList { get; set; }
        public List<ProjectRoleSummeryDTO>? ProjectRoleSummeryList { get; set; }
        public List<AccessMasterDTO>? AccessList { get; set; }
        public List<CommentsDashboardDTO>? ActivityList { get; set; }
        public List<DocumentCategoryDTO>? DocumentCategoryList { get; set; }
        public List<ProjectFolderDTO>? ProjectFolderList { get; set; }
        public List<ProjectFolderDTO>? ProjectFileList { get; set; }
        public List<ProjectFolderFilesDTO>? FolderFileList { get; set; }
        public List<DrawingCategoryDTO>? DrawingCategoryList { get; set; }
        public List<ProjectDrawingsDTO>? ProjectDrawingList { get; set; }
        public List<ProjectDrawingsDTO>? ProjectDrawingFileList { get; set; }

        public List<DrawingFolderFilesDTO>? DrawingFileList { get; set; }

        public List<PhotoCategoryDTO>? PhotoCategoryList { get; set; }
        public List<ProjectPhotosDTO>? PhotoFolderList { get; set; }
        public List<ProjectFilePhotosDTO>? PhotoFolderFileList { get; set; }

    }
}
