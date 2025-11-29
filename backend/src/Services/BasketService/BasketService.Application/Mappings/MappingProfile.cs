using AutoMapper;
using BasketService.Application.DTOs;
using BasketService.Domain.Entities;

namespace BasketService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Basket, BasketDto>().ReverseMap();
        CreateMap<BasketItem, BasketItemDto>().ReverseMap();
    }
}
