using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using NAiteEntities.Models;
using System.Text.Json.Serialization;

namespace NAiteWebApi.DataTransferObjects.Parameters
{
    public class ItemDataImportParams : QueryStringParameters
    {
        public ItemDataImportParams()
        {
            OrderBy = nameof(ItemDataImport.Created);
        }

        public string Keyword { get; set; } = string.Empty;
    }

    public class UpdateItemDataImportParams
    {
        public bool IsHeader { get; set; } = false;

        [DisplayName("ユーザID")]
        [Required(ErrorMessage = "ユーザIDは必須です")]
        public string UserId { get; set; } = null!;

        [DisplayName("ファイル種別")]
        [Required(ErrorMessage = "ファイル種別は必須です")]
        public string FileType { get; set; } = null!;

        [DisplayName("フィールド")]
        public List<CreateItemDataImportFieldParams> Fields { get; set; } = new List<CreateItemDataImportFieldParams>();

        [JsonIgnore]
        public DateTime Reserved { get; set; } = DateTime.Now;

        [JsonIgnore]
        public DateTime Modified { get; set; } = DateTime.Now;
    }

    public class UpdateItemDataImportFielsParams
    {
        [DisplayName("フィールド")]
        public List<CreateItemDataImportFieldParams> Fields { get; set; } = new List<CreateItemDataImportFieldParams>();

        [JsonIgnore]
        public DateTime Modified { get; set; } = DateTime.Now;
    }
}
