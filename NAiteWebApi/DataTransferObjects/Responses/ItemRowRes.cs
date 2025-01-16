using NAiteEntities.Models;

namespace NAiteWebApi.DataTransferObjects.Responses
{
    public class ItemRowRes
    {
        public string Id { get; set; } = null!;

        public int DefaultOrder { get; set; }

        public string? Code { get; set; } = null!;

        public ItemRes[] Items { get; set; } = new ItemRes[0];
    }
}
