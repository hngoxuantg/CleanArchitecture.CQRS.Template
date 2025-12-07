using Project.Domain.Entities.Base;

namespace Project.Domain.Entities.Business
{
    public class ProductImage : SoftDeleteEntity
    {
        public string ImageUrl { get; set; }

        public int ProductId { get; set; }

        public Product Product { get; set; }

        public ProductImage()
        {
        }
        public ProductImage(string imageUrl, int productId)
        {
            ImageUrl = imageUrl;
            ProductId = productId;
        }
    }
}
