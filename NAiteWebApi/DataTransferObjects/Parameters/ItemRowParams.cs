using NAiteEntities.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace NAiteWebApi.DataTransferObjects.Parameters
{
    public class ItemRowParams : QueryStringParameters
    {
        public ItemRowParams()
        {
            OrderBy = nameof(ItemRow.Created);
        }

        public string? SortKey { get; set; } = string.Empty;
        public string? SortOrder { get; set; } = string.Empty;
        public ItemRowSearchFieldParams[]? SearchFields { get; set; }
    }

    public class ItemRowSearchFieldParams
    {
        public string Id { get; set; } = string.Empty;

        public string Type { get; set; } = string.Empty;

        public string Name { get; set; } = string.Empty;

        public string? Text { get; set; }

        public int? IntStart { get; set; }

        public int? IntEnd { get; set; }

        public DateTime? DateStart { get; set; }

        public DateTime? DateEnd { get; set; }
    }
}
