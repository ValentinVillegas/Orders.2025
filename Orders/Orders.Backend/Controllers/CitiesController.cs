using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Backend.UnitsOfWork.Implementations;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CitiesController : GenericController<City>
{
    private readonly ICitiesUnitOfWork _citiesUnitOfWork;

    public CitiesController(IGenericUnitOfWork<City> unitOfWork, ICitiesUnitOfWork citiesUnitOfWork) : base(unitOfWork)
    {
        _citiesUnitOfWork = citiesUnitOfWork;
    }

    [HttpGet("totalRecords")]
    public override async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _citiesUnitOfWork.GetTotalRecordsAsync(pagination);
        if (response.WasSucces) return Ok(response.Result);
        return BadRequest();
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var response = await _citiesUnitOfWork.GetAsync(pagination);
        if (response.WasSucces) return Ok(response.Result);
        return BadRequest();
    }

    [AllowAnonymous]
    [HttpGet("combo/{stateId:int}")]
    public async Task<IActionResult> GetActionAsync(int stateId)
    {
        return Ok(await _citiesUnitOfWork.GetComboAsync(stateId));
    }
}