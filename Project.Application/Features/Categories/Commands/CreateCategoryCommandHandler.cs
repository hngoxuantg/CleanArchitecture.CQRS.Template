using AutoMapper;
using MediatR;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Common.Exceptions;
using Project.Domain.Entities.Business;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Categories.Commands
{
    public class CreateCategoryCommandHandler : IRequestHandler<CreateCategoryCommand, CategoryDto>
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public CreateCategoryCommandHandler(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
        }

        public async Task<CategoryDto> Handle(CreateCategoryCommand command, CancellationToken cancellationToken)
        {
            return await CreateCategoryAsync(command.Request, cancellationToken);
        }
        private async Task<CategoryDto> CreateCategoryAsync(CreateCategoryRequest request, CancellationToken cancellationToken = default)
        {
            if (await _unitOfWork.CategoryRepository.IsExistsAsync(nameof(Category.Name), request.Name))
                throw new ValidatorException(nameof(CreateCategoryRequest.Name), $"Category with name {request.Name} already exists.");

            Category category = _mapper.Map<Category>(request);

            await _unitOfWork.CategoryRepository.CreateAsync(category, cancellationToken);

            return _mapper.Map<CategoryDto>(category);
        }
    }
}
