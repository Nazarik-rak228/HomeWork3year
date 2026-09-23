namespace API_First_Lesson.Models
{
    public class OrderDTO
    {
        public int OrderId { get; set; }

        public int UserId { get; set; }

        public DateTime OrderDate { get; set; }

        public decimal TotalAmount { get; set; }
    }
}