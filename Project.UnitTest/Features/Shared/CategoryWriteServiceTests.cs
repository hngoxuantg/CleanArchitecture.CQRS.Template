using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using NSubstitute;
using Project.Application.Common.DTOs.Categories;
using Project.Application.Common.DTOs.Mails;
using Project.Application.Common.Interfaces.IBackgroundJobs;
using Project.Application.Common.Interfaces.IServices;
using Project.Application.Common.Mappers;
using Project.Application.Features.Categories.Request;
using Project.Application.Features.Categories.Shared.Interfaces;
using Project.Application.Features.Categories.Shared.Services;
using Project.Domain.Entities.Business;
using Project.Domain.Entities.Identity_Auth;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;

namespace Project.UnitTest.Features.Shared
{
    public class CategoryWriteServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly ICategoryWriteService _categoryWriteService;
        private readonly IMapper _mapper;
        private readonly IBackgroundJobService _backgroundJobService;
        private readonly UserManager<User> _userManager;
        private readonly ICurrentUserService _currentUserService;
        private readonly Fixture _fixture;

        public CategoryWriteServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<CategoryProfile>();
            });

            _mapper = config.CreateMapper();

            _backgroundJobService = Substitute.For<IBackgroundJobService>();

            IUserStore<User> userStore = Substitute.For<IUserStore<User>>();
            _userManager = Substitute.For<UserManager<User>>(
                userStore, null, null, null, null, null, null, null, null);

            _currentUserService = Substitute.For<ICurrentUserService>();

            _fixture = new Fixture();

            _categoryWriteService = new CategoryWriteService(
                _unitOfWork,
                _backgroundJobService,
                _userManager,
                _currentUserService,
                _mapper);
        }

        [Fact]
        public async Task CreateCategoryAsync_WhenRequestIsValid_ShouldCreateCategory()
        {
            int userId = _fixture.Create<int>();

            CreateCategoryRequest request = _fixture.Create<CreateCategoryRequest>();

            _unitOfWork.CategoryRepository.IsExistsAsync(nameof(Category.Name), request.Name, Arg.Any<CancellationToken>())
                .Returns(false);

            _unitOfWork.CategoryRepository.CreateAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>())
                .Returns(ci => ci.ArgAt<Category>(0));

            _currentUserService.UserId.Returns(userId);

            _userManager.FindByIdAsync(userId.ToString())
                .Returns(_fixture.Create<User>());

            CategoryDto result = await _categoryWriteService.CreateCategoryAsync(request);

            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);

            await _unitOfWork.CategoryRepository.Received(1)
                .CreateAsync(Arg.Is<Category>(c => c.Name == request.Name), Arg.Any<CancellationToken>());

            await _userManager.Received(1)
                .FindByIdAsync(userId.ToString());

            _backgroundJobService.Received(1)
                .EnqueueSendEmail(Arg.Do<EmailDto>(e =>
                {
                    Assert.Equal("CategoryCreated", e.TemplateName);
                    Assert.Equal(request.Name, e.TemplateData["CategoryName"]);
                }));
        }

        [Fact]
        public async Task UpdateCategoryAsync_WhenCategoryExists_ShouldUpdateSuccessfully()
        {
            int categoryId = _fixture.Create<int>();

            UpdateCategoryRequest request = _fixture.Create<UpdateCategoryRequest>();
            Category category = _fixture
                .Build<Category>()
                .With(x => x.Id, categoryId)
                .Create();

            _unitOfWork.CategoryRepository
                .IsExistsForUpdateAsync(categoryId, nameof(Category.Name), request.Name, Arg.Any<CancellationToken>())
                .Returns(false);

            _unitOfWork.CategoryRepository
                .GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
                .Returns(category);

            _unitOfWork.CategoryRepository
                .UpdateAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>())
                .Returns(ci => ci.ArgAt<Category>(0));

            CategoryDto result = await _categoryWriteService.UpdateCategoryAsync(categoryId, request);

            Assert.NotNull(result);
            Assert.Equal(categoryId, result.Id);

            await _unitOfWork.CategoryRepository.Received(1)
                .UpdateAsync(Arg.Is<Category>(c => c.Id == categoryId && c.Name == request.Name), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UpdateDescriptionCategoryAsync_WhenCategoryExists_ShouldUpdateDescriptionOnly()
        {
            int categoryId = _fixture.Create<int>();
            string newDescription = "New Description Updated";
            Category existingCategory = _fixture.Build<Category>()
                .With(x => x.Id, categoryId)
                .Create();

            _unitOfWork.CategoryRepository
                .GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
                .Returns(existingCategory);

            _unitOfWork.CategoryRepository
                .UpdateAsync(Arg.Any<Category>(), Arg.Any<CancellationToken>())
                .Returns(ci => ci.ArgAt<Category>(0));

            CategoryDto result = await _categoryWriteService.UpdateDescriptionCategoryAsync(categoryId, newDescription);

            Assert.NotNull(result);
            Assert.Equal(newDescription, result.Description);

            await _unitOfWork.CategoryRepository.Received(1)
                .UpdateAsync(Arg.Is<Category>(c => c.Description == newDescription), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task DeleteCategoryAsync_WhenCategoryExists_ShouldReturnTrue()
        {
            int categoryId = _fixture.Create<int>();

            Category existingCategory = _fixture.Build<Category>()
                .With(x => x.Id, categoryId)
                .Create();

            _unitOfWork.CategoryRepository
                .GetByIdAsync(categoryId, Arg.Any<CancellationToken>())
                .Returns(existingCategory);

            bool result = await _categoryWriteService.DeleteCategoryAsync(categoryId);

            Assert.True(result);

            await _unitOfWork.CategoryRepository.Received(1)
                .DeleteAsync(existingCategory, Arg.Any<CancellationToken>());
        }
    }
}
