using AutoMapper;
using ProductService.Application.DTOs;
using ProductService.Application.Features.Products.Commands.CreateProduct;
using ProductService.Application.Features.Products.Commands.UpdateProduct;
using ProductService.Application.Features.Categories.Commands.CreateCategory;
using ProductService.Domain.Entities;

namespace ProductService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Product, ProductDto>()
            .ForMember(dest => dest.CategoryName,
                opt => opt.MapFrom(src => src.Category != null ? src.Category.Name : "Unknown"));

        CreateMap<CreateProductCommand, Product>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore());

        CreateMap<UpdateProductCommand, Product>()
            .ForMember(dest => dest.CreatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => DateTime.UtcNow))
            .ForMember(dest => dest.Category, opt => opt.Ignore());

        CreateMap<Category, CategoryDto>();

        CreateMap<CreateCategoryCommand, Category>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Products, opt => opt.Ignore());

        CreateMap<CreateCategoryDto, Category>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.Products, opt => opt.Ignore());

        CreateMap<CreateProductDto, Product>()
            .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => true))
            .ForMember(dest => dest.UpdatedAt, opt => opt.Ignore())
            .ForMember(dest => dest.Category, opt => opt.Ignore());
    }
}