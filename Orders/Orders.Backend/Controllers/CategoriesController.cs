using Microsoft.AspNetCore.Mvc;
using Orders.Backend.UnitsOfWork.Interfaces;
using Orders.Shared.DTOs;
using Orders.Shared.Entities;

namespace Orders.Backend.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : GenericController<Category>
{
    private readonly ICategoriesUnitOfWork _categoriesUnitOfWork;

    public CategoriesController(IGenericUnitOfWork<Category> unitOfWork, ICategoriesUnitOfWork categoriesUnitOfWork) : base(unitOfWork)
    {
        _categoriesUnitOfWork = categoriesUnitOfWork;
    }

    [HttpGet("totalRecords")]
    public override async Task<IActionResult> GetTotalRecordsAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _categoriesUnitOfWork.GetTotalRecordsAsync(pagination);
        if (action.WasSucces) return Ok(action.Result);
        return BadRequest(action.Message);
    }

    [HttpGet("paginated")]
    public override async Task<IActionResult> GetAsync([FromQuery] PaginationDTO pagination)
    {
        var action = await _categoriesUnitOfWork.GetAsync(pagination);
        if (action.WasSucces) return Ok(action.Result);
        return BadRequest(action.Message);
    }
}