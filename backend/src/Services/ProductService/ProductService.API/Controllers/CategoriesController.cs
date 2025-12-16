using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs;
using ProductService.Application.Features.Categories.Commands.CreateCategory;
using ProductService.Application.Features.Categories.Commands.UpdateCategory;
using ProductService.Application.Features.Categories.Commands.DeleteCategory;
using ProductService.Application.Features.Categories.Queries.GetAllCategories;
using ProductService.Application.Features.Categories.Queries.GetCategoryById;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CategoriesController : ControllerBase
{
    private readonly IMediator _mediator;

    public CategoriesController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<CategoryDto>>> GetAll()
    {
        var query = new GetAllCategoriesQuery();
        var categories = await _mediator.Send(query);
        return Ok(categories);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<CategoryDto>> GetById(Guid id)
    {
        var query = new GetCategoryByIdQuery(id);
        var category = await _mediator.Send(query);

        if (category == null)
            return NotFound(new { message = "Category not found" });

        return Ok(category);
    }

    [HttpPost]
    public async Task<ActionResult<CategoryDto>> Create([FromBody] CreateCategoryDto dto)
    {
        var command = new CreateCategoryCommand(dto.Name, dto.Description);
        var category = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = category.Id }, category);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateCategoryDto dto)
    {
        var command = new UpdateCategoryCommand(id, dto.Name, dto.Description, dto.IsActive);
        var updated = await _mediator.Send(command);

        if (!updated)
            return NotFound(new { message = "Category not found" });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteCategoryCommand(id);
        var deleted = await _mediator.Send(command);

        if (!deleted)
            return NotFound(new { message = "Category not found" });

        return NoContent();
    }
}
