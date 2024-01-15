﻿using BeautyWeb.Model;
using Microsoft.JSInterop;

namespace BeautyWeb.Pages
{
    public partial class ProductInventory
    {
        private List<InventoryItem> items = new List<InventoryItem>();
        private string modalStyle = "display:none;";
        private bool IsAddProduct = true;
        private InventoryItem newProduct = new InventoryItem();
        protected override async Task OnInitializedAsync()
        {
            items = await JS.InvokeAsync<List<InventoryItem>>("getInventory");
        }

        private void EditProduct(InventoryItem item)
        {
            newProduct = new InventoryItem
            {
                Id = item.Id,
                productName = item.productName,
                Brand = item.Brand,
                Type = item.Type,
                Quantity = item.Quantity,
                Price = item.Price,
                Discount = item.Discount
            };
            IsAddProduct = false;
            modalStyle = "display:block;";
        }
        private async void DeleteProduct(InventoryItem item)
        {
            await JS.InvokeVoidAsync("deleteInventory", item.Id);
            ToastService.ShowSuccess("Product removed from inventory!");
            items = await JS.InvokeAsync<List<InventoryItem>>("getInventory");
            StateHasChanged();
        }
        private void AddProduct()
        {
            modalStyle = "display:block;";
            IsAddProduct = true;
        }
        private void CloseAddProductModal()
        {
            modalStyle = "display:none;";
            newProduct = new InventoryItem();
        }
        private async void AddProductToDb()
        {
            var validationErrors = ValidateProduct(newProduct);
            if (newProduct.Quantity == null)
            {
                newProduct.Quantity = 0;
            }

            if (newProduct.Price == null)
            {
                newProduct.Price = 0;
            }

            if (newProduct.Discount == null)
            {
                newProduct.Discount = 0;
            }
            if (validationErrors.Count == 0)
            {
                await JS.InvokeVoidAsync("addInventory", newProduct);
                ToastService.ShowSuccess("Product added successfully!");
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

            if (string.IsNullOrWhiteSpace(product.Type))
            {
                validationErrors.Add("Type is required.");
            }
            return validationErrors;
        }
        private async void UpdateInventoryItem()
        {
            var validationErrors = ValidateProduct(newProduct);
            if (newProduct.Quantity == null)
            {
                newProduct.Quantity = 0;
            }

            if (newProduct.Price == null)
            {
                newProduct.Price = 0;
            }

            if (newProduct.Discount == null)
            {
                newProduct.Discount = 0;
            }
            if (validationErrors.Count == 0)
            {
                await JS.InvokeVoidAsync("editInventory", newProduct);
                ToastService.ShowSuccess("Product updated in inventory successfully!");
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
    }
}