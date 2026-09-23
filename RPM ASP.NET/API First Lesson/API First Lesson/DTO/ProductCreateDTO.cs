namespace API_First_Lesson.Models
{
    public class ProductCreateDTO
    {
        public string ProductName { get; set; } = null!;

        public decimal Price { get; set; }

        public int CategoryId { get; set; }

        public string? Discription { get; set; }

        public byte[]? ImageData { get; set; }

        public string? ImageMimeType { get; set; }
    }
}