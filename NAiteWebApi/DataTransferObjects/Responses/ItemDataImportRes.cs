namespace NAiteWebApi.DataTransferObjects.Responses
{
    public class ItemDataImportRes
    {
        public string Id { get; set; } = null!;

        public string? UserId { get; set; } = null!;

        public string? UserName { get; set; } = null!;

        public bool IsHeader { get; set; }

        public string FileType { get; set; } = null!;

        public string OriginalFileName { get; set; } = null!;

        public int? Number { get; set; }

        public DateTime? Reserved { get; set; }

        public DateTime? Imported { get; set; }
    }

    public class ItemDataImportCreateRes
    {
        public string Id { get; set; } = null!;

        public string? FileType { get; set; } = null!;

        public List<string[]> Rows { get; set; } = new List<string[]>();
    }

    public class ItemDataImportFieldRes
    {
        public string Tag { get; set; } = null!;
    }
}
