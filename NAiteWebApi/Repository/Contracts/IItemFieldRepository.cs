using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.Helpers;

namespace NAiteWebApi.Repository.Contracts
{
    public interface IItemFieldRepository : IRepositoryBase<ItemField>
    {
        ItemField[] GetItemFields();
        ItemField? GetItemField(string id);
        void CreateItemField(ItemField itemField);
        void UpdateItemField(ItemField itemField);
        void DeleteItemField(ItemField itemField);
    }
}
