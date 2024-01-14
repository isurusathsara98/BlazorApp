using BeautyWeb.Model;
using Microsoft.JSInterop;

namespace BeautyWeb.Pages
{
    public partial class ProductInventory
    {
        private List<InventoryItem> items = new List<InventoryItem>();
        private string modalStyle = "display:none;";
        private InventoryItem newProduct = new InventoryItem();
        protected override async Task OnInitializedAsync()
        {
            items = await JS.InvokeAsync<List<InventoryItem>>("getInventory");
        }

        private void EditProduct(InventoryItem item) 
        {
        }
        private void DeleteProduct(InventoryItem item) 
        { 
        }
        private void AddProduct() 
        {
            modalStyle = "display:block;";
        }
        private void CloseAddProductModal()
        {
            modalStyle = "display:none;";
            newProduct = new InventoryItem();
        }
        private void AddProductToDb()
        {
            
        }
    }
}
