using NAiteEntities.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel;
using System.Text.Json.Serialization;

namespace NAiteWebApi.DataTransferObjects.Parameters
{
    public class UserParams : QueryStringParameters
    {
        public UserParams()
        {
            OrderBy = nameof(User.LoginId);
        }

        public string? Keyword { get; set; } = string.Empty;
    }

    public class CreateUserParams
    {
        [JsonIgnore]
        public string Id { get; set; } = Guid.NewGuid().ToString("N");

        [DisplayName("社員番号")]
        [Required(ErrorMessage = "社員番号は必須です")]
        [StringLength(50, ErrorMessage = "社員番号は50文字以内にしてください")]
        public string LoginId { get; set; } = null!;

        [DisplayName("姓")]
        [Required(ErrorMessage = "姓は必須です")]
        [StringLength(50, ErrorMessage = "姓は50文字以内にしてください")]
        public string LastName { get; set; } = null!;

        [DisplayName("名")]
        [Required(ErrorMessage = "名は必須です")]
        [StringLength(50, ErrorMessage = "名は50文字以内にしてください")]
        public string FirstName { get; set; } = null!;

        [DisplayName("メールアドレス")]
        [Required(ErrorMessage = "メールアドレスは必須です")]
        [StringLength(256, ErrorMessage = "メールアドレスは256文字以内にしてください")]
        public string Email { get; set; } = null!;

        [DisplayName("権限")]
        [Required(ErrorMessage = "権限は必須です")]
        [StringLength(50, ErrorMessage = "権限は50文字以内にしてください")]
        public string Authority { get; set; } = null!;

        [DisplayName("部署名")]
        [Required(ErrorMessage = "部署名は必須です")]
        [StringLength(50, ErrorMessage = "部署名は50文字以内にしてください")]
        public string Department { get; set; } = null!;

        [DisplayName("商品データベースの更新通知")]
        [Required(ErrorMessage = "商品データベースの更新通知は必須です")]
        public bool IsNotifyUpdateData { get; set; } = false;

        [JsonIgnore]
        public DateTime Created { get; set; } = DateTime.Now;

        [JsonIgnore]
        public DateTime Modified { get; set; } = DateTime.Now;
    }

    public class UpdateUserParams
    {
        [DisplayName("社員番号")]
        [Required(ErrorMessage = "社員番号は必須です")]
        [StringLength(50, ErrorMessage = "社員番号は50文字以内にしてください")]
        public string LoginId { get; set; } = null!;

        [DisplayName("姓")]
        [Required(ErrorMessage = "姓は必須です")]
        [StringLength(50, ErrorMessage = "姓は50文字以内にしてください")]
        public string LastName { get; set; } = null!;

        [DisplayName("名")]
        [Required(ErrorMessage = "名は必須です")]
        [StringLength(50, ErrorMessage = "名は50文字以内にしてください")]
        public string FirstName { get; set; } = null!;

        [DisplayName("メールアドレス")]
        [Required(ErrorMessage = "メールアドレスは必須です")]
        [StringLength(256, ErrorMessage = "メールアドレスは256文字以内にしてください")]
        public string Email { get; set; } = null!;

        [DisplayName("権限")]
        [Required(ErrorMessage = "権限は必須です")]
        [StringLength(50, ErrorMessage = "権限は50文字以内にしてください")]
        public string Authority { get; set; } = null!;

        [DisplayName("部署名")]
        [Required(ErrorMessage = "部署名は必須です")]
        [StringLength(50, ErrorMessage = "部署名は50文字以内にしてください")]
        public string Department { get; set; } = null!;

        [DisplayName("商品データベースの更新通知")]
        [Required(ErrorMessage = "商品データベースの更新通知は必須です")]
        public bool IsNotifyUpdateData { get; set; } = false;

        [JsonIgnore]
        public DateTime Modified { get; set; } = DateTime.Now;
    }
}
