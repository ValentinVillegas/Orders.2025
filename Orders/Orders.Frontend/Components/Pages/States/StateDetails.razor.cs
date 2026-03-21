using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Components.Pages.Cities;
using Orders.Frontend.Components.Shared;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;

namespace Orders.Frontend.Components.Pages.States;

public partial class StateDetails
{
    private State? state;
    private List<City>? cities;
    private MudTable<City> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/cities";
    private string infoFormat = "{first_item}-{lst_item} de {all_items}";

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    [Parameter] public int StateId { get; set; }
    [Parameter] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadTotalRecordsAsync();
    }

    private async Task<bool> LoadStateAsync()
    {
        var responseHttp = await Repository.GetAsync<State>($"api/states/{StateId}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == System.Net.HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/countries");
            }
            else
            {
                var message = await responseHttp.GetErrorMessageAsync();
                Snackbar.Add(message!, Severity.Error);
            }

            return false;
        }

        state = responseHttp.Response;
        return true;
    }

    private async Task LoadTotalRecordsAsync()
    {
        loading = true;

        if (state is null)
        {
            var stateLoaded = await LoadStateAsync();

            if (!stateLoaded)
            {
                NavigationManager.NavigateTo("/countries");
                return;
            }
        }

        var url = $"{baseUrl}/totalrecords?id={StateId}" + (!string.IsNullOrEmpty(Filter) ? $"&filter={Filter}" : "");

        var responseHttp = await Repository.GetAsync<int>(url);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        totalRecords = responseHttp.Response;
        loading = false;
    }

    private async Task<TableData<City>> LoadListAsync(TableState state, CancellationToken cancellationToken)
    {
        int page = state.Page + 1;
        int pageSize = state.PageSize;
        var url = $"{baseUrl}/paginated?id={StateId}&page={page}&recordsnumber{pageSize}" + (!string.IsNullOrEmpty(Filter) ? $"&filter={Filter}" : "");

        var responseHttp = await Repository.GetAsync<List<City>>(url);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return new TableData<City> { Items = [], TotalItems = 0 };
        }

        if (responseHttp.Response is null) return new TableData<City> { Items = [], TotalItems = 0 };

        return new TableData<City> { Items = responseHttp.Response, TotalItems = totalRecords };
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
    }

    private async Task ShowModalAsync(int id = 0, bool isEdit = false)
    {
        var options = new DialogOptions { CloseButton = true, CloseOnEscapeKey = true };

        IDialogReference? dialog;

        if (isEdit)
        {
            var parameters = new DialogParameters { { "Id", id } };
            dialog = await DialogService.ShowAsync<CityEdit>("Editar Ciudad", parameters, options);
        }
        else
        {
            var parameters = new DialogParameters { { "StateId", StateId } };
            dialog = await DialogService.ShowAsync<CityCreate>("Crear Ciudad", parameters, options);
        }

        var result = await dialog.Result;

        if (result!.Canceled!)
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
    }

    private void ReturnAction()
    {
        NavigationManager.NavigateTo($"/countries/details/{state!.CountryId}");
    }

    private async Task DeleteAsync(City city)
    {
        var parameters = new DialogParameters { { "Message", $"¿Estás seguro de que quieres eliminar la ciudad {city.Name}?" } };
        var options = new DialogOptions { CloseButton = true, CloseOnEscapeKey = true, MaxWidth = MaxWidth.ExtraSmall };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);

        var result = await dialog.Result;

        if (result!.Canceled) return;

        var responseHttp = await Repository.DeleteAsync($"api/cities/{city.Id}");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }

        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
        Snackbar.Add("Registro eliminado correctamente.", Severity.Success);
    }
}