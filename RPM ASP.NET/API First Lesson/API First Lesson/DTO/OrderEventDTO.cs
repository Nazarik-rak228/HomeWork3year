using System;

namespace API_First_Lesson.Models
{
    public class OrderEventDTO
    {
        public int OrderEventId { get; set; }

        public int? OrderId { get; set; }

        public int? UserId { get; set; }

        public string? Username { get; set; }

        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public int Quantity { get; set; }

        public string EventType { get; set; } = null!;

        public DateTime EventDate { get; set; }
    }
}
