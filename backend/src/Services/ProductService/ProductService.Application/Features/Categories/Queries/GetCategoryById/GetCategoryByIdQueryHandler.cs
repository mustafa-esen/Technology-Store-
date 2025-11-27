using AutoMapper;
using MediatR;
using ProductService.Application.DTOs;
using ProductService.Application.Interfaces;

namespace ProductService.Application.Features.Categories.Queries.GetCategoryById;

public class GetCategoryByIdQueryHandler : IRequestHandler<GetCategoryByIdQuery, CategoryDto?>
{
    private readonly ICategoryRepository _categoryRepository;
    private readonly IMapper _mapper;

    public GetCategoryByIdQueryHandler(ICategoryRepository categoryRepository, IMapper mapper)
    {
        _categoryRepository = categoryRepository;
        _mapper = mapper;
    }

    public async Task<CategoryDto?> Handle(GetCategoryByIdQuery request, CancellationToken cancellationToken)
    {
        var category = await _categoryRepository.GetByIdAsync(request.Id);
        return category == null ? null : _mapper.Map<CategoryDto>(category);
    }
}
