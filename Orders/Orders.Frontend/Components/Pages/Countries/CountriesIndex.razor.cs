using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Components.Shared;
using Orders.Frontend.Repositories;
using Orders.Shared.Entities;
using System.Net;

namespace Orders.Frontend.Components.Pages.Countries;

public partial class CountriesIndex
{
    private List<Country>? countries;
    private MudTable<Country> table = new();
    private readonly int[] pageSizeOptions = { 10, 25, 50, int.MaxValue };
    private int totalRecords = 0;
    private bool loading;
    private const string baseUrl = "api/countries";
    private string infoFormat = "{first_item}-{last_item}=>{all_items}";

    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar SnackBar { get; set; } = null!;
    [Inject] private NavigationManager NavigationManaager { get; set; } = null!;
    [Parameter, SupplyParameterFromQuery] public string Filter { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        await LoadTotalRecordsAsync();
    }

    private async Task LoadTotalRecordsAsync()
    {
        loading = true;
        var url = $"{baseUrl}/totalRecords" + (!(string.IsNullOrWhiteSpace(Filter)) ? $"?filter={Filter}" : "");

        var responseHttp = await Repository.GetAsync<int>(url);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            SnackBar.Add(message!, Severity.Error);
            return;
        }

        totalRecords = responseHttp.Response;
        loading = false;
    }

    private async Task<TableData<Country>> LoadListAsync(TableState state, CancellationToken cancellationToken)
    {
        int page = state.Page + 1;
        int pageSize = state.PageSize;
        var url = $"{baseUrl}/paginated?page={page}&recordsnumber={pageSize}" + (!(string.IsNullOrWhiteSpace(Filter)) ? $"&filter={Filter}" : "");

        var responseHttp = await Repository.GetAsync<List<Country>>(url);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            SnackBar.Add(message!, Severity.Error);
            return new TableData<Country>
            {
                Items = [],
                TotalItems = 0
            };
        }

        if (responseHttp.Response == null)
        {
            return new TableData<Country>
            {
                Items = [],
                TotalItems = 0
            };
        }

        return new TableData<Country>
        {
            Items = responseHttp.Response,
            TotalItems = totalRecords
        };
    }

    private async Task SetFilterValue(string value)
    {
        Filter = value;
        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
    }

    private async Task ShowModalAsync(int id = 0, bool isEdit = false)
    {
        var options = new DialogOptions { CloseOnEscapeKey = true, CloseButton = true };

        IDialogReference? dialog;

        if (isEdit)
        {
            var parameters = new DialogParameters
            {
                { "Id", id }
            };

            dialog = await DialogService.ShowAsync<CountryEdit>("Editar país", parameters, options);
        }
        else
        {
            dialog = await DialogService.ShowAsync<CountryCreate>("Nuevo país", options);
        }

        var result = await dialog.Result;

        if (result!.Canceled)
        {
            await LoadTotalRecordsAsync();
            await table.ReloadServerData();
        }
    }

    private async Task DeleteAsync(Country country)
    {
        var parameters = new DialogParameters
        {
            { "Message", $"Estás seguro de borrar el país {country.Name}"}
        };

        var options = new DialogOptions
        {
            CloseButton = true,
            MaxWidth = MaxWidth.ExtraSmall,
            CloseOnEscapeKey = true
        };

        var dialogo = await DialogService.ShowAsync<ConfirmDialog>("Confirmación", parameters, options);

        var result = await dialogo.Result;

        if (result!.Canceled) return;

        var responseHttp = await Repository.DeleteAsync($"{baseUrl}/{country.Id}");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManaager.NavigateTo("/countries");
            }
            else
            {
                var message = await responseHttp.GetErrorMessageAsync();
                SnackBar.Add(message!, Severity.Error);
            }

            return;
        }

        await LoadTotalRecordsAsync();
        await table.ReloadServerData();
        SnackBar.Add("Registro borrado.", Severity.Success);
    }
}