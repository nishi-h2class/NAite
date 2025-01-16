using Microsoft.EntityFrameworkCore;
using NAiteEntities.Models;
using NAiteWebApi.Repository.Contracts;

namespace NAiteWebApi.Repository.Repositories
{
    public class ItemRepository : RepositoryBase<Item>, IItemRepository
    {
        public ItemRepository(NAiteContext context)
            : base(context)
        {
        }

        public Item[] GetItems()
        {
            var items = FindByCondition(a => a.Deleted == null && a.ItemField.Deleted == null && a.ItemRow.Deleted == null).OrderBy(a => a.ItemField.Order).ToArray();
            return items;
        }

        public Item? GetItem(string id)
        {
            return FindByCondition(a => a.Id == id).Include(a => a.ItemFiles).FirstOrDefault();
        }

        public Item? GetItemByRowField(string rowId, string fieldId)
        {
            return FindByCondition(a => a.ItemRowId == rowId && a.ItemFieldId == fieldId).Include(a => a.ItemFiles).FirstOrDefault();
        }

        public void CreateItem(Item item)
        {
            Create(item);
        }

        public void UpdateItem(Item item)
        {
            Update(item);
        }

        public void DeleteItem(Item item)
        {
            item.Deleted = DateTime.Now;
            Update(item);
        }
    }
}
