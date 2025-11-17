using AutoMapper;
using Project.Application.Interfaces.IServices;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public CategoryService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
    }
}
