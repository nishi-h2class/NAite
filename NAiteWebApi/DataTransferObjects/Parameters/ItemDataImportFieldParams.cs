using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using NAiteEntities.Models;
using System.Text.Json.Serialization;

namespace NAiteWebApi.DataTransferObjects.Parameters
{
    public class ItemDataImportFieldParams : QueryStringParameters
    {
        public ItemDataImportFieldParams()
        {
            OrderBy = nameof(ItemDataImportField.Order);
        }

        [Required]
        public string ItemDataImportId { get; set; } = string.Empty;

        public string Keyword { get; set; } = string.Empty;
    }

    public class CreateItemDataImportFieldParams : UpdateItemDataImportFieldParams
    {
        [DisplayName("商品データインポートID")]
        public string? ItemDataImportId { get; set; } = null!;
        [JsonIgnore]
        public DateTime Created { get; set; } = DateTime.Now;
    }

    public class UpdateItemDataImportFieldParams
    {
        [DisplayName("タグ")]
        [StringLength(50, ErrorMessage = "タグは50文字以内にしてください")]
        public string? Tag { get; set; } = null!;

        [DisplayName("並び順")]
        public int? Order { get; set; } = null!;

        [JsonIgnore]
        public DateTime Modified { get; set; } = DateTime.Now;
    }
}
