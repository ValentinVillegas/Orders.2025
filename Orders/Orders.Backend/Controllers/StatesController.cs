using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Orders.Backend.UnitsOfWork.Implementations;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class StatesController : GenericController<State>
{
    private readonly IStatesUnitOfWork _statesUnitOfWoork;

    public StatesController(IGenericUnitOfWork<State> unitOfWork, IStatesUnitOfWork statesUnitOfWoork) : base(unitOfWork)
    {
        _statesUnitOfWoork = statesUnitOfWoork;
    }

    [HttpGet("totalRecords")]
    public override async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _statesUnitOfWoork.GetTotalRecordsAsync(pagination);
        if (action.WasSucces) return Ok(action.Result);
        return BadRequest(action.Message);
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _statesUnitOfWoork.GetAsync(pagination);
        if (action.WasSucces) return Ok(action.Result);
        return BadRequest(action.Message);
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync()
    {
        var action = await _statesUnitOfWoork.GetAsync();
        if (action.WasSucces) return Ok(action.Result);
        return BadRequest(action.Message);
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var action = await _statesUnitOfWoork.GetAsync(id);
        if (action.WasSucces) return Ok(action.Result);
        return BadRequest(action.Message);
    }

    [AllowAnonymous]
    [HttpGet("combo/{countryId:int}")]
    public async Task<IActionResult> GetActionAsync(int countryId)
    {
        return Ok(await _statesUnitOfWoork.GetComboAsync(countryId));
    }
}