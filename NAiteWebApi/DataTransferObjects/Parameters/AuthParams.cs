using System.ComponentModel.DataAnnotations;
using System.ComponentModel;

namespace NAiteWebApi.DataTransferObjects.Parameters
{
    public class AuthParams
    {
        /// <summary>
        /// ログインID
        /// </summary>
        [DisplayName("ログインID")]
        [Required(ErrorMessage = "ログインIDは必須です")]
        public string LoginId { get; set; } = null!;

        /// <summary>
        /// ログインパスワード
        /// </summary>
        [DisplayName("パスワード")]
        [Required(ErrorMessage = "パスワードは必須です")]
        public string Password { get; set; } = null!;
    }
}
