using Project.Domain.Entities.Base;

namespace Project.Domain.Entities.Business
{
    public class Product : SoftDeleteEntity, IAggregateRoot
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int? CategoryId { get; set; }

        public Category? Category { get; set; }


        private readonly List<ProductImage> _productImages = new List<ProductImage>();
        public IReadOnlyCollection<ProductImage> ProductImages => _productImages.AsReadOnly();

        public Product()
        {
        }

        public Product(
            string name,
            decimal price,
            int? categoryId = null,
            string? description = null)
        {
            Name = name;
            Price = price;
            CategoryId = categoryId;
            Description = description;
        }

        public void AddImages(List<string> imageUrls)
        {
            foreach (var imageUrl in imageUrls)
            {
                ProductImage productImage = new ProductImage()
                {
                    ImageUrl = imageUrl
                };

                _productImages.Add(productImage);
            }
        }

        public void AddImage(string imageUrl)
        {
            ProductImage productImage = new ProductImage()
            {
                ImageUrl = imageUrl
            };
            _productImages.Add(productImage);
        }
    }
}
