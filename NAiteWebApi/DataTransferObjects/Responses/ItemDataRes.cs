namespace NAiteWebApi.DataTransferObjects.Responses
{
    public class ItemDataRes
    {
        public DateTime Date { get; set; }

        public int Quantity { get; set; }
    }

    public class GraphData
    {
        public string Id { get; set; } = null!;
        public int[] Data { get; set; } = null!;
    }

    public class GraphRes
    {
        public string[] GraphLabels { get; set; } = null!;
        public List<GraphData> GraphDatas { get; set; } = null!;
        public int? StockThreshold { get; set; }
        public string? InventoryDate { get; set; } = null!;
        public string? Today {  get; set; } = null!;
        public int? MaxValue { get; set; }
        public int? MinValue { get; set; }
    }
}
