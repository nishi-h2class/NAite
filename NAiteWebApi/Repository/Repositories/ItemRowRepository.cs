using Microsoft.EntityFrameworkCore;
using NAiteEntities.Models;
using NAiteWebApi.DataTransferObjects.Parameters;
using NAiteWebApi.DataTransferObjects.Responses;
using NAiteWebApi.Helpers;
using NAiteWebApi.Repository.Contracts;
using System.Xml.Linq;

namespace NAiteWebApi.Repository.Repositories
{
    public class ItemRowRepository : RepositoryBase<ItemRow>, IItemRowRepository
    {
        private ISortHelper<ItemRow> _sortHelper;

        public ItemRowRepository(NAiteContext context,
            ISortHelper<ItemRow> sortHelper)
            : base(context)
        {
            _sortHelper = sortHelper;
        }

        public PagedList<ItemRow> GetItemRows(ItemRowParams param, string? type)
        {
            var itemRows = FindByCondition(a => a.Deleted == null);

            SearchByName(ref itemRows, param.SearchFields);

            itemRows = itemRows.Include(a => a.Items.OrderBy(b => b.ItemField.Order).Where(b => b.ItemField.Deleted == null)).ThenInclude(item => item.ItemField).Include(a => a.Items).ThenInclude(item => item.ItemFiles).ThenInclude(itemFile => itemFile.File);

            var sortedItemRows = _sortHelper.ApplySort(itemRows, param.OrderBy);

            if (param.SortOrder != null)
            {
                if (param.SortOrder == "asc")
                {
                    sortedItemRows = itemRows.OrderBy(ir => type == "int" ? (object?)ir.Items.First(i => i.ItemFieldId == param.SortKey).ValueInt : type == "decimal" ? (object?)ir.Items.First(i => i.ItemFieldId == param.SortKey).ValueDecimal : type == "text" ? (object?)ir.Items.First(i => i.ItemFieldId == param.SortKey).ValueText : (object?)ir.Items.First(i => i.ItemFieldId == param.SortKey).ValueDateTime);
                }
                else
                {
                    sortedItemRows = itemRows.OrderByDescending(ir => type == "int" ? (object?)ir.Items.First(i => i.ItemFieldId == param.SortKey).ValueInt : type == "decimal" ? (object?)ir.Items.First(i => i.ItemFieldId == param.SortKey).ValueDecimal : type == "text" ? (object?)ir.Items.First(i => i.ItemFieldId == param.SortKey).ValueText : (object?)ir.Items.First(i => i.ItemFieldId == param.SortKey).ValueDateTime);
                }
            }

            return PagedList<ItemRow>.ToPagedList(
                sortedItemRows.ToList(),
                param.PageNumber,
                param.PageSize
                );
        }

        private void SearchByName(ref IQueryable<ItemRow> itemRows, ItemRowSearchFieldParams[]? searchFields)
        {
            if (!itemRows.Any() || searchFields == null)
                return;

            foreach (var field in searchFields)
            {
                switch (field.Type)
                {
                    case "int":
                        if (field.IntStart != null)
                        {
                            itemRows = itemRows.Where(o => o.Items.Where(a => a.ValueInt >= field.IntStart).Count() > 0);
                        }
                        if (field.IntEnd != null)
                        {
                            itemRows = itemRows.Where(o => o.Items.Where(a => a.ValueInt <= field.IntEnd).Count() > 0);
                        }
                        break;
                    case "decimal":
                        if (field.IntStart != null)
                        {
                            itemRows = itemRows.Where(o => o.Items.Where(a => a.ValueDecimal >= field.IntStart).Count() > 0);
                        }
                        if (field.IntEnd != null)
                        {
                            itemRows = itemRows.Where(o => o.Items.Where(a => a.ValueDecimal <= field.IntEnd).Count() > 0);
                        }
                        break;
                    case "date":
                        if (field.DateStart != null)
                        {
                            itemRows = itemRows.Where(o => o.Items.Where(a => a.ValueDateTime >= field.DateStart).Count() > 0);
                        }
                        if (field.DateEnd != null)
                        {
                            itemRows = itemRows.Where(o => o.Items.Where(a => a.ValueDateTime <= field.DateEnd).Count() > 0);
                        }
                        break;
                    case "datetime":
                        if (field.DateStart != null)
                        {
                            itemRows = itemRows.Where(o => o.Items.Where(a => a.ValueDateTime >= field.DateStart).Count() > 0);
                        }
                        if (field.DateEnd != null)
                        {
                            itemRows = itemRows.Where(o => o.Items.Where(a => a.ValueDateTime <= field.DateEnd).Count() > 0);
                        }
                        break;
                    default:
                        if (!string.IsNullOrEmpty(field.Text))
                        {
                            itemRows = itemRows.Where(o => o.Items.Where(a => a.ValueText!.ToLower().Contains(field.Text.Trim().ToLower())).Count() > 0);
                        }
                        break;
                }
            }
        }

        public ItemRow[] GetAllItemRows()
        {
            var rows = FindByCondition(a => a.Deleted == null).OrderBy(a => a.DefaultOrder).ToArray();
            return rows;
        }

        public ItemRow? GetItemRow(string id)
        {
            return FindByCondition(a => a.Id == id).Include(u => u.Items).FirstOrDefault();
        }

        public ItemRow? GetItemRowByCode(string code)
        {
            return FindByCondition(a => a.Deleted == null && a.Items.Where(b => b.ItemField.FixedFieldType == "Code" && b.ValueText == code).Count() > 0).Include(a => a.Items).ThenInclude(item => item.ItemField).FirstOrDefault();
        }

        public int GetItemRowCount()
        {
            var count = FindByCondition(a => a.Deleted == null).Count();
            if (count > 0)
            {
                var max = FindByCondition(a => a.Deleted == null).Max(a => a.DefaultOrder);
                count = max + 1;
            }
            return count;
        }

        public void CreateItemRow(ItemRow itemRow)
        {
            Create(itemRow);
        }

        public void UpdateItemRow(ItemRow itemRow)
        {
            Update(itemRow);
        }

        public void DeleteItemRow(ItemRow itemRow)
        {
            itemRow.Deleted = DateTime.Now;
            Update(itemRow);
        }
    }
}
