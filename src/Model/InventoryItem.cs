namespace BeautyWeb.Model
{
    public class InventoryItem
    {
        public string productName {  get; set; }
        public int? Quantity { get; set; } = null;
        public int? Price { get; set; } = null;
        public string Type { get; set; }
        public int? Discount { get; set; } = null;
        public string Brand { get; set; }
    }
}
