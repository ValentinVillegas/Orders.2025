using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Helpers;
using Orders.Backend.Repositories.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Implementations;

public class StatesRepository : GenericRepository<State>, IStatesRepository
{
    private readonly DataContext _context;

    public StatesRepository(DataContext context) : base(context)
    {
        _context = context;
    }

    public override async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination)
    {
        var queryable = _context.States.Where(x => x.CountryId == pagination.Id).AsQueryable();
        double count = await queryable.CountAsync();
        return new ActionResponse<int> { WasSucces = true, Result = (int)count };
    }

    public override async Task<ActionResponse<IEnumerable<State>>> GetAsync(PaginationDTO pagination)
    {
        var queryable = _context.States.Include(x => x.Cities).Where(x => x.CountryId == pagination.Id).AsQueryable();
        return new ActionResponse<IEnumerable<State>>
        {
            WasSucces = true,
            Result = await queryable.OrderBy(x => x.Name)
                                    .Paginate(pagination)
                                    .ToListAsync()
        };
    }

    public override async Task<ActionResponse<State>> GetAsync(int id)
    {
        var state = await _context.States.Include(x => x.Cities).FirstOrDefaultAsync(x => x.Id == id);
        if (state is null) return new ActionResponse<State> { WasSucces = false, Message = "Registro no encontrado" };
        return new ActionResponse<State> { WasSucces = true, Result = state };
    }

    public override async Task<ActionResponse<IEnumerable<State>>> GetAsync()
    {
        var states = await _context.States.Include(x => x.Cities).ToListAsync();
        return new ActionResponse<IEnumerable<State>> { WasSucces = true, Result = states };
    }
}