using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.Helpers;
using NAiteWebApi.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace NAiteWebApi.Repository.Repositories
{
    public class ItemDataImportFieldRepository : RepositoryBase<ItemDataImportField>, IItemDataImportFieldRepository
    {
        public ItemDataImportFieldRepository(NAiteContext context)
            : base(context)
        {
        }

        public ItemDataImportField[] GetItemDataImportFields(ItemDataImportFieldParams param)
        {
            var itemDataImportFields = FindByCondition(a => a.Deleted == null);

            if (!string.IsNullOrEmpty(param.ItemDataImportId))
            {
                itemDataImportFields = itemDataImportFields.Where(a => a.ItemDataImportId == param.ItemDataImportId);
            }

            return itemDataImportFields.OrderBy(a => a.Order).ToArray();
        }

        public ItemDataImportField? GetItemDataImportField(int id)
        {
            return FindByCondition(a => a.Id == id).Include(u => u.ItemDataImport).FirstOrDefault();
        }

        public void CreateItemDataImportField(ItemDataImportField itemDataImportField)
        {
            Create(itemDataImportField);
        }

        public void UpdateItemDataImportField(ItemDataImportField itemDataImportField)
        {
            Update(itemDataImportField);
        }

        public void DeleteItemDataImportField(ItemDataImportField itemDataImportField)
        {
            Delete(itemDataImportField);
        }
    }
}
