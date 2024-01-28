using BeautyWeb.Model;
using Google.Protobuf;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using static System.Runtime.InteropServices.JavaScript.JSType;
using System;

namespace BeautyWeb.Pages
{
    public partial class POS
    {

        private List<InventoryItem> items = new List<InventoryItem>();
        private List<InventoryItem> checkList = new List<InventoryItem>();

        private List<InventoryItem> productlist = new List<InventoryItem>();
        private InventoryItem CurrentProduct = new InventoryItem();
        private string searchItem = null;
        private List<InventoryItem> filteredItems = new List<InventoryItem>();
        private TransactionItem newTransaction = new TransactionItem();
        private int quantity = 1;


        private bool Isadding = false; 
        private bool IsmodalOpen = false;
        private bool Ischeckoutavailable = false;

        private int? TotalAmount = 0;
        private int? PaidAmount = 0;
        private int? Discount = 0;
        private int? Status = 0;
        private int? ReturnedAmount = 0;

        protected override async Task OnInitializedAsync()
        {
            TotalAmount = 0;
            items = await JS.InvokeAsync<List<InventoryItem>>("getInventory");
        }

        private async Task AddProductToCheckList()
        {
            CurrentProduct = filteredItems[0];
            CurrentProduct.BuyingQuantity = quantity;
            CurrentProduct.TotalPrice = (CurrentProduct.sellingPrice * quantity);
            TotalAmount += CurrentProduct.TotalPrice;
            checkList.Add(CurrentProduct);
            filteredItems.Clear();
        }

        private async Task CheckOut()
        {
            TotalAmount = 0;
            foreach (var list in checkList)
            {
                TotalAmount += list.TotalPrice;
                UpdateProduct(list);
            }

            TotalAmount -= Discount;

            ToastService.ShowSuccess("Product updated in inventory successfully!");

            AddTransactionToDb(newTransaction, checkList);

            searchItem = null;
            filteredItems.Clear();
            productlist.Clear();
            checkList.Clear();
            IsmodalOpen = false;
        }

        private async Task CalculateReturn()
        {
            ReturnedAmount = PaidAmount - TotalAmount + Discount;
            Ischeckoutavailable = true;
        }

        private async Task PreCheckOut()
        {
            IsmodalOpen = true;
        }

        private async Task CancelCheckout()
        {
            IsmodalOpen = false;
            Ischeckoutavailable = false;
            PaidAmount = 0;
            Discount = 0;
            ReturnedAmount = 0;

        }

        private async Task CloseAddProductModal()
        {
            IsmodalOpen = false;
        }

        private void UpdateProduct(InventoryItem item)
        {
            var newProduct = new InventoryItem
            {
                Id = item.Id,
                productName = item.productName,
                Brand = item.Brand,
                Quantity = item.Quantity - item.BuyingQuantity,
                netPrice = item.netPrice,
                sellingPrice = item.sellingPrice
            };

            UpdateInventoryItem(newProduct);
        }

        private async void UpdateInventoryItem(InventoryItem item)
        {
            var validationErrors = ValidateProduct(item);
            if (item.Quantity == null)
            {
                item.Quantity = 0;
            }

            if (item.netPrice == null)
            {
                item.netPrice = 0;
            }

            if (item.sellingPrice == null)
            {
                item.sellingPrice = 0;
            }
            if (validationErrors.Count == 0)
            {
                await JS.InvokeVoidAsync("editInventory", item);
                items = await JS.InvokeAsync<List<InventoryItem>>("getInventory");
                CloseAddProductModal();
                StateHasChanged();
            }
            else
            {
                foreach (var error in validationErrors)
                {
                    ToastService.ShowError(error);
                }
            }
        }

        List<string> ValidateProduct(InventoryItem product)
        {
            List<string> validationErrors = new List<string>();

            if (string.IsNullOrWhiteSpace(product.productName))
            {
                validationErrors.Add("Product Name is required.");
            }

            if (string.IsNullOrWhiteSpace(product.Brand))
            {
                validationErrors.Add("Brand is required.");
            }

            return validationErrors;
        }

        List<string> ValidateTransaction(TransactionItem TransactionItem)
        {
            List<string> validationErrors = new List<string>();

            return validationErrors;
        }

        private void HandleSearchInput(ChangeEventArgs args)
        {
            searchItem = args.Value.ToString();
            UpdateFilteredItems();
        }
        private void UpdateFilteredItems()
        {
            filteredItems = items
                .Where(product => string.IsNullOrEmpty(searchItem) ||
                                   product.productName.Contains(searchItem, StringComparison.OrdinalIgnoreCase) ||
                                   product.Brand.Contains(searchItem, StringComparison.OrdinalIgnoreCase))
                .ToList();
            StateHasChanged();
        }

        private async void AddTransactionToDb(TransactionItem newTransaction, List<InventoryItem> checkList)
        {
            var validationErrors = ValidateTransaction(newTransaction);
            if (newTransaction.Status == null)
            {
                newTransaction.Status = 0;
            }

            if (newTransaction.TotalAmount == null)
            {
                newTransaction.TotalAmount = 0;
            }
            if (validationErrors.Count == 0)
            {
                TransactionItem transactionData = new TransactionItem();
                transactionData.Status = Status;
                transactionData.Discount = Discount;
                transactionData.TotalAmount = TotalAmount;
                transactionData.productItems = checkList;

                foreach (var list in transactionData.productItems)
                {
                    list.Quantity = list.BuyingQuantity;
                }
                

                await JS.InvokeVoidAsync("insertTransaction", transactionData);
                ToastService.ShowSuccess("Transaction added successfully!");
                StateHasChanged();

            }
            else
            {
                foreach (var error in validationErrors)
                {
                    ToastService.ShowError(error);
                }
            }
        }
    }
}
