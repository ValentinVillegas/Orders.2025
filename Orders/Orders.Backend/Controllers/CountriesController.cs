using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Orders.Backend.Data;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Backend.Controllers;

[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Route("api/[controller]")]
public class CountriesController : GenericController<Country>
{
    private readonly ICountriesUnitOfWork _countriesUnitOfWork;

    public CountriesController(IGenericUnitOfWork<Country> unitOfWork, ICountriesUnitOfWork countriesUnitOfWork) : base(unitOfWork)
    {
        _countriesUnitOfWork = countriesUnitOfWork;
    }

    [HttpGet("totalRecords")]
    public override async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _countriesUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSucces) return Ok(action.Result);
        return BadRequest(action.Message);
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync(PaginationDTO pagination)
    {
        var action = await _countriesUnitOfWork.GetAsync(pagination);
        if (action.WasSucces) return Ok(action.Result);
        return BadRequest(action.Message);
    }

    [HttpGet]
    public override async Task<IActionResult> GetAsync()
    {
        var action = await _countriesUnitOfWork.GetAsync();
        if (action.WasSucces) return Ok(action.Result);
        return BadRequest(action.Message);
    }

    [HttpGet("{id}")]
    public override async Task<IActionResult> GetAsync(int id)
    {
        var action = await _countriesUnitOfWork.GetAsync(id);
        if (action.WasSucces) return Ok(action.Result);
        return BadRequest(action.Message);
    }
}

//public class CountriesController : ControllerBase
//{
//    private readonly DataContext _context;

//    public CountriesController(DataContext context)
//    {
//        _context = context;
//    }

//    [HttpPost]
//    public async Task<IActionResult> PostAsync(Country country)
//    {
//        _context.Countries.Add(country);
//        await _context.SaveChangesAsync();

//        return Ok(country);
//    }

//    [HttpGet]
//    public async Task<IActionResult> GetAsync()
//    {
//        return Ok(await _context.Countries.ToListAsync());
//    }
//}