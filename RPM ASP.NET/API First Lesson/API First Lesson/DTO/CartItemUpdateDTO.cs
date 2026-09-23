namespace API_First_Lesson.Models
{
    public class CartItemUpdateDTO
    {
        public int CartId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }
    }
}