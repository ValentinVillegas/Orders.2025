using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Repositories.Implementations;
using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Implementations;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.Entities;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.
    AddControllers().
    AddJsonOptions(x => x.JsonSerializerOptions.ReferenceHandler = ReferenceHandler.IgnoreCycles); //Ignora las redundancias cíclicas

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<DataContext>(x => x.UseSqlServer("name=LocalConnection"));
builder.Services.AddTransient<SeedDB>(); //Alimentador de base de datos

builder.Services.AddScoped(typeof(IGenericUnitOfWork<>), typeof(GenericUnitOfWork<>));
builder.Services.AddScoped(typeof(IGenericRepository<>), typeof(GenericRepository<>));
builder.Services.AddScoped(typeof(ICountriesUnitOfWork), typeof(CountriesUnitOfWork));
builder.Services.AddScoped(typeof(ICountriesRepository), typeof(CountriesRepository));
builder.Services.AddScoped(typeof(IStatesUnitOfWork), typeof(StatesUnitOfWork));
builder.Services.AddScoped(typeof(IStatesRepository), typeof(StatesRepository));
builder.Services.AddScoped(typeof(ICitiesUnitOfWork), typeof(CitiesUnitOfWork));
builder.Services.AddScoped(typeof(ICitiesRepository), typeof(CitiesRepository));
builder.Services.AddScoped(typeof(ICategoriesUnitOfWork), typeof(CategoriesUnitOfWork));
builder.Services.AddScoped(typeof(ICategoriesRepository), typeof(CategoriesRepository));
builder.Services.AddScoped(typeof(IUsersUnitOfWork), typeof(UsersUnitOfWork));
builder.Services.AddScoped(typeof(IUsersRepository), typeof(UsersRepository));

builder.Services.AddIdentity<User, IdentityRole>(x =>
{
    x.User.RequireUniqueEmail = true;
    x.Password.RequireDigit = false;
    x.Password.RequiredUniqueChars = 0;
    x.Password.RequireLowercase = false;
    x.Password.RequireNonAlphanumeric = false;
    x.Password.RequireUppercase = false;
})
    .AddEntityFrameworkStores<DataContext>()
    .AddDefaultTokenProviders();

var app = builder.Build();

SeedData(app);

void SeedData(WebApplication app)
{
    var scopedFactory = app.Services.GetService<IServiceScopeFactory>();

    using (var scope = scopedFactory!.CreateScope())
    {
        var service = scope.ServiceProvider.GetService<SeedDB>();
        service!.SeedDBAsync().Wait();
    }

    //using var scope = scopedFactory!.CreateScope();
    //var service = scope.ServiceProvider.GetService<SeedDB>();
    //service!.SeedDBAsync().Wait();
}

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();