using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Products.Queries.GetProductsByCategory;

public class GetProductsByCategoryQueryHandler : IRequestHandler<GetProductsByCategoryQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetProductsByCategoryQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetProductsByCategoryQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetByCategoryAsync(request.CategoryId);
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
}
