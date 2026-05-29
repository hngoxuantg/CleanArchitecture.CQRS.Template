using AutoFixture;
using AutoMapper;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Project.Application.Common.DTOs.Products;
using Project.Application.Common.Interfaces.IExternalServices.IStorageServices;
using Project.Application.Common.Mappers;
using Project.Application.Features.Products.Request;
using Project.Application.Features.Products.Shared.Interfaces;
using Project.Application.Features.Products.Shared.Services;
using Project.Domain.Entities.Business;
using Project.Domain.Interfaces.IRepositories.IBaseRepositories;
using System.Linq.Expressions;

namespace Project.UnitTest.Features.Shared
{
    public class ProductWriteServiceTests
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IProductWriteService _productWriteService;
        private readonly IMapper _mapper;
        private readonly IFileService _fileService;
        private readonly Fixture _fixture;

        public ProductWriteServiceTests()
        {
            _unitOfWork = Substitute.For<IUnitOfWork>();

            MapperConfiguration config = new MapperConfiguration(cfg =>
            {
                cfg.AddProfile<ProductProfile>();
            }, new LoggerFactory());

            _mapper = config.CreateMapper();

            _fileService = Substitute.For<IFileService>();

            _fixture = new Fixture();

            _productWriteService = new ProductWriteService(_unitOfWork, _mapper, _fileService);
        }

        [Fact]
        public async Task CreateProductAsync_WhenValid_ShouldCreate()
        {
            CreateProductRequest request = _fixture.Create<CreateProductRequest>();

            _unitOfWork.ProductRepository.IsExistsAsync(nameof(Product.Name), request.Name, Arg.Any<CancellationToken>())
                .Returns(false);

            _unitOfWork.CategoryRepository.IsExistsAsync(nameof(Category.Id), request.CategoryId, Arg.Any<CancellationToken>())
                .Returns(true);

            _unitOfWork.ProductRepository.CreateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
                .Returns(ci => ci.ArgAt<Product>(0));

            ProductDto result = await _productWriteService.CreateProductAsync(request);

            Assert.NotNull(result);
            Assert.Equal(request.Name, result.Name);

            await _unitOfWork.ProductRepository.Received(1)
                .CreateAsync(Arg.Is<Product>(p => p.Name == request.Name), Arg.Any<CancellationToken>());
        }

        [Fact]
        public async Task UploadImagesAsync_WhenFilesProvided_ShouldSaveAndAttach()
        {
            int productId = _fixture.Create<int>();

            Product product = _fixture.Build<Product>()
                .With(p => p.Id, productId)
                .Create();

            _unitOfWork.ProductRepository.GetOneUntrackedAsync<Product>(
                Arg.Any<Expression<Func<Product, bool>>>(),
                Arg.Any<Expression<Func<IQueryable<Product>, IOrderedQueryable<Product>>>>(),
                Arg.Any<Expression<Func<Product, Product>>>(),
                Arg.Any<Expression<Func<IQueryable<Product>, IQueryable<Product>>>>(),
                Arg.Any<CancellationToken>())
                .Returns(product);

            _unitOfWork.ProductRepository.UpdateAsync(Arg.Any<Product>(), Arg.Any<CancellationToken>())
                .Returns(ci => ci.ArgAt<Product>(0));

            var relativePaths = new List<string> { $"/uploads/products/{productId}/img1.jpg" };
            var absoluteUrls = new List<string> { $"http://localhost/uploads/products/{productId}/img1.jpg" };

            _fileService.SaveImagesAsync(Arg.Any<IList<IFormFile>>(), Arg.Any<string>(), Arg.Any<CancellationToken>())
                .Returns(relativePaths);

            _fileService.GetAbsoluteUrls(Arg.Is<IEnumerable<string>>(r => r.SequenceEqual(relativePaths)))
                .Returns(absoluteUrls);

            var fakeFile = Substitute.For<IFormFile>();

            var result = await _productWriteService.UploadImagesAsync(productId, new List<IFormFile> { fakeFile });

            Assert.NotNull(result);
            Assert.Contains(absoluteUrls.First(), result);

            await _unitOfWork.ProductRepository.Received(1)
                .UpdateAsync(Arg.Is<Product>(p => p.ProductImages.Any(pi => pi.ImageUrl == relativePaths.First())), Arg.Any<CancellationToken>());
        }
    }
}
