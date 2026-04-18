using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Repositories;
using Orders.Shared.DTOs;

namespace Orders.Frontend.Components.Pages.Auth;

public partial class ResendConfirmationEmailToken
{
    private EmailDTO emailDTO = new();
    private bool loading;

    [Inject] public NavigationManager NavigationManager { get; set; } = null!;
    [Inject] public ISnackbar Snackbar { get; set; } = null!;
    [Inject] public IRepository Repository { get; set; } = null!;
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    private async Task ResendEmailConfirmationTokenAsync()
    {
        loading = true;
        var responseHttp = await Repository.PostAsync("/api/accounts/ResendToken", emailDTO);
        loading = false;

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        MudDialog.Cancel();
        NavigationManager.NavigateTo("/");
        Snackbar.Add("Se ha enviado un correo electrónico con las instrucciones para activar tu usuario.", Severity.Success);
    }
}