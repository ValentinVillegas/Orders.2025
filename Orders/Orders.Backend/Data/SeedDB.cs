using Microsoft.EntityFrameworkCore;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.Entities;
using Orders.Shared.Enums;

namespace Orders.Backend.Data;

public class SeedDB
{
    private readonly DataContext _context;
    private readonly IUsersUnitOfWork _usersUnitOfWork;

    public SeedDB(DataContext context, IUsersUnitOfWork usersUnitOfWork)
    {
        _context = context;
        _usersUnitOfWork = usersUnitOfWork;
    }

    public async Task SeedDBAsync()
    {
        await _context.Database.EnsureCreatedAsync(); //Crea la base de datos
        await CheckCountriesFullAsync(); //Agrega paises a la base de datos
        await CheckCountriesAsync(); //Agrega paises a la base de datos
        await CheckCategoriesAsync(); //Agrega categorias ala base de datos
        await CheckRolesAsync();
        //await CheckUserAsync("1010", "Juan", "Zuluaga", "zulu@yopmail.com", "322 311 4620", "Calle Luna Calle Sol", UserType.Admin);
        await CheckUserAsync("1010", "Valentín", "Villegas", "prodpan@yopmail.com", "8124756698", "Calle Luna Calle Sol", UserType.Admin);
    }

    public async Task CheckCountriesFullAsync()
    {
        if (!_context.Countries.Any())
        {
            var scriptSQL = File.ReadAllText("Data\\CountriesStatesCities.sql");
            await _context.Database.ExecuteSqlRawAsync(scriptSQL);
        }
    }

    public async Task CheckCountriesAsync()
    {
        if (!_context.Countries.Any())
        {
            _context.Countries.Add(new Country { Name = "Colombia" });
            _context.Countries.Add(new Country { Name = "Bolivia" });

            await _context.SaveChangesAsync();
        }
    }

    public async Task CheckCategoriesAsync()
    {
        if (!_context.Categories.Any())
        {
            _context.Categories.Add(new Category { Name = "Apple" });
            _context.Categories.Add(new Category { Name = "Autos" });
            _context.Categories.Add(new Category { Name = "Belleza" });
            _context.Categories.Add(new Category { Name = "Calzado" });
            _context.Categories.Add(new Category { Name = "Comida" });
            _context.Categories.Add(new Category { Name = "Cosmeticos" });
            _context.Categories.Add(new Category { Name = "Deportes" });
            _context.Categories.Add(new Category { Name = "Erótica" });
            _context.Categories.Add(new Category { Name = "Ferreteria" });
            _context.Categories.Add(new Category { Name = "Gamer" });
            _context.Categories.Add(new Category { Name = "Hogar" });
            _context.Categories.Add(new Category { Name = "Jardín" });
            _context.Categories.Add(new Category { Name = "Jugetes" });
            _context.Categories.Add(new Category { Name = "Lenceria" });
            _context.Categories.Add(new Category { Name = "Mascotas" });
            _context.Categories.Add(new Category { Name = "Nutrición" });
            _context.Categories.Add(new Category { Name = "Ropa" });
            _context.Categories.Add(new Category { Name = "Tecnología" });

            await _context.SaveChangesAsync();
        }
    }

    private async Task CheckRolesAsync()
    {
        await _usersUnitOfWork.CheckRoleAsync(UserType.Admin.ToString());
        await _usersUnitOfWork.CheckRoleAsync(UserType.User.ToString());
    }

    private async Task<User> CheckUserAsync(string document, string firstName, string lastName, string email, string phone, string address, UserType userType)
    {
        var user = await _usersUnitOfWork.GetUserAsync(email);

        if (user == null)
        {
            user = new User
            {
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                UserName = email,
                PhoneNumber = phone,
                Adress = address,
                Document = document,
                City = _context.Cities.FirstOrDefault(),
                UserType = userType
            };

            await _usersUnitOfWork.AddUserAsync(user, "123456");
            await _usersUnitOfWork.AddUserToRoleAsync(user, userType.ToString());
        }

        return user;
    }
}