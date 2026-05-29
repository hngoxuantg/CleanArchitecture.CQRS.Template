using AutoMapper;
using Project.Application.Common.DTOs.Products;
using Project.Application.Features.Products.Request;
using Project.Domain.Entities.Business;

namespace Project.Application.Common.Mappers
{
    public class ProductProfile : Profile
    {
        public ProductProfile()
        {
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.Id,
                    opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price,
                    opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.CategoryId,
                    opt => opt.MapFrom(src => src.CategoryId))
                .ForMember(dest => dest.ImageUrls,
                    opt => opt.MapFrom(src => src.ProductImages.Select(pi => pi.ImageUrl)));

            CreateMap<CreateProductRequest, Product>()
                .ConstructUsing(src => new Product(
                    src.Name,
                    src.Price,
                    src.CategoryId,
                    src.Description));

            CreateMap<UpdateProductRequest, Product>()
                .ForMember(dest => dest.Name,
                    opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description,
                    opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.Price,
                    opt => opt.MapFrom(src => src.Price))
                .ForMember(dest => dest.CategoryId,
                    opt => opt.MapFrom(src => src.CategoryId));
        }
    }
}
