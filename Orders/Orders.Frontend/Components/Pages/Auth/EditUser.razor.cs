using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components;
using MudBlazor;
using Orders.Frontend.Repositories;
using Orders.Frontend.Services;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using System.Net;

namespace Orders.Frontend.Components.Pages.Auth;

[Authorize]
public partial class EditUser
{
    private User? user;
    private List<Country>? countries;
    private List<State>? states;
    private List<City>? cities;
    private bool loading = true;
    private string? imageUrl;

    private Country selectedCountry = new();
    private State selectedState = new();
    private City selectedCity = new();

    [Inject] private NavigationManager NavigationManager { get; set; } = null!;
    [Inject] private IDialogService DialogService { get; set; } = null!;
    [Inject] private ISnackbar SnackBar { get; set; } = null!;
    [Inject] private IRepository Repository { get; set; } = null!;
    [Inject] private ILoginService LoginService { get; set; } = null!;

    protected override async Task OnInitializedAsync()
    {
        await LoadUserAsync();
        await LoadCountriesAsync();
        await LoadStatesAsync(user!.City!.State!.Country!.Id);
        await LoadCitiesAsync(user.City.State.Id);

        selectedCountry = user.City.State.Country;
        selectedState = user.City.State;
        selectedCity = user.City;

        if (!string.IsNullOrEmpty(user.Photo))
        {
            imageUrl = user.Photo;
            user.Photo = null;
        }
    }

    private async Task LoadUserAsync()
    {
        var responseHttp = await Repository.GetAsync<User>("/api/accounts");

        if (responseHttp.Error)
        {
            if (responseHttp.HttpResponseMessage.StatusCode == HttpStatusCode.NotFound)
            {
                NavigationManager.NavigateTo("/");
                return;
            }

            var messageError = await responseHttp.GetErrorMessageAsync();
            SnackBar.Add(messageError!, Severity.Error);
            return;
        }

        user = responseHttp.Response;
        loading = false;
    }

    private void ShowModal()
    {
        var closeOnEscapeKey = new DialogOptions { CloseOnEscapeKey = true };
        DialogService.Show<ChangePassword>("Cambiar contraseña", closeOnEscapeKey);
    }

    private void ImageSelected(string imageBase64)
    {
        user!.Photo = imageBase64;
        imageUrl = null;
    }

    private async Task SaveUserAsync()
    {
        var responseHttp = await Repository.PutAsync<User, TokenDTO>("/api/accounts", user!);

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            SnackBar.Add(message!, Severity.Error);
            return;
        }

        await LoginService.LoginAsync(responseHttp.Response!.Token);
        SnackBar.Add("Datos modificados con éxito.", Severity.Success);
        NavigationManager.NavigateTo("/");
    }

    private void ReturnAction()
    {
        NavigationManager.NavigateTo("/");
    }

    private void InvalidForm()
    {
        SnackBar.Add("Rellene todos los campos del formulario", Severity.Warning);
    }

    private async Task LoadCountriesAsync()
    {
        var responseHttp = await Repository.GetAsync<List<Country>>("/api/countries/combo");
        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            SnackBar.Add(message!, Severity.Error);
            return;
        }

        countries = responseHttp.Response;
    }

    private async Task LoadStatesAsync(int countryId)
    {
        var responseHttp = await Repository.GetAsync<List<State>>($"/api/states/combo/{countryId}");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            SnackBar.Add(message!, Severity.Error);
            return;
        }

        states = responseHttp.Response;
    }

    private async Task LoadCitiesAsync(int stateId)
    {
        var responseHttp = await Repository.GetAsync<List<City>>($"/api/cities/combo/{stateId}");

        if (responseHttp.Error)
        {
            var message = await responseHttp.GetErrorMessageAsync();
            SnackBar.Add(message!, Severity.Error);
            return;
        }

        cities = responseHttp.Response;
    }

    private async Task CountryChangedAsync(Country country)
    {
        selectedCountry = country;
        selectedState = new State();
        selectedCity = new City();
        states = null;
        cities = null;
        await LoadStatesAsync(country.Id);
    }

    private async Task StateChangedAsync(State state)
    {
        selectedState = state;
        selectedCity = new City();
        cities = null;
        await LoadCitiesAsync(state.Id);
    }

    private async Task CityChanged(City city)
    {
        selectedCity = city;
        user!.CityId = city.Id;
    }

    private async Task<IEnumerable<Country>> SearchCountries(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText)) return countries!;
        return countries!.Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
    }

    private async Task<IEnumerable<State>> SearchStates(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText)) return states!;
        return states!.Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
    }

    private async Task<IEnumerable<City>> SearchCities(string searchText, CancellationToken token)
    {
        await Task.Delay(5);
        if (string.IsNullOrWhiteSpace(searchText)) return cities!;
        return cities!.Where(c => c.Name.Contains(searchText, StringComparison.InvariantCultureIgnoreCase)).ToList();
    }
}