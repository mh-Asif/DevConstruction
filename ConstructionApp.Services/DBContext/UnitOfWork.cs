using AutoMapper;
using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.Repository;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Services.DBContext
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ConstDbContext _context;
        private readonly DapperDBContext _dapperDBContext;
        public UnitOfWork(ConstDbContext context, DapperDBContext dapperDBContext, IMapper mapper)
        {
            _context = context;
            _dapperDBContext = dapperDBContext;

            StateMaster = new StateMasterRepository(_context);
            CityMaster = new CityMasterRepository(_context);
            CountryMaster = new CountryMasterRepository(_context);
            StatusMaster = new StatusMasterRepository(_context);
            PriorityMaster = new PriorityMasterRepository(_context);
            DepartmentMaster = new DepartmentMasterRepository(_context);
            JobTitleMaster = new JobTitleMasterRepository(_context);
            UsersMaster = new UsersMasterRepository(_context, mapper);
            LoginDetails = new LoginDetailsRepository(_context);
            RoleMaster = new RoleMasterRepository(_context);
            PhasesMaster = new PhasesMasterRepository(_context);
            CompanyPriorityMaster = new CompanyPriorityMasterRepository(_context);
            UnitPhasesMaster = new UnitPhasesMasterRepository(_context);
            UnitStatusMaster = new UnitStatusMasterRepository(_context);
            UnitCategoryMaster = new UnitCategoryMasterRepository(_context);
            MasterCategory = new MasterCategoryRepository(_context);
            UsersMasterDashboard = new UsersMasterDashboardRepository(_dapperDBContext);
            ProjectDashboard = new ProjectDashboardRepository(_dapperDBContext);
            TaskDashboard = new TaskDashboardRepository(_dapperDBContext);
            Projects =new ProjectsRepository(_context);
            ProjectTasks= new ProjectTasksRepository(_context);
            ProjectSummery= new ProjectSummeryRepository(_dapperDBContext);
            ProjectRoleSummery = new ProjectRoleSummeryRepository(_dapperDBContext);
            ModuleMaster = new ModuleMasterRepository(_context);
            AccessMaster = new AccessMasterRepository(_dapperDBContext);
            UserActivities = new UserActivitiesRepository(_context);
            UserComments = new UserCommentsRepository(_context);
            CommentsDashboard = new CommentsDashboardRepository(_dapperDBContext);
            TasksDashboard= new TasksDashboardRepository(_dapperDBContext);
            ProjectsDashboard = new ProjectsDashboardRepository(_dapperDBContext);
            ProjectSubTasks = new ProjectSubTasksRepository(_context);
            SubTaskDashboard = new SubTaskDashboardRepository(_dapperDBContext);
            UserNotifications = new UserNotificationsRepository(_dapperDBContext);
            DocumentCategory = new DocumentCategoryRepository(_context);
            ProjectFolder= new ProjectFolderRepository(_context);
            ProjectFolderFiles= new ProjectFolderFilesRepository(_context);
            DrawingCategory= new DrawingCategoryRepository(_context);
            ProjectDrawings = new ProjectDrawingsRepository(_context);
            DrawingFolderFiles= new DrawingFolderFilesRepository(_context);
             PhotoCategory = new PhotoCategoryRepository(_context);
            ProjectPhotos = new ProjectPhotosRepository(_context);
            ProjectFilePhotos = new ProjectFilePhotosRepository(_context);
            ExternalUsers = new ExternalUsersRepository(_dapperDBContext);
            PublicUsers = new PublicUsersRepository(_dapperDBContext);
            AccessClientMaster = new AccessClientMasterRepository(_dapperDBContext);
            Approvals = new ApprovalsRepository(_context);
            Invoices = new InvoicesRepository(_context);
            Payments = new PaymentsRepository(_context);
            Items = new ItemsRepository(_context);
            StockTransactions = new StockTransactionsRepository(_context);
            StockOutTransaction = new StockOutTransactionRepository(_context);
        }

        public ICountryMasterRepository CountryMaster { get; private set; }

        public IStateMasterRepository StateMaster { get; private set; }
        public ICityMasterRepository CityMaster { get; private set; }
        public IStatusMasterRepository StatusMaster { get; private set; }
        public IPriorityMasterRepository PriorityMaster { get; private set; }
        public IDepartmentMasterRepository DepartmentMaster { get; private set; }
        public IJobTitleMasterRepository JobTitleMaster { get; private set; }
        public IUsersMasterRepository UsersMaster { get; private set; }
        public ILoginDetailsRepository LoginDetails { get; private set; }
        public IRoleMasterRepository RoleMaster { get; private set; }
        public IUsersMasterDashboardRepository UsersMasterDashboard { get; private set; }
        public IPhasesMasterRepository PhasesMaster { get; private set; }
        public ICompanyPriorityMasterRepository CompanyPriorityMaster { get; private set; }

        public IUnitPhasesMasterRepository UnitPhasesMaster { get; private set; }
        public IUnitStatusMasterRepository UnitStatusMaster { get; private set; }
        public IUnitCategoryMasterRepository UnitCategoryMaster { get; private set; }
        public IMasterCategoryRepository MasterCategory { get; private set; }
        public IProjectsRepository Projects { get; private set; }
        public IProjectDashboardRepository ProjectDashboard { get; private set; }
        public IProjectTasksRepository ProjectTasks { get; private set; }
        public ITaskDashboardRepository TaskDashboard { get; private set; }
        public IProjectSummeryRepository ProjectSummery { get; private set; }
        public IProjectRoleSummeryRepository ProjectRoleSummery { get; private set; }
        public IModuleMasterRepository ModuleMaster { get; private set; }
        public IAccessMasterRepository AccessMaster { get; private set; }
        public IUserActivitiesRepository UserActivities { get; private set; }
        public IUserCommentsRepository UserComments { get; private set; }
        public ICommentsDashboardRepository CommentsDashboard { get; private set; }

        public ITasksDashboardRepository TasksDashboard { get; private set; }
        public IProjectsDashboardRepository ProjectsDashboard { get; private set; }
        public IProjectSubTasksRepository ProjectSubTasks { get; private set; }
        public ISubTaskDashboardRepository SubTaskDashboard { get; private set; }

        public IUserNotificationsRepository UserNotifications { get; private set; }

        public IDocumentCategoryRepository DocumentCategory { get; private set; }
        public IProjectFolderRepository ProjectFolder { get; private set; }
        public IProjectFolderFileRepository ProjectFolderFiles { get; private set; }

        public IDrawingCategoryRepository DrawingCategory { get; private set; }

        public IProjectDrawingsRepository ProjectDrawings { get; private set; }

       public IDrawingFolderFilesRepository DrawingFolderFiles { get; private set; }

        public IPhotoCategoryRepository PhotoCategory { get; private set; }
        public IProjectPhotosRepository ProjectPhotos { get; private set; }
        public IProjectFilePhotosRepository ProjectFilePhotos { get; private set; }
        public IExternalUsersRepository ExternalUsers { get; private set; }
        public IPublicUsersRepository PublicUsers { get; private set; }
        public IAccessClientMasterRepository AccessClientMaster { get; private set; }
        public IApprovalsRepository Approvals { get; private set; }
        public IInvoicesRepository Invoices { get; private set; }
        public IPaymentsRepository Payments { get; private set; }
        public IItemsRepository Items { get; private set; }
        public IStockTransactionsRepository StockTransactions { get; private set; }
        public IStockOutTransactionRepository StockOutTransaction { get; private set; }
        public void Dispose()
        {
            try { _context.Dispose(); }
            catch { }
        }

        public int Save()
        {
            return _context.SaveChanges();
        }
    }
}
