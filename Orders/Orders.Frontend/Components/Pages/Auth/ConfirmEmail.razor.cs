using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Repositories;

namespace Orders.Frontend.Components.Pages.Auth;

public partial class ConfirmEmail
{
    private string? message;

    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public IDialogService DialogService { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IRepository Repository { get; set; } = null!;

    [Parameter, SupplyParameterFromQuery] public string UserId { get; set; } = string.Empty;
    [Parameter, SupplyParameterFromQuery] public string Token { get; set; } = string.Empty;

    protected async Task ConfirmAccountAsync()
    {
        var responseHttp = await Repository.GetAsync($"/api/accounts/ConfirmEmail/?userId={UserId}&token={Token}");

        if (responseHttp.Error)
        {
            message = await responseHttp.GetErrorMessageAsync();
            NavigationManager.NavigateTo("/");
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        Snackbar.Add("Su cuenta ha sido confirmada correctamente. Ahora puede ingresar con sus credenciales.", Severity.Success);
        var closeOnEscapeKey = new DialogOptions { CloseOnEscapeKey = true };
        DialogService.Show<LogIn>("Inicio de Sesión", closeOnEscapeKey);
    }
}