using NAiteEntities.Models;

namespace NAiteWebApi.Repository.Contracts
{
    public interface IItemRepository : IRepositoryBase<Item>
    {
        Item[] GetItems();
        Item? GetItem(string id);
        Item? GetItemByRowField(string rowId, string fieldId);
        void CreateItem(Item item);
        void UpdateItem(Item item);
        void DeleteItem(Item item);
    }
}
