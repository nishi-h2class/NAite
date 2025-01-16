using NAiteEntities.Models;

namespace NAiteWebApi.Repository.Contracts
{
    public interface IItemFileRepository : IRepositoryBase<ItemFile>
    {
        ItemFile[] GetItemFilesByItem(string id);
        ItemFile[] GetItemFilesByFile(string id);
        void CreateItemFile(ItemFile itemFile);
        void DeleteItemFile(ItemFile itemFile);
    }
}
