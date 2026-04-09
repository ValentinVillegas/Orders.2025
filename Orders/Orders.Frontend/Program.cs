using Microsoft.AspNetCore.Components.Authorization;
using MudBlazor.Services;
using Orders.Frontend.AuthenticationProviders;
using Orders.Frontend.Components;
using Orders.Frontend.Repositories;
using Orders.Frontend.Services;

var builder = WebApplication.CreateBuilder(args);

// Add MudBlazor services
builder.Services.AddMudServices();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

//builder.Services.AddScoped(_ => new HttpClient { BaseAddress = new Uri("https://localhost:7277") });
builder.Services.AddSingleton(_ => new HttpClient { BaseAddress = new Uri("https://localhost:7277") });
//builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProviderTest>();
builder.Services.AddScoped<IRepository, Repository>();

builder.Services.AddAuthorizationCore();
builder.Services.AddScoped<AuthenticationProviderJWT>();
builder.Services.AddScoped<AuthenticationStateProvider, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());
builder.Services.AddScoped<ILoginService, AuthenticationProviderJWT>(x => x.GetRequiredService<AuthenticationProviderJWT>());

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();