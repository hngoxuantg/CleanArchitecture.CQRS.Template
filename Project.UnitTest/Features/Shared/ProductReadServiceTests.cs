using AutoFixture;
using NSubstitute;
using Project.Application.Common.DTOs.Products;
using Project.Application.Common.Interfaces.IExternalServices.IStorageServices;
using Project.Application.Features.Products.Shared.Interfaces;
using Project.Application.Features.Products.Shared.Services;
using Project.Domain.Entities.Business;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;
using System.Linq.Expressions;

namespace Project.UnitTest.Features.Shared
{
    public class ProductReadServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductReadService _productReadService;
        private readonly IFileService _fileService;
        private readonly Fixture _fixture;

        public ProductReadServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();
            _fileService = Substitute.For<IFileService>();
            _fixture = new Fixture();
            _productReadService = new ProductReadService(_unitOfWork, _fileService);
        }

        [Fact]
        public async Task GetByIdAsync_WhenProductHasImages_ShouldReturnAbsoluteUrls()
        {
            int productId = _fixture.Create<int>();

            Product product = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .Create();

            product.AddImage($"/uploads/products/{productId}/img1.jpg");

            var productDto = new ProductDto
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId,
                ImageUrls = new List<string> { $"http://localhost/uploads/products/{productId}/img1.jpg" }
            };

            _unitOfWork.ProductRepository.GetOneUntrackedAsync<ProductDto>(
                Arg.Any<Expression<Func<Product, bool>>>(),
                Arg.Any<Expression<Func<IQueryable<Product>, IOrderedQueryable<Product>>>>(),
                Arg.Any<Expression<Func<Product, ProductDto>>>(),
                Arg.Any<Expression<Func<IQueryable<Product>, IQueryable<Product>>>>(),
                Arg.Any<CancellationToken>())
                .Returns(productDto);

            _fileService.GetAbsoluteUrls(Arg.Any<IEnumerable<string>>())
                .Returns(productDto.ImageUrls);

            var dto = await _productReadService.GetByIdAsync(productId);

            Assert.NotNull(dto);
            Assert.NotNull(dto.ImageUrls);
            Assert.Single(dto.ImageUrls);
            Assert.Equal(productDto.ImageUrls.First(), dto.ImageUrls.First());
        }
    }
}
