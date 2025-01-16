using System.ComponentModel.DataAnnotations.Schema;
using NAiteEntities.Models;

namespace NAiteWebApi.DataTransferObjects.Responses
{
    public class ItemRes
    {
        public string Id { get; set; } = null!;

        public string ItemFieldId { get; set; } = null!;

        public string? ValueText { get; set; } = null!;

        public int? ValueInt { get; set; }

        public decimal? ValueDecimal { get; set; }

        public string? ValueDateTime { get; set; }

        public FileRes[]? Files { get; set; }
    }
}
