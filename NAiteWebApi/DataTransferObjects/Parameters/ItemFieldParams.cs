using NAiteEntities.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NAiteWebApi.DataTransferObjects.Parameters
{
    public class ItemFieldParams
    {
    }

    public class CreateItemFieldParams
    {
        [JsonIgnore]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        [DisplayName("フィールド名")]
        [Required(ErrorMessage = "フィールド名は必須です")]
        [StringLength(50, ErrorMessage = "フィールド名は50文字以内にしてください")]
        public string Name { get; set; } = null!;

        [DisplayName("フィールド種別")]
        [Required(ErrorMessage = "フィールド種別は必須です")]
        [StringLength(50, ErrorMessage = "フィールド種別は50文字以内にしてください")]
        public string Type { get; set; } = null!;

        [DisplayName("固定フィールド種別")]
        [StringLength(50, ErrorMessage = "固定フィールド種別は50文字以内にしてください")]
        public string? FixedFieldType { get; set; } = null!;

        [DisplayName("検索対象")]
        [Required(ErrorMessage = "検索対象は必須です")]
        public bool IsSearch { get; set; } = false;

        [DisplayName("インポート時列アルファベット")]
        [StringLength(10, ErrorMessage = "インポート時列アルファベットは10文字以内にしてください")]
        public string? ExcelColumnName { get; set; }

        [JsonIgnore]
        public DateTime Created { get; set; } = DateTime.Now;

        [JsonIgnore]
        public DateTime Modified { get; set; } = DateTime.Now;
    }

    public class UpdateItemFieldParams
    {
        [DisplayName("フィールド名")]
        [Required(ErrorMessage = "フィールド名は必須です")]
        [StringLength(50, ErrorMessage = "フィールド名は50文字以内にしてください")]
        public string Name { get; set; } = null!;

        [DisplayName("固定フィールド種別")]
        [StringLength(50, ErrorMessage = "固定フィールド種別は50文字以内にしてください")]
        public string? FixedFieldType { get; set; } = null!;

        [DisplayName("検索対象")]
        [Required(ErrorMessage = "検索対象は必須です")]
        public bool IsSearch { get; set; } = false;

        [DisplayName("インポート時列アルファベット")]
        [StringLength(10, ErrorMessage = "インポート時列アルファベットは10文字以内にしてください")]
        public string? ExcelColumnName { get; set; }

        [JsonIgnore]
        public DateTime Modified { get; set; } = DateTime.Now;
    }

    public class UpdateItemFieldOrderParams
    {
        public string ReplaceItemFieldId { get; set; } = null!;
    }
}
