using MediatR;

namespace ProductService.Application.Features.Products.Commands.DeleteProduct;

public record DeleteProductCommand(Guid Id) : IRequest<bool>;
