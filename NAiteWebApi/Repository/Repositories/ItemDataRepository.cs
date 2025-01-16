using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.Repository.Contracts;

namespace NAiteWebApi.Repository.Repositories
{
    public class ItemDataRepository : RepositoryBase<ItemData>, IItemDataRepository
    {
        public ItemDataRepository(NAiteContext context)
            : base(context)
        {
        }

        public ItemData[] GetItemDatas(ItemDataParams param)
        {
            var itemDatas = FindByCondition(a => a.Deleted == null && a.Code == param.Code);

            if (!string.IsNullOrEmpty(param.Type))
            {
                itemDatas = itemDatas.Where(a => a.Type == param.Type);
            }

            itemDatas = itemDatas.Where(a => a.Date >= param.StartDate && a.Date < param.EndDate.AddDays(1));

            return itemDatas.OrderBy(a => a.Date).ToArray();
        }
    }
}
