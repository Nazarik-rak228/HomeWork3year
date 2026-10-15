namespace API_First_Lesson.Models
{
    public class OrderEventCreateDTO
    {
        public int? OrderId { get; set; }

        public int? UserId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public string EventType { get; set; } = null!;
    }
}
