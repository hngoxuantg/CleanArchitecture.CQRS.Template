using AutoMapper;
using Project.Application.Interfaces.IServices;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Services
{
    public class ProductService : IProductService
    {
        private readonly IMapper _mapper;
        private readonly IUnitOfWork _unitOfWork;
        public ProductService(IMapper mapper, IUnitOfWork unitOfWork)
        {
            _mapper = mapper;
            _unitOfWork = unitOfWork;
        }
    }
}
