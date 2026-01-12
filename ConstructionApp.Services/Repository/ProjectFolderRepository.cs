using ConstructionApp.Core.Entities;
using ConstructionApp.Core.Repository;
using ConstructionApp.Services.DBContext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ConstructionApp.Services.Repository
{
    public class ProjectFolderRepository : GenericRepository<ProjectFolder>, IProjectFolderRepository
    {
        public ProjectFolderRepository(ConstDbContext context) : base(context)
        {

        }
    }

    public class ProjectFolderFilesRepository : GenericRepository<ProjectFolderFiles>, IProjectFolderFileRepository
    {
        public ProjectFolderFilesRepository(ConstDbContext context) : base(context)
        {

        }
    }

    public class ProjectDrawingsRepository : GenericRepository<ProjectDrawings>, IProjectDrawingsRepository
    {
        public ProjectDrawingsRepository(ConstDbContext context) : base(context)
        {

        }
    }

    public class DrawingFolderFilesRepository : GenericRepository<DrawingFolderFiles>, IDrawingFolderFilesRepository
    {
        public DrawingFolderFilesRepository(ConstDbContext context) : base(context)
        {

        }
    }

    public class ProjectPhotosRepository : GenericRepository<ProjectPhotos>, IProjectPhotosRepository
    {
        public ProjectPhotosRepository(ConstDbContext context) : base(context)
        {

        }
    }

    public class ProjectFilePhotosRepository : GenericRepository<ProjectFilePhotos>, IProjectFilePhotosRepository
    {
        public ProjectFilePhotosRepository(ConstDbContext context) : base(context)
        {

        }
    }
}
