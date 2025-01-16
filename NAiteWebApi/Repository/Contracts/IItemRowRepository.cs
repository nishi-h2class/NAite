using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.Helpers;

namespace NAiteWebApi.Repository.Contracts
{
    public interface IItemRowRepository : IRepositoryBase<ItemRow>
    {
        PagedList<ItemRow> GetItemRows(ItemRowParams param, string? type);
        ItemRow[] GetAllItemRows();
        ItemRow? GetItemRow(string id);
        ItemRow? GetItemRowByCode(string code);
        int GetItemRowCount();
        void CreateItemRow(ItemRow itemRow);
        void UpdateItemRow(ItemRow itemRow);
        void DeleteItemRow(ItemRow itemRow);
    }
}
