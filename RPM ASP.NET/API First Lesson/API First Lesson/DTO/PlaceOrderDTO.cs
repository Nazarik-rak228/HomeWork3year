using System.Collections.Generic;

namespace API_First_Lesson.Models
{
    public class PlaceOrderDTO
    {
        public int UserId { get; set; }

        public List<PlaceOrderItemDTO> Items { get; set; } = new List<PlaceOrderItemDTO>();
    }
}
