using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Common.DTOs.Mails;
using Project.Application.Common.Exceptions;
using Project.Application.Common.Interfaces.IBackgroundJobs;
using Project.Application.Common.Interfaces.IServices;
using Project.Application.Features.Categories.Request;
using Project.Application.Features.Categories.Shared.Interfaces;
using Project.Domain.Entities.Business;
using Project.Domain.Entities.Identity_Auth;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.Application.Features.Categories.Shared.Services
{
    public class CategoryWriteService : ICategoryWriteService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IBackgroundJobService _backgroundJob;
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;

        public CategoryWriteService(
            IUnitOfWork unitOfWork,
            IBackgroundJobService backgroundJob,
            UserManager<User> userManager,
            ICurrentUserService currentUserService,
            IMapper mapper)
        {
            _unitOfWork = unitOfWork;
            _mapper = mapper;
            _userManager = userManager;
            _currentUserService = currentUserService;
            _backgroundJob = backgroundJob;
        }

        public async Task<CategoryDto> CreateCategoryAsync(
            CreateCategoryRequest request,
            CancellationToken cancellationToken = default)
        {
            if (await IsValidCategoryNameAsync(request.Name, cancellationToken))
                throw new ValidatorException(
                    nameof(CreateCategoryRequest.Name), $"Category with name {request.Name} already exists.");

            Category category = _mapper.Map<Category>(request);

            await _unitOfWork.CategoryRepository.CreateAsync(category, cancellationToken);

            SendNotificationEmailAsync(category.Name,
                await GetUserByIdAysnc(_currentUserService.UserId.ToString() ?? string.Empty),
                cancellationToken);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> UpdateCategoryAsync(
            int id,
            UpdateCategoryRequest request,
            CancellationToken cancellationToken = default)
        {
            if (await IsValidCategoryNameAsync(id, request.Name, cancellationToken))
                throw new ValidatorException(
                    nameof(UpdateCategoryRequest.Name), $"Category with name {request.Name} already exists.");

            Category category = await GetCategoryByIdAsync(id, cancellationToken);

            _mapper.Map(request, category);

            await _unitOfWork.CategoryRepository.UpdateAsync(category, cancellationToken);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<CategoryDto> UpdateDescriptionCategoryAsync(
            int id,
            string description,
            CancellationToken cancellationToken = default)
        {
            Category category = await GetCategoryByIdAsync(id, cancellationToken);

            category.SetDescription(description);

            await _unitOfWork.CategoryRepository.UpdateAsync(category, cancellationToken);

            return _mapper.Map<CategoryDto>(category);
        }

        public async Task<bool> DeleteCategoryAsync(int id, CancellationToken cancellationToken = default)
        {
            Category category = await GetCategoryByIdAsync(id, cancellationToken);

            await _unitOfWork.CategoryRepository.DeleteAsync(category, cancellationToken);

            return true;
        }

        private async Task<Category> GetCategoryByIdAsync(int id, CancellationToken cancellation = default)
        {
            return await _unitOfWork.CategoryRepository.GetByIdAsync(id, cancellation)
                ?? throw new NotFoundException($"Category with ID {id} not found.");
        }

        private async Task<bool> IsValidCategoryNameAsync(string name, CancellationToken cancellation = default)
        {
            return await _unitOfWork.CategoryRepository.IsExistsAsync(nameof(Category.Name), name, cancellation);
        }

        private async Task<bool> IsValidCategoryNameAsync(int id, string name, CancellationToken cancellation = default)
        {
            return await _unitOfWork.CategoryRepository.IsExistsForUpdateAsync(
                id,
                nameof(Category.Name),
                name,
                cancellation);
        }

        private void SendNotificationEmailAsync(string categoryName, User user, CancellationToken cancellation = default)
        {
            EmailDto email = new EmailDto
            {
                To = user.Email ?? string.Empty,
                Subject = $"[Notification] New Category Created: {categoryName}",
                TemplateName = "CategoryCreated",
                TemplateData = new Dictionary<string, string>
                {
                    { "AdminName", "Ngô Xuân Hải" },
                    { "CategoryName", categoryName },
                    { "CreatedDate", DateTime.Now.ToString("dd/MM/yyyy HH:mm") }
                }
            };

            _backgroundJob.EnqueueSendEmail(email);
        }

        private async Task<User> GetUserByIdAysnc(string id, CancellationToken cancellationToken = default)
        {
            return await _userManager.FindByIdAsync(id) ?? throw new NotFoundException("User not found.");
        }
    }
}
