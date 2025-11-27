using MediatR;
using Microsoft.AspNetCore.Mvc;
using ProductService.Application.DTOs;
using ProductService.Application.Features.Products.Commands.CreateProduct;
using ProductService.Application.Features.Products.Commands.UpdateProduct;
using ProductService.Application.Features.Products.Commands.DeleteProduct;
using ProductService.Application.Features.Products.Queries.GetAllProducts;
using ProductService.Application.Features.Products.Queries.GetProductById;
using ProductService.Application.Features.Products.Queries.GetProductsByCategory;

namespace ProductService.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ProductsController : ControllerBase
{
    private readonly IMediator _mediator;

    public ProductsController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetAll()
    {
        var query = new GetAllProductsQuery();
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<ProductDto>> GetById(Guid id)
    {
        var query = new GetProductByIdQuery(id);
        var product = await _mediator.Send(query);

        if (product == null)
            return NotFound(new { message = "Product not found" });

        return Ok(product);
    }

    [HttpGet("category/{categoryId}")]
    public async Task<ActionResult<IEnumerable<ProductDto>>> GetByCategory(Guid categoryId)
    {
        var query = new GetProductsByCategoryQuery(categoryId);
        var products = await _mediator.Send(query);
        return Ok(products);
    }

    [HttpPost]
    public async Task<ActionResult<ProductDto>> Create([FromBody] CreateProductDto dto)
    {
        var command = new CreateProductCommand(
            dto.Name,
            dto.Description,
            dto.Price,
            dto.Stock,
            dto.CategoryId,
            dto.Brand,
            dto.ImageUrl
        );

        var product = await _mediator.Send(command);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> Update(Guid id, [FromBody] UpdateProductDto dto)
    {
        var command = new UpdateProductCommand(
            id,
            dto.Name,
            dto.Description,
            dto.Price,
            dto.Stock,
            dto.CategoryId,
            dto.Brand,
            dto.ImageUrl,
            dto.IsActive
        );

        var updated = await _mediator.Send(command);

        if (!updated)
            return NotFound(new { message = "Product not found" });

        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(Guid id)
    {
        var command = new DeleteProductCommand(id);
        var deleted = await _mediator.Send(command);

        if (!deleted)
            return NotFound(new { message = "Product not found" });

        return NoContent();
    }
}
