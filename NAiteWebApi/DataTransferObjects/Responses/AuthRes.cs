namespace NAiteWebApi.DataTransferObjects.Responses
{
    public class AuthRes
    {
        /// <summary>
        /// ID
        /// </summary>
        public string Id { get; set; } = null!;

        /// <summary>
        /// 氏名
        /// </summary>
        public string Name { get; set; } = null!;

        /// <summary>
        /// トークン
        /// </summary>
        public string? Token { get; set; } = null;

        /// <summary>
        /// 権限
        /// </summary>
        public string Authority { get; set; } = null!;
    }
}
