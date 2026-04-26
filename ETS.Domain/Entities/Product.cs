using ETS.Domain.Common;
using ETS.Domain.Enums;

namespace ETS.Domain.Entities
{
    public class Product : Entity<Guid>
    {
        public string Name { get; private set; }
        public ProductCategory Category { get; private set; }
        public string ProductCode { get; private set; }
        public string Description { get; private set; }
        public string Brand { get; private set; }
        public string PublishedBy { get; private set; }
        public DateTime? PublishedOn { get; private set; }
        public PublishStatus PublishStatus { get; private set; }
        public decimal Price { get; private set; }
        public bool IsActive { get; private set; }
        public ICollection<ProductImage> Images { get; set; } = [];
        

        public static Product Create(string name, ProductCategory category, string productCode, string description,
            string brand, decimal price)
        {
            return new Product
            {
                Description = description,
                IsActive = true,
                Brand = brand,
                Name = name,
                Category = category,
                Price = price,
                PublishStatus = PublishStatus.Unpublished,
                Id = Guid.NewGuid()
            };
        }

        public void UpdatePublishStatus(PublishStatus publishStatus)
        {
            PublishStatus = publishStatus;
        }

    }
}
