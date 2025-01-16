using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;

namespace NAiteWebApi.Repository.Contracts
{
    public interface IItemDataImportFieldRepository : IRepositoryBase<ItemDataImportField>
    {
        ItemDataImportField[] GetItemDataImportFields(ItemDataImportFieldParams param);
        ItemDataImportField? GetItemDataImportField(int id);
        void CreateItemDataImportField(ItemDataImportField itemDataImportField);
        void UpdateItemDataImportField(ItemDataImportField itemDataImportField);
        void DeleteItemDataImportField(ItemDataImportField itemDataImportField);
    }
}
