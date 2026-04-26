using ETS.Domain.Common;

namespace ETS.Domain.Entities
{
    public class ProductImage : Entity<Guid>
    {
        public Guid ProductId { get; private set; }
        public string ImageUrl { get; private set; }
        public string? AltText { get; private set; }
        public bool IsPrimary { get; private set; }
        public string? ContentType { get; private set; }


        public ProductImage Create(Guid productId, string imageUrl, string altText, bool isPrimary, string contentType)
        {
            return new ProductImage
            {
                AltText = altText,
                ContentType = contentType,
                ImageUrl = imageUrl,
                IsPrimary = isPrimary,
                ProductId = productId
            };
        }
    }
}
