using AutoMapper;
using DataAccess.Entities;
using Models.DTOs;

namespace EcommerceStripePayment.Profiles
{
    public class MappingProfiles : Profile
    {
        public MappingProfiles()
        {
            CreateMap<Product, ProductDto>().AfterMap((src, dest) => { dest.Price = src.Price * 10; }).ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<Payment, PaymentDto>().ReverseMap();
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore());
        }
    }

}
