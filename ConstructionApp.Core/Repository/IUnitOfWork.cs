using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Core.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        // public IRegistrationRepository Registration { get; }
        public ICountryMasterRepository CountryMaster { get; }
        public IStatusMasterRepository StatusMaster { get; }
        public IPriorityMasterRepository PriorityMaster { get; }
        public IStateMasterRepository StateMaster { get; }
        public ICityMasterRepository CityMaster { get; }
        public IDepartmentMasterRepository DepartmentMaster { get; }
        public IJobTitleMasterRepository JobTitleMaster { get; }
        public IUsersMasterRepository UsersMaster { get; }
        public ILoginDetailsRepository LoginDetails { get; }
        public IRoleMasterRepository RoleMaster { get; }
        public IUsersMasterDashboardRepository UsersMasterDashboard { get; }
        public IPhasesMasterRepository PhasesMaster { get; }
        public ICompanyPriorityMasterRepository CompanyPriorityMaster { get; }
        public IUnitPhasesMasterRepository UnitPhasesMaster { get; }
        public IUnitStatusMasterRepository UnitStatusMaster { get; }
        public IUnitCategoryMasterRepository UnitCategoryMaster { get; }
        public IMasterCategoryRepository MasterCategory { get; }
        public IProjectsRepository Projects { get; }
        public IProjectDashboardRepository ProjectDashboard { get; }
        public IProjectTasksRepository ProjectTasks { get; }
        public ITaskDashboardRepository TaskDashboard { get; }
        public IProjectSummeryRepository ProjectSummery { get; }
        public IProjectRoleSummeryRepository ProjectRoleSummery { get; }
        public IModuleMasterRepository ModuleMaster { get; }
        public IAccessMasterRepository AccessMaster { get; }
        public IUserActivitiesRepository UserActivities { get; }
        public IUserCommentsRepository UserComments { get; }
        public ICommentsDashboardRepository CommentsDashboard { get; }
        public ITasksDashboardRepository TasksDashboard { get; }
        public IProjectsDashboardRepository ProjectsDashboard { get; }
        public IProjectSubTasksRepository ProjectSubTasks { get; }
        public ISubTaskDashboardRepository SubTaskDashboard { get; }
        public IUserNotificationsRepository UserNotifications { get; }
        public IDocumentCategoryRepository DocumentCategory { get; }
        public IProjectFolderRepository ProjectFolder { get; }
        public IProjectFolderFileRepository ProjectFolderFiles { get; }
        public IDrawingCategoryRepository DrawingCategory { get; }
        public IProjectDrawingsRepository ProjectDrawings { get; }
        public IDrawingFolderFilesRepository DrawingFolderFiles { get; }
        public IPhotoCategoryRepository PhotoCategory { get; }
        public IProjectPhotosRepository ProjectPhotos { get; }
        public IProjectFilePhotosRepository ProjectFilePhotos { get; }
        public IExternalUsersRepository ExternalUsers { get; }
        public IPublicUsersRepository PublicUsers { get; }
        public IAccessClientMasterRepository AccessClientMaster { get; }
        public IApprovalsRepository Approvals { get; }
        public IInvoicesRepository Invoices { get; }
        public IPaymentsRepository Payments { get; }
        public IItemsRepository Items { get; }
        public IStockTransactionsRepository StockTransactions { get; }
        public IStockOutTransactionRepository StockOutTransaction { get; }

        int Save();
    }
}
