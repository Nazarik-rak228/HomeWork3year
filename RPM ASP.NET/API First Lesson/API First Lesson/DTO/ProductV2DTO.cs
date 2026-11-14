namespace API_First_Lesson.Models
{
    /// <summary>
    /// Ответ версии 2.0 для списка товаров: данные переструктурированы
    /// (meta + data, вложенные product/pricing/availability/image).
    /// </summary>
    public class ProductV2ListDTO
    {
        public ProductV2MetaDTO Meta { get; set; } = new();

        public List<ProductV2DTO> Data { get; set; } = new();
    }

    public class ProductV2MetaDTO
    {
        public int Page { get; set; }

        public int PageSize { get; set; }

        public int TotalCount { get; set; }

        public int TotalPages { get; set; }
    }

    public class ProductV2DTO
    {
        public ProductV2InfoDTO Product { get; set; } = new();

        public ProductV2PricingDTO Pricing { get; set; } = new();

        public ProductV2AvailabilityDTO Availability { get; set; } = new();

        public ProductV2ImageDTO Image { get; set; } = new();
    }

    public class ProductV2InfoDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;

        public string? Description { get; set; }

        public ProductV2CategoryDTO Category { get; set; } = new();
    }

    public class ProductV2CategoryDTO
    {
        public int Id { get; set; }

        public string Name { get; set; } = null!;
    }

    public class ProductV2PricingDTO
    {
        public decimal Value { get; set; }

        public string Currency { get; set; } = "RUB";
    }

    public class ProductV2AvailabilityDTO
    {
        public bool InStock { get; set; }

        public int Quantity { get; set; }
    }

    public class ProductV2ImageDTO
    {
        public bool HasImage { get; set; }

        public string? MimeType { get; set; }
    }
}
