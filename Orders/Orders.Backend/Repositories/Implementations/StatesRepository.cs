using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.Repositories.Interfaces;
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