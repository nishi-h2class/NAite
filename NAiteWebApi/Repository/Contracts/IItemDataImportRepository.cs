using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.Helpers;

namespace NAiteWebApi.Repository.Contracts
{
    public interface IItemDataImportRepository : IRepositoryBase<ItemDataImport>
    {
        PagedList<ItemDataImport> GetItemDataImports(ItemDataImportParams param);
        ItemDataImport? GetItemDataImport(string id);
        void CreateItemDataImport(ItemDataImport itemDataImport);
        void UpdateItemDataImport(ItemDataImport itemDataImport);
        void DeleteItemDataImport(ItemDataImport itemDataImport);
    }
}
