using Project.Domain.Entities.Base;
using System.ComponentModel.DataAnnotations;

namespace Project.Domain.Entities.Business
{
    public class Product : SoftDeleteEntity
    {
        [Required, MaxLength(55)]
        public string Name { get; set; }
        [MaxLength(255)]
        public string? Description { get; set; }
        public string? ImageUrl { get; set; }
        [Required]
        public decimal Price { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

        public Product()
        {
        }

        public Product(
            string name,
            decimal price,
            int? categoryId = null,
            string? description = null,
            string? imageUrl = null)
        {
            Name = name;
            Price = price;
            CategoryId = categoryId;
            Description = description;
            ImageUrl = imageUrl;
        }
    }
}
