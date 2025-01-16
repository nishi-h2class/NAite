using Microsoft.EntityFrameworkCore;
using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.Helpers;
using NAiteWebApi.Repository.Contracts;

namespace NAiteWebApi.Repository.Repositories
{
    public class ItemDataImportRepository : RepositoryBase<ItemDataImport>, IItemDataImportRepository
    {
        private ISortHelper<ItemDataImport> _sortHelper;

        public ItemDataImportRepository(NAiteContext context,
            ISortHelper<ItemDataImport> sortHelper)
            : base(context)
        {
            _sortHelper = sortHelper;
        }

        public PagedList<ItemDataImport> GetItemDataImports(ItemDataImportParams param)
        {
            var itemDataImports = FindByCondition(a => a.Deleted == null && a.Reserved != null);

            itemDataImports = itemDataImports.Include(a => a.User);

            var sortedItemDataImports = _sortHelper.ApplySort(itemDataImports, param.OrderBy);

            return PagedList<ItemDataImport>.ToPagedList(
                sortedItemDataImports.ToList(),
                param.PageNumber,
                param.PageSize
                );
        }

        public ItemDataImport? GetItemDataImport(string id)
        {
            return FindByCondition(a => a.Id == id).Include(u => u.User).FirstOrDefault();
        }

        public void CreateItemDataImport(ItemDataImport itemDataImport)
        {
            Create(itemDataImport);
        }

        public void UpdateItemDataImport(ItemDataImport itemDataImport)
        {
            Update(itemDataImport);
        }

        public void DeleteItemDataImport(ItemDataImport itemDataImport)
        {
            itemDataImport.Deleted = DateTime.Now;
            Update(itemDataImport);
        }
    }
}
