using ConstructionApp.Core.Entities;


namespace ConstructionApp.Core.Repository
{
    public interface IProjectFolderRepository : IGenericRepository<ProjectFolder>
    {
    }

    public interface IProjectFolderFileRepository : IGenericRepository<ProjectFolderFiles>
    {
    }

    public interface IProjectDrawingsRepository : IGenericRepository<ProjectDrawings>
    {
    }

    public interface IDrawingFolderFilesRepository : IGenericRepository<DrawingFolderFiles>
    {
    }

    public interface IProjectPhotosRepository : IGenericRepository<ProjectPhotos>
    {
    }

    public interface IProjectFilePhotosRepository : IGenericRepository<ProjectFilePhotos>
    {
    }
}
