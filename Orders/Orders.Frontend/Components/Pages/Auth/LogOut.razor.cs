using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Services;

namespace Orders.Frontend.Components.Pages.Auth;

public partial class LogOut
{
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private ILoginService LoginService { get; set; } = null!;
    [CascadingParameter] private IMudDialogInstance MudDialog { get; set; } = null!;

    private async Task LogOutActionAsync()
    {
        await LoginService.LogOutAsync();
        CancelAction();
    }

    private void CancelAction()
    {
        MudDialog.Cancel();
    }
}