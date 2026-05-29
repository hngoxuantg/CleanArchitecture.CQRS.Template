namespace Project.Application.Features.Products.Request
{
    public class CreateProductRequest
    {
        public string Name { get; set; }

        public string? Description { get; set; }

        public decimal Price { get; set; }

        public int? CategoryId { get; set; }
    }
}
