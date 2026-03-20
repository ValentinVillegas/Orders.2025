using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Components.Pages.States;
using Orders.Frontend.Components.Shared;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;
using System.Net;

namespace Orders.Frontend.Components.Pages.Countries;

public partial class CountryDetails
{
    private Country? country;
    private List<State>? states;
    private MudTable<State> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/states";
    private string infoFormat = "{first_item}-{lst_item} de {all_items}";

    [Parameter] public int CountryId { get; set; }
    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar Snackbar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManager { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await LoadTotalRecordsAsync();
    }

    private async Task<bool> LoadCountryAsync()
    {
        var responseHttp = await Repository.GetAsync<Country>($"api/countries/{CountryId}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
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

        country = responseHttp.Response;
        return true;
    }

    private async Task<bool> LoadTotalRecordsAsync()
    {
        loading = true;

        if (country is null)
        {
            var countryLoaded = await LoadCountryAsync();

            if (!countryLoaded)
            {
                NavigationManager.NavigateTo("/countries");
                return false;
            }
        }

        var url = $"{baseUrl}/totalRecords?id={CountryId}" + (!string.IsNullOrEmpty(Filter) ? $"&filter={Filter}" : "");

        var responseHttp = await Repository.GetAsync<int>(url);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return false;
        }

        totalRecords = responseHttp.Response;
        loading = false;
        return true;
    }

    private async Task<TableData<State>> LoadListAsync(TableState state, CancellationToken cancellationToken)
    {
        int page = state.Page + 1;
        int pageSize = state.PageSize;
        var url = $"{baseUrl}/paginated?id={CountryId}&page={page}&recordsnumber={pageSize}" + (!string.IsNullOrEmpty(Filter) ? $"&filter={Filter}" : "");

        var responseHttp = await Repository.GetAsync<List<State>>(url);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return new TableData<State> { Items = [], TotalItems = 0 };
        }

        if (responseHttp.Response is null) return new TableData<State> { Items = [], TotalItems = 0 };

        return new TableData<State> { Items = responseHttp.Response, TotalItems = totalRecords };
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
    }

    private void ReturnAction()
    {
        NavigationManager.NavigateTo("/countries");
    }

    private async Task ShowModalAsync(int id = 0, bool isEdit = false)
    {
        var options = new DialogOptions
        {
            CloseButton = true,
            CloseOnEscapeKey = true,
        };

        IDialogReference? dialog;

        if (isEdit)
        {
            var parameters = new DialogParameters
        {
            {"id", id }
        };

            dialog = await DialogService.ShowAsync<StateEdit>("Editar Estado", parameters, options);
        }
        else
        {
            var parameters = new DialogParameters
        {
            {"CountryId", CountryId }
        };

            dialog = await DialogService.ShowAsync<StateCreate>("Nuevo Estado", parameters, options);
        }

        var result = await dialog.Result;

        if (result!.Canceled!)
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
    }

    private async Task DeleteAsync(State state)
    {
        var parameters = new DialogParameters
        {
            { "Message", $"¿Estás seguro de que quieres eliminar el estado {state.Name}?" }
        };

        var options = new DialogOptions { CloseButton = true, MaxWidth = MaxWidth.ExtraSmall, CloseOnEscapeKey = true };
        var dialog = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);
        var result = await dialog.Result;

        if (result!.Canceled) return;

        var responseHttp = await Repository.DeleteAsync($"api/states/{state.Id}");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            Snackbar.Add(message!, Severity.Error);
            return;
        }
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
        Snackbar.Add("Estado eliminado.", Severity.Success);
    }

    private void ShowCities(State state)
    {
        NavigationManager.NavigateTo($"states/details/{state.Id}");
    }
}