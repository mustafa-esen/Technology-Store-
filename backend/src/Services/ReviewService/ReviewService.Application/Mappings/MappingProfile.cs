using AutoMapper;
using ReviewService.Application.DTOs;
using ReviewService.Domain.Entities;

namespace ReviewService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<ProductReview, ReviewDto>();
        CreateMap<CreateReviewDto, ProductReview>()
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls ?? new List<string>()));
        CreateMap<UpdateReviewDto, ProductReview>()
            .ForMember(dest => dest.ImageUrls, opt => opt.MapFrom(src => src.ImageUrls ?? new List<string>()));
    }
}
