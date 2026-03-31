using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.Repositories.Interfaces;

public interface IStatesRepository
{
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<IEnumerable<State>>> GetAsync(PaginationDTO pagination);

    Task<ActionResponse<IEnumerable<State>>> GetAsync();

    Task<ActionResponse<State>> GetAsync(int id);

    Task<IEnumerable<State>> GetComboAsync(int countryId);
}