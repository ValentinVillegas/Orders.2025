using Orders.Shared.Entities;

namespace Orders.Backend.Data;

public class SeedDB
{
    private readonly DataContext _context;

    public SeedDB(DataContext context)
    {
        _context = context;
    }

    public async Task SeedDBAsync()
    {
        await _context.Database.EnsureCreatedAsync(); //Crea la base de datos
        await CheckCountriesAsync(); //Agrega paises a la base de datos
        await CheckCategoriesAsync(); //Agrega categorias ala base de datos
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
            _context.Categories.Add(new Category { Name = "Calzado" });
            _context.Categories.Add(new Category { Name = "Tecnología" });

            await _context.SaveChangesAsync();
        }
    }
}