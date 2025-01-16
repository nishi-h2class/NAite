namespace NAiteWebApi.DataTransferObjects.Responses
{
    public class UserRes
    {
        public string Id { get; set; } = null!;

        public string FirstName { get; set; } = null!;

        public string LastName { get; set; } = null!;

        public string Email { get; set; } = null!;

        public string LoginId { get; set; } = null!;

        public string Authority { get; set; } = null!;

        public string AuthorityName { get; set; } = null!;

        public string Department { get; set; } = null!;

        public bool IsNotifyUpdateData { get; set; } = false;
    }
}
