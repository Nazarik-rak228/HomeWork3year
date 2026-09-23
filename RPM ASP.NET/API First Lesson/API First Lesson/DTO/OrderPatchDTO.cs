namespace API_First_Lesson.Models
{
    public class OrderPatchDTO
    {
        public int? UserId { get; set; }

        public decimal? TotalAmount { get; set; }
    }
}