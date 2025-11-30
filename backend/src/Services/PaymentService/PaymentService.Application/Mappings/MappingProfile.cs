using AutoMapper;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Entities;

namespace PaymentService.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<Payment, PaymentDto>()
            .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Amount))
            .ForMember(dest => dest.Currency, opt => opt.MapFrom(src => src.Amount.Currency));
    }
}
