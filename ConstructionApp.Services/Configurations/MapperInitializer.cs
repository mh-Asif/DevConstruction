using AutoMapper;
using ConstructionApp.Core.Entities;
using Construction.Infrastructure.Models;
using Construction.Infrastructure.KeyValues;

namespace ConstructionApp.Services.Configurations
{
    public class MapperInitializer : Profile
    {
        public MapperInitializer()
        {
            CreateMap<CityMaster, CityMasterDTO>().ReverseMap();
            CreateMap<CountryMaster, CountryMasterDTO>().ReverseMap();
            CreateMap<StateMaster, StateMasterDTO>().ReverseMap();
            CreateMap<StatusMaster, StatusMasterDTO>().ReverseMap();
            CreateMap<PriorityMaster, PriorityMasterDTO>().ReverseMap();
            CreateMap<DepartmentMaster, DepartmentMasterDTO>().ReverseMap();
            CreateMap<JobTitleMaster, JobTitleMasterDTO>().ReverseMap();
            CreateMap<UsersMaster, UsersMasterDTO>().ReverseMap();          
            CreateMap<LoginDetails, LoginDetailsDTO>().ReverseMap();
            CreateMap<RoleMaster, RoleMasterDTO>().ReverseMap();
            CreateMap<PhasesMaster, PhasesMasterDTO>().ReverseMap();
            CreateMap<CompanyPriorityMaster, CompanyPriorityMasterDTO>().ReverseMap();
            CreateMap<CompanyPriorityMaster,UnitPriorityKeyValues>().ReverseMap();
            CreateMap<UnitPhasesMaster, UnitPhasesMasterDTO>().ReverseMap();
            CreateMap<UnitStatusMaster, UnitStatusMasterDTO>().ReverseMap();
            CreateMap<UnitCategoryMaster, UnitCategoryMasterDTO>().ReverseMap();
            CreateMap<MasterCategory, MasterCategoryDTO>().ReverseMap();
            CreateMap<Projects, ProjectsDTO>().ReverseMap();
            CreateMap<ProjectTasks, ProjectTasksDTO>().ReverseMap();
            CreateMap<ModuleMaster, ModuleMasterDTO>().ReverseMap();
            CreateMap<AccessMaster, AccessMasterDTO>().ReverseMap();
            CreateMap<UserActivities, UserActivitiesDTO>().ReverseMap();
            CreateMap<UserComments, UserCommentsDTO>().ReverseMap();
            CreateMap<CommentsDashboard, CommentsDashboardDTO>().ReverseMap();
            CreateMap<TasksDashboard, TasksDashboardDTO>().ReverseMap();
            CreateMap<ProjectsDashboard, ProjectsDashboardDTO>().ReverseMap();
            CreateMap<ProjectSubTasks, ProjectSubTasksDTO>().ReverseMap();
            CreateMap<SubTaskDashboard, SubTaskDashboardDTO>().ReverseMap();
            CreateMap<UserNotifications, UserNotificationsDTO>().ReverseMap();
            CreateMap<ProjectTasks,TasksDTO> ().ReverseMap();

            CreateMap<UserKeyValues, UsersMasterDTO>().ReverseMap();
            CreateMap<UnitPriorityKeyValues, CompanyPriorityMasterDTO>().ReverseMap();
            CreateMap<UnitStatusKeyValues, UnitStatusMasterDTO>().ReverseMap();
            CreateMap<CategoryKeyValues, UnitCategoryMasterDTO>().ReverseMap();
            CreateMap<ProjectFolder, ProjectFolderDTO>().ReverseMap();
            CreateMap<ProjectFolderFiles, ProjectFolderFilesDTO>().ReverseMap();
            CreateMap<DocumentCategory, DocumentCategoryDTO>().ReverseMap();
            CreateMap<DrawingCategory, DrawingCategoryDTO>().ReverseMap();
            CreateMap<ProjectDrawings, ProjectDrawingsDTO>().ReverseMap();
            CreateMap<DrawingFolderFiles, DrawingFolderFilesDTO>().ReverseMap();
            CreateMap<PhotoCategory, PhotoCategoryDTO>().ReverseMap();
            CreateMap<ProjectPhotos, ProjectPhotosDTO>().ReverseMap();
            CreateMap<ProjectFilePhotos, ProjectFilePhotosDTO>().ReverseMap();
            CreateMap<ExternalUsers, ExternalUsersDTO>().ReverseMap();
            CreateMap<PublicUsers, PublicUsersDTO>().ReverseMap();
            CreateMap<Invoices, InvoicesDTO>().ReverseMap();
            CreateMap<Approvals, ApprovalsDTO>().ReverseMap();
            CreateMap<Payments, PaymentsDTO>().ReverseMap();
            CreateMap<Items, ItemsDTO>().ReverseMap();
            CreateMap<StockTransactions, StockTransactionsDTO>().ReverseMap();
            CreateMap<StockOutTransaction, StockOutTransactionDTO>().ReverseMap();
        }
    }
}
