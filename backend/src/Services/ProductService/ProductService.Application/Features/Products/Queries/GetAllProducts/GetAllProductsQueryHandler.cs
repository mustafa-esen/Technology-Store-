using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Products.Queries.GetAllProducts;

public class GetAllProductsQueryHandler : IRequestHandler<GetAllProductsQuery, IEnumerable<ProductDto>>
{
    private readonly IProductRepository _productRepository;
    private readonly IMapper _mapper;

    public GetAllProductsQueryHandler(IProductRepository productRepository, IMapper mapper)
    {
        _productRepository = productRepository;
        _mapper = mapper;
    }

    public async Task<IEnumerable<ProductDto>> Handle(GetAllProductsQuery request, CancellationToken cancellationToken)
    {
        var products = await _productRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<ProductDto>>(products);
    }
}
