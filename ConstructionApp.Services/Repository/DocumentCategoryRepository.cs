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
    public class DocumentCategoryRepository : GenericRepository<DocumentCategory>, IDocumentCategoryRepository
    {
        public DocumentCategoryRepository(ConstDbContext context) : base(context)
        {

        }
    }

    public class DrawingCategoryRepository : GenericRepository<DrawingCategory>, IDrawingCategoryRepository
    {
        public DrawingCategoryRepository(ConstDbContext context) : base(context)
        {

        }
    }

    public class PhotoCategoryRepository : GenericRepository<PhotoCategory>, IPhotoCategoryRepository
    {
        public PhotoCategoryRepository(ConstDbContext context) : base(context)
        {

        }
    }
}
