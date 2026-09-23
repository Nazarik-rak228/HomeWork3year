namespace API_First_Lesson.Models
{
    public class OrderItemDTO
    {
        public int OrderItemId { get; set; }

        public int OrderId { get; set; }

        public int ProductId { get; set; }

        public int Quantity { get; set; }

        public decimal PriceAtPurchase { get; set; }
    }
}