namespace API_First_Lesson.Models
{
    public class CartItemDTO
    {
        public int CartItemId { get; set; }

        public int CartId { get; set; }

        public int ProductId { get; set; }

        public string? ProductName { get; set; }

        public int Quantity { get; set; }
    }
}
