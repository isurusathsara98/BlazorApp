using BeautyWeb.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;

namespace BeautyWeb.Layout
{
    public partial class MainLayout
    {
        private string username;
        private string password;
        private string modalStyle = "display:none;";
        private string errorMessage = string.Empty;
        private async void Login()
        {
            if(username == string.Empty || password == string.Empty)
            {
                errorMessage = "Username and password must not be empty";
                StateHasChanged();
                return;
            }
            if (await adminService.AuthenticateAsync(username, password))
            {
                username = string.Empty;
                password = string.Empty;
                ToggleLogin(false);
                errorMessage = string.Empty;
                StateHasChanged();
            }
            else
            {
                username = string.Empty;
                password = string.Empty;
                errorMessage = "Error occured in Authentication";

                StateHasChanged();
            }
        }
        private void ToggleLogin(bool display)
        {
            if (display)
                modalStyle = "display:block;";
            else
                modalStyle = "display:none;";
        }
        
    }
}
