using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace NAiteWebApi.DataTransferObjects.Parameters
{
    public class ItemDataParams
    {
        [DisplayName("商品コード")]
        [Required(ErrorMessage = "商品コードは必須です")]
        public string Code { get; set; } = null!;

        [DisplayName("開始日")]
        [Required(ErrorMessage = "開始日は必須です")]
        public DateTime StartDate { get; set; }

        [DisplayName("終了日")]
        [Required(ErrorMessage = "終了日は必須です")]
        public DateTime EndDate { get; set; }

        [DisplayName("種別")]
        public string? Type { get; set; } = null!;
    }
}
