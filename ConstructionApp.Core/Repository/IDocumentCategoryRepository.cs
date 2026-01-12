using ConstructionApp.Core.Entities;


namespace ConstructionApp.Core.Repository
{
    public interface IDocumentCategoryRepository : IGenericRepository<DocumentCategory>
    {
    }

    public interface IDrawingCategoryRepository : IGenericRepository<DrawingCategory>
    {
    }

    public interface IPhotoCategoryRepository : IGenericRepository<PhotoCategory>
    {
    }
}
