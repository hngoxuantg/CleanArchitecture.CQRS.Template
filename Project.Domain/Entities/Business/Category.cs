using Project.Domain.Entities.Base;

namespace Project.Domain.Entities.Business
{
    public class Category : SoftDeleteEntity, IAggregateRoot
    {
        public string Name { get; set; }

        public string? Description { get; set; }


        private readonly List<Product> _products = new List<Product>();
        public IReadOnlyCollection<Product> Products => _products.AsReadOnly();

        public void SetDescription(string description)
        {
            Description = description;
        }

        public Category()
        {
        }

        public Category(string name, string? description = null)
        {
            Name = name;
            Description = description;
        }
    }
}
