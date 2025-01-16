using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.Helpers;
using NAiteWebApi.Repository.Contracts;

namespace NAiteWebApi.Repository.Repositories
{
    public class ItemFieldRepository : RepositoryBase<ItemField>, IItemFieldRepository
    {
        public ItemFieldRepository(NAiteContext context)
            : base(context)
        {
            
        }

        public ItemField[] GetItemFields()
        {
            var fields = FindByCondition(a => a.Deleted == null).OrderBy(a => a.Order).ToArray();
            return fields;
        }

        public ItemField? GetItemField(string id)
        {
            return FindByCondition(a => a.Id == id).FirstOrDefault();
        }

        public void CreateItemField(ItemField field)
        {
            var fields = FindByCondition(a => a.Deleted == null).OrderBy(a => a.Order).ToArray();
            int order = 1;
            if (fields.Length > 0)
            {
                var maxOrder = fields.Max(a => a.Order);
                order = maxOrder + 1;
            }
            field.Order = order;

            Create(field);
        }

        public void UpdateItemField(ItemField field)
        {
            Update(field);
        }

        public void DeleteItemField(ItemField field)
        {
            field.Deleted = DateTime.Now;
            Update(field);
        }

    }
}
