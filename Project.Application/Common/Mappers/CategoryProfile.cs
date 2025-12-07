using AutoMapper;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Features.Categories.Request;
using Project.Domain.Entities.Business;

namespace Project.Application.Common.Mappers
{
    public class CategoryProfile : Profile
    {
        public CategoryProfile()
        {
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.Id,
                opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Description));

            CreateMap<CreateCategoryRequest, Category>()
                .ConstructUsing(src => new Category(
                    src.Name,
                    src.Description));

            CreateMap<UpdateCategoryRequest, Category>()
                .ForMember(dest => dest.Name,
                opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Description,
                opt => opt.MapFrom(src => src.Description));
        }
    }
}
