namespace BeautyWeb.Model
{
    public class InventoryItem
    {
        public string Id { get; set; }
        public string productName {  get; set; }
        public int? Quantity { get; set; } = null;
        public int? netPrice { get; set; } = null;
        public int? sellingPrice { get; set; } = null;
        public string Brand { get; set; }
    }
}
