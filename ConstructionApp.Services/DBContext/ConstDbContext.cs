using ConstructionApp.Core.Entities;
using Microsoft.EntityFrameworkCore;


namespace ConstructionApp.Services.DBContext
{
    public partial class ConstDbContext: DbContext
    {
        public ConstDbContext()
        {

        }
        public ConstDbContext(DbContextOptions<ConstDbContext> options)
        : base(options)
        {
        }
       
        public virtual DbSet<PriorityMaster> PriorityMasters { get; set; }
        public virtual DbSet<Projects> Projectss { get; set; }
        public virtual DbSet<UsersMaster> UsersMasters { get; set; }
        public virtual DbSet<LoginDetails> LoginsDetails { get; set; }
        public virtual DbSet<StatusMaster> StatusMasters { get; set; }
        public virtual DbSet<CityMaster> CityMasters { get; set; }
        public virtual DbSet<CountryMaster> CountryMasters { get; set; }
        public virtual DbSet<StateMaster> StateMasters { get; set; }
        public virtual DbSet<DepartmentMaster> DepartmentMasters { get; set; }
        public virtual DbSet<JobTitleMaster> JobTitleMasters { get; set; }
        public virtual DbSet<RoleMaster> RoleMasters { get; set; }
        public virtual DbSet<PhasesMaster> PhasesMasters { get; set; }
        public virtual DbSet<CompanyPriorityMaster> CompanyPriorityMasters { get; set; }
        public virtual DbSet<UnitStatusMaster> UnitStatusMasters { get; set; }
        public virtual DbSet<UnitPhasesMaster> UnitPhasesMasters { get; set; }
        public virtual DbSet<UnitCategoryMaster> UnitCategoryMasters { get; set; }
        public virtual DbSet<MasterCategory> CategoryMasters { get; set; }
        public virtual DbSet<ProjectTasks> ProjectTaskss { get; set; }
        public virtual DbSet<ModuleMaster> ModuleMasters { get; set; }
        public virtual DbSet<UserActivities> UserActivitiess { get; set; }
        public virtual DbSet<UserComments> UserCommentss { get; set; }
        public virtual DbSet<ProjectSubTasks> ProjectSubTaskss { get; set; }
        public virtual DbSet<DocumentCategory> DocumentCategorys { get; set; }
        public virtual DbSet<ProjectFolder> ProjectFolders { get; set; }
        public virtual DbSet<ProjectFolderFiles> ProjectFolderFile { get; set; }
        public virtual DbSet<DrawingCategory> DrawingCategorys { get; set; }
        public virtual DbSet<ProjectDrawings> ProjectDrawing { get; set; }
       public virtual DbSet<DrawingFolderFiles> DrawingFolderFile { get; set; }
        public virtual DbSet<PhotoCategory> PhotoCategorys { get; set; }
        public virtual DbSet<ProjectPhotos> ProjectPhoto { get; set; }
        public virtual DbSet<ProjectFilePhotos> ProjectFilePhoto { get; set; }
        public virtual DbSet<Approvals> Approval { get; set; }
        public virtual DbSet<Invoices> Invoice { get; set; }
        public virtual DbSet<Payments> Payment { get; set; }
        public virtual DbSet<Items> Item { get; set; }
        public virtual DbSet<StockTransactions> StockTransaction { get; set; }
        public virtual DbSet<StockOutTransaction> StockOutTransactions { get; set; }

    }
}
