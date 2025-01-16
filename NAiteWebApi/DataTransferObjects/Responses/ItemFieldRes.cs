namespace NAiteWebApi.DataTransferObjects.Responses
{
    public class ItemFieldRes
    {
        public string Id { get; set; } = null!;

        public string Name { get; set; } = null!;

        public string Type { get; set; } = null!;

        public string? FixedFieldType { get; set; } = null!;

        public bool IsSearch { get; set; }

        public int Order { get; set; } = 0;

        public string? ExcelColumnName { get; set; }
    }
}
