using Microsoft.AspNetCore.Components;
using MudBlazor;

namespace Orders.Frontend.Components.Shared;

public partial class ConfirmDialog
{
    [CascadingParameter] public IMudDialogInstance MudDialog { get; set; } = null!;
    [Parameter] public string Message { get; set; } = null!;

    private void Aceptar()
    {
        MudDialog.Close(DialogResult.Ok(true));
    }

    private void Cancelar()
    {
        MudDialog.Close(DialogResult.Cancel());
    }
}