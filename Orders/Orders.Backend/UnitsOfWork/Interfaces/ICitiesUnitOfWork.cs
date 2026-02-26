using Orders.Shared.DTOs;
using Orders.Shared.Entities;
using Orders.Shared.Responses;

namespace Orders.Backend.UnitsOfWork.Interfaces;

public interface ICitiesUnitOfWork
{
    Task<ActionResponse<int>> GetTotalRecordsAsync(PaginationDTO pagination);

    Task<ActionResponse<IEnumerable<City>>> GetAsync(PaginationDTO pagination);
}