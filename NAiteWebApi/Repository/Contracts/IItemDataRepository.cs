using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;

namespace NAiteWebApi.Repository.Contracts
{
    public interface IItemDataRepository : IRepositoryBase<ItemData>
    {
        ItemData[] GetItemDatas(ItemDataParams param);
    }
}
