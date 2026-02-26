using Orders.Backend.Repositories.Interfaces;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Implementations;

public class StatesUnitOfWork : IStatesUnitOfWork
{
    private readonly IStatesRepository _statesRepository;

    public StatesUnitOfWork(IStatesRepository statesRepository)
    {
        _statesRepository = statesRepository;
    }

    public async Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination) => await _statesRepository.GetTotalRecordsAsync(pagination);

    public async Task<ActionResponse<IEnumerable<State>>> GetAsync(PaginationDTO pagination) => await (_statesRepository.GetAsync(pagination));

    public async Task<ActionResponse<State>> GetAsync(int id) => await _statesRepository.GetAsync(id);

    public async Task<ActionResponse<IEnumerable<State>>> GetAsync() => await _statesRepository.GetAsync();
}