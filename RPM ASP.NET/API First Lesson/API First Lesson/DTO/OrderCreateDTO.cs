namespace API_First_Lesson.Models
{
    public class OrderCreateDTO
    {
        public int UserId { get; set; }

        public decimal TotalAmount { get; set; }
    }
}